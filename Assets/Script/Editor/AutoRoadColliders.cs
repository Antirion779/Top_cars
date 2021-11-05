using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class AutoRoadColliders : EditorWindow
{
    [MenuItem("Auto Road Colliders/ Tool")]
    public static void ShowWindow()
    {
        GetWindow<AutoRoadColliders>("Auto road collider - Tool");
    }

    // Auto tool vars
    public LineRenderer trackLr;
    public LineRenderer grassLr;
    float minDistanceBetweenEdgePoints = 2f;

    EdgeCollider2D trackCollider;
    EdgeCollider2D grassInsideCollider;
    EdgeCollider2D grassOutsideCollider;
    EdgeCollider2D waterInsideCollider;
    EdgeCollider2D waterOutsideCollider;


    // Others
    public LineRenderer lr;
    public GameObject colliderTarget;

    float distance = 0.2f;

    private void OnGUI()
    {
        #region Full auto tool
        GUILayout.Label("Full auto tool");
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Track line renderer");
            trackLr = (LineRenderer)EditorGUILayout.ObjectField(trackLr, typeof(LineRenderer), true);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Grass line renderer");
            grassLr = (LineRenderer)EditorGUILayout.ObjectField(grassLr, typeof(LineRenderer), true);
        }
        GUILayout.Space(5);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Distance minimum between two points of the edge collider");
            minDistanceBetweenEdgePoints = EditorGUILayout.FloatField(minDistanceBetweenEdgePoints);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Generate colliders!"))
        {
            // Triggers initialisation
            if (trackCollider == null) // --> We assume everything is null then
            {
                Transform triggersParent = new GameObject("Triggers").transform;
                triggersParent.position = trackLr.transform.position;

                trackCollider = new GameObject("Track trigger").AddComponent<EdgeCollider2D>();
                trackCollider.gameObject.tag = "Track";
                trackCollider.transform.SetParent(triggersParent);

                GameObject grassGo = new GameObject("Grass triggers");
                grassGo.tag = "Grass";
                grassGo.transform.SetParent(triggersParent);
                grassInsideCollider = grassGo.AddComponent<EdgeCollider2D>();
                grassOutsideCollider = grassGo.AddComponent<EdgeCollider2D>();

                GameObject waterGo = new GameObject("Water triggers");
                waterGo.tag = "Water";
                waterGo.transform.SetParent(triggersParent);
                waterInsideCollider = waterGo.AddComponent<EdgeCollider2D>();
                waterOutsideCollider = waterGo.AddComponent<EdgeCollider2D>();
            }

            // Lr initialisation
            grassLr.loop = true;
            trackLr.loop = true;

            // Form triggers
            float borderEdgeRadius = (grassLr.startWidth - trackLr.startWidth) / 4f;
            float grassDistance = grassLr.startWidth / 2f - borderEdgeRadius;
            FormLrInsideBorder(ref grassInsideCollider, grassLr, grassDistance, borderEdgeRadius);
            FormLrOutsideBorder(ref grassOutsideCollider, grassLr, grassDistance, borderEdgeRadius);

            float waterDistance = grassLr.startWidth / 2f + borderEdgeRadius;
            FormLrInsideBorder(ref waterInsideCollider, grassLr, waterDistance, borderEdgeRadius);
            FormLrOutsideBorder(ref waterOutsideCollider, grassLr, waterDistance, borderEdgeRadius);

            FormLrInside(ref trackCollider, trackLr);

        }
        #endregion
        GUILayout.Space(100);

        GUILayout.Label("---------------------------------------------------------------------------------------------------------------------------------------------------------------");
        GUILayout.Space(10);

        #region Other tools
        GUILayout.Label("Other tools");
        GUILayout.Space(15);

        // Line renderer
        using (new EditorGUILayout.VerticalScope())
        {

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Line renderer");
                lr = (LineRenderer)EditorGUILayout.ObjectField(lr, typeof(LineRenderer), true);
                if (GUILayout.Button("Fix line renderer Z points"))
                {
                    Undo.RecordObject(lr, "Set all points Z coordinates to 0");
                    for (int i = 0; i < lr.positionCount; i++)
                    {
                        Vector3 newPos = lr.GetPosition(i);
                        newPos.z = 0;
                        lr.SetPosition(i, newPos);
                    }
                }
            }

            // Collider target
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Collider target");
                colliderTarget = (GameObject)EditorGUILayout.ObjectField(colliderTarget, typeof(GameObject), true);
                if (GUILayout.Button("Remove"))
                {
                    if (colliderTarget != null) colliderTarget = null;
                }
            }
        }

        // Functions
        GUILayout.Space(10);

        // Fill function
        if (GUILayout.Button("Fill")) AdaptColliderToFill();
        GUILayout.Space(10);

        // Other functions
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Line thickness");
            distance = EditorGUILayout.FloatField(distance);
            //debugDistance = EditorGUILayout.Toggle(debugDistance);
        }
        using (new GUILayout.VerticalScope())
        {
            // Around function
            if (GUILayout.Button("Around")) AdaptColliderAround(distance);
            GUILayout.Space(5);
            // Interior border only function
            if (GUILayout.Button("Inside border")) AdaptColliderAround(distance, true, false);
            GUILayout.Space(5);
            // Exterior border only function
            if (GUILayout.Button("Exterior border")) AdaptColliderAround(distance, false, true);
        }
        #endregion

        //if (debugDistance)
        //{
        //    if(lr != null)
        //    {
        //        Gizmos.color = Color.cyan;
        //        for (int i = 0; i < lr.positionCount; i++)
        //        {
        //            Handles.CircleHandleCap(500, lr.GetPosition(i), Quaternion.identity, distance, EventType.Ignore);
        //            //Gizmos.DrawWireSphere(lr.GetPosition(i), distance);
        //        }
        //    }
        //}
    }

    #region Auto tool func
    void FormLrInsideBorder(ref EdgeCollider2D _edgeCollider, LineRenderer _lr, float distance, float edgeRadius)
    {
        List<Vector2> newPoints = new List<Vector2>();

        for (int i = 0; i < _lr.positionCount; i++)
        {
            Vector2 nextPoint = i + 1 < _lr.positionCount ? _lr.GetPosition(i + 1) : _lr.GetPosition(0);
            Vector2 nextDir = (nextPoint - (Vector2)_lr.GetPosition(i)).normalized;

            Vector2 newPoint = (Vector2)_lr.GetPosition(i) + new Vector2(nextDir.y, -nextDir.x) * distance;
            Action addPoint = () =>
            {
                newPoints.Add(newPoint);
                Debug.DrawRay((Vector2)_lr.GetPosition(i), new Vector2(nextDir.y, -nextDir.x) * distance, Color.magenta, 10f);
            };
            if (i == 0) addPoint?.Invoke();
            else if (Vector2.Distance(newPoint, newPoints[newPoints.Count - 1]) > minDistanceBetweenEdgePoints)
            {
                addPoint?.Invoke();
            }
        }

        Undo.RecordObject(_edgeCollider, "Changed edga radius and all points of EdgeCollider2D");
        _edgeCollider.SetPoints(newPoints);
        _edgeCollider.edgeRadius = edgeRadius;
    }
    void FormLrOutsideBorder(ref EdgeCollider2D _edgeCollider, LineRenderer _lr, float distance, float edgeRadius)
    {
        List<Vector2> newPoints = new List<Vector2>();
        Debug.Log(_lr.positionCount);

        for (int i = _lr.positionCount - 1; i >= 0; i--)
        {
            Vector2 nextPoint = i - 1 >= 0 ? _lr.GetPosition(i - 1) : _lr.GetPosition(_lr.positionCount - 1);
            Vector2 nextDir = (nextPoint - (Vector2)_lr.GetPosition(i)).normalized;

            Vector2 newPoint = (Vector2)_lr.GetPosition(i) - new Vector2(-nextDir.y, nextDir.x) * distance;
            Action addPoint = () =>
            {
                newPoints.Add(newPoint);
                Debug.DrawRay((Vector2)_lr.GetPosition(i), -new Vector2(-nextDir.y, nextDir.x) * distance, Color.blue, 10f);
            };
            if (newPoints.Count == 0) addPoint?.Invoke();
            else if (Vector2.Distance(newPoint, newPoints[newPoints.Count - 1]) > minDistanceBetweenEdgePoints)
            {
                addPoint?.Invoke();
            }
        }

        Undo.RecordObject(_edgeCollider, "Changed edga radius and all points of EdgeCollider2D");
        _edgeCollider.SetPoints(newPoints);
        _edgeCollider.edgeRadius = edgeRadius;
    }
    void FormLrInside(ref EdgeCollider2D _edgeCollider, LineRenderer _lr)
    {
        List<Vector2> newPoints = new List<Vector2>();
        for (int i = 0; i < _lr.positionCount; i++)
        {
            newPoints.Add(_lr.GetPosition(i));
        }
        newPoints.Add(_lr.GetPosition(0));

        Undo.RecordObject(_edgeCollider, "Changed edga radius and all points of EdgeCollider2D");
        _edgeCollider.SetPoints(newPoints);
        _edgeCollider.edgeRadius = _lr.startWidth / 2f;
    }
    #endregion

    #region Other tools func
    public void AdaptColliderToFill()
    {
        if (lr == null) return;
        if (colliderTarget == null)
        {
            colliderTarget = new GameObject("Collider target");
            Selection.SetActiveObjectWithContext(colliderTarget, colliderTarget);
        }
        EdgeCollider2D edgeCollider = FindEdgeCollider();

        List<Vector2> newPoints = new List<Vector2>();
        for (int i = 0; i < lr.positionCount; i++)
        {
            newPoints.Add(lr.GetPosition(i));
        }
        for (int i = lr.positionCount - 1; i >= 0; i--)
        {
            newPoints.Add(lr.GetPosition(i));
        }

        Undo.RecordObject(edgeCollider, "Changed edga radius and all points of EdgeCollider2D");
        edgeCollider.SetPoints(newPoints);
        edgeCollider.edgeRadius = lr.startWidth / 2f;
    }
    public void AdaptColliderAround(float distance, bool interior = true, bool exterior = true)
    {
        // Initialize
        if (lr == null) return;
        if (colliderTarget == null) colliderTarget = new GameObject("Collider target");

        EdgeCollider2D edgeCollider = FindEdgeCollider();

        List<Vector2> newPoints = new List<Vector2>();

        // Interior
        if (interior)
        {
            for (int i = 0; i < lr.positionCount; i++)
            {
                Vector2 nextPoint = i + 1 < lr.positionCount ? lr.GetPosition(i + 1) : lr.GetPosition(i - 1);
                Vector2 nextDir = (nextPoint - (Vector2)lr.GetPosition(i)).normalized;

                newPoints.Add((Vector2)lr.GetPosition(i) + new Vector2(nextDir.y, -nextDir.x) * distance);

                Debug.DrawRay((Vector2)lr.GetPosition(i), new Vector2(nextDir.y, -nextDir.x) * distance, Color.magenta, 10f);
            }
        }

        // Exterior
        if (exterior)
        {
            for (int i = lr.positionCount - 1; i >= 0; i--)
            {
                Vector2 nextPoint = i - 1 >= 0 ? lr.GetPosition(i - 1) : lr.GetPosition(0);
                Vector2 nextDir = (nextPoint - (Vector2)lr.GetPosition(i)).normalized;

                newPoints.Add((Vector2)lr.GetPosition(i) - new Vector2(-nextDir.y, nextDir.x) * distance);

                Debug.DrawRay((Vector2)lr.GetPosition(i), -new Vector2(-nextDir.y, nextDir.x) * distance, Color.blue, 10f);

            }
        }

        // Set points and edge radius
        Undo.RecordObject(edgeCollider, "Changed edga radius and all points of EdgeCollider2D");
        edgeCollider.SetPoints(newPoints);
        edgeCollider.edgeRadius = lr.startWidth / 10f;
    }

    EdgeCollider2D FindEdgeCollider()
    {
        EdgeCollider2D result;
        if (colliderTarget.TryGetComponent<EdgeCollider2D>(out result)) return result;
        else return colliderTarget.AddComponent<EdgeCollider2D>();
    }
    #endregion
}

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

    public LineRenderer lr;
    public GameObject colliderTarget;

    float distance = 0.2f;
    bool debugDistance;

    private void OnGUI()
    {
        GUILayout.Label("First params");

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
                    if(colliderTarget != null) colliderTarget = null;
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

                Debug.DrawRay((Vector2)lr.GetPosition(i), new Vector2(nextDir.y, -nextDir.x) * distance, Color.magenta, 25f);
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

                Debug.DrawRay((Vector2)lr.GetPosition(i), -new Vector2(-nextDir.y, nextDir.x) * distance, Color.blue, 25f);

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
}

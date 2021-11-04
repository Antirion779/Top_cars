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

    private void OnGUI()
    {
        // Line renderer
        if (GUILayout.Button("Fix line renderer Z points"))
        {
            for (int i = 0; i < lr.positionCount; i++)
            {
                Vector3 newPos = lr.GetPosition(i);
                newPos.z = 0;
                lr.SetPosition(i, newPos);
            }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Line renderer");
            lr = (LineRenderer)EditorGUILayout.ObjectField(lr, typeof(LineRenderer), true);
        }

        // Collider target
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Collider target");
            colliderTarget = (GameObject)EditorGUILayout.ObjectField(colliderTarget, typeof(GameObject), true);
        }

        // Functions
        GUILayout.Space(10);

            // Fill function
        if (GUILayout.Button("Fill")) AdaptColliderToFill();
        GUILayout.Space(10);

            // Around function
        using (new GUILayout.HorizontalScope())
        {
            distance = EditorGUILayout.FloatField(distance);
            if (GUILayout.Button("Around")) AdaptColliderAround(distance);
        }
        GUILayout.Space(5);

            // Interior border only function
        using (new GUILayout.HorizontalScope())
        {
            distance = EditorGUILayout.FloatField(distance);
            if (GUILayout.Button("Inside border")) AdaptColliderAround(distance, true, false);
        }
        GUILayout.Space(5);

            // Exterior border only function
        using (new GUILayout.HorizontalScope())
        {
            distance = EditorGUILayout.FloatField(distance);
            if (GUILayout.Button("Exterior border")) AdaptColliderAround(distance, false, true);
        }
    }

    public void AdaptColliderToFill()
    {
        if (lr == null) return;
        if (colliderTarget == null) colliderTarget = new GameObject("Collider target");
        EdgeCollider2D edgeCollider = colliderTarget.AddComponent<EdgeCollider2D>();

        List<Vector2> newPoints = new List<Vector2>();
        for (int i = 0; i < lr.positionCount; i++)
        {
            newPoints.Add(lr.GetPosition(i));
        }
        for (int i = lr.positionCount - 1; i >= 0; i--)
        {
            newPoints.Add(lr.GetPosition(i));
        }
        edgeCollider.SetPoints(newPoints);

        edgeCollider.edgeRadius = lr.startWidth / 2f;
    }
    public void AdaptColliderAround(float distance, bool interior = true, bool exterior = true)
    {
        // Initialize
        if (lr == null) return;
        if (colliderTarget == null) colliderTarget = new GameObject("Collider target");
        EdgeCollider2D edgeCollider = colliderTarget.AddComponent<EdgeCollider2D>();

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
        edgeCollider.SetPoints(newPoints);
        edgeCollider.edgeRadius = lr.startWidth / 10f;
    }
}

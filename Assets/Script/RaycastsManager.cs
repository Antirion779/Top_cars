using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastsManager : MonoBehaviour
{
    [SerializeField]
    RaycastData[] raycasts;
    [SerializeField] LayerMask playerLayer;
    enum Terrain { Track, Grass, Water };

    RaycastResult[] raycastResults;

    // Funcs for the AI
    public RaycastResult[] Results => raycastResults;
    public void SetUpRaycasts(RaycastData[] _raycastDatas)
    {
        raycasts = _raycastDatas;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D currentTerrainCollider = Physics2D.OverlapCircle(transform.position, 0.1f, ~playerLayer);
        Terrain currentTerrain = TagToTerrain(currentTerrainCollider.tag);
        Debug.Log($"{currentTerrainCollider.name} --> {currentTerrain}");

        // Collect each raycast result
        for (int i = 0; i < raycasts.Length; i++)
        {
            RaycastData raycast = raycasts[i];
            Terrain resultTerrain = currentTerrain;
            float distance = 0f;

            // Shoot raycast and select terrain touched
            float offset = Mathf.PI / 2f + transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 raycastDir = new Vector2(Mathf.Cos(raycast.radAngle + offset), Mathf.Sin(raycast.radAngle + offset)).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, raycastDir, raycast.length);
            foreach (RaycastHit2D hit in hits)
            {
                Terrain tempTerrain = TagToTerrain(hit.collider.tag);
                //Debug.Log($"{tempTerrain} != {resultTerrain}");
                if (tempTerrain != resultTerrain)
                {
                    resultTerrain = tempTerrain;
                    distance = Vector2.Distance(transform.position, hit.point);

                    if (resultTerrain == Terrain.Water) break;
                }
            }

            // Debug rays
            if (Debug.isDebugBuild)
            {
                Color debugRayColor = resultTerrain switch
                {
                    Terrain.Water => Color.red,
                    Terrain.Grass => Color.yellow,
                    Terrain.Track => Color.green,
                    _ => throw new System.NotImplementedException()
                };
                Debug.DrawRay(transform.position, raycastDir * raycast.length, debugRayColor);
            }


            RaycastResult result;
            result.terrainTouched = (int)resultTerrain;
            result.distance = distance;
            Debug.Log($"Raycast {i} {result}");

            raycastResults[i] = result;
        }
    }


    Terrain TagToTerrain(string tag)
    {
        switch (tag)
        {
            case "Grass":
                return Terrain.Grass;
            case "Water":
                return Terrain.Water;
        }
        return 0;
    }

    [System.Serializable]
    public struct RaycastData
    {
        public float radAngle;
        public float length;

    }
    public struct RaycastResult
    {
        public int terrainTouched;
        public float distance;
        public override string ToString()
        {
            return $"result = {{terrainTouched = ({terrainTouched}){(Terrain)terrainTouched}; distance = {distance} }}";
        }
    }
}

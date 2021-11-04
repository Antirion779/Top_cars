using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceProgres : MonoBehaviour
{
    [Header("Comonent")] 
    [SerializeField] private Slider barProgression;
    [SerializeField] private GameObject currentCar;
    [SerializeField] private GameObject[] waypoint;

    [Header("Variable")]
    [SerializeField] private float objectif;
    // Start is called before the first frame update
    void Start()
    {
        barProgression.maxValue = waypoint.Length;
        objectif = GetDistance(currentCar, waypoint[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCar.transform.position.y <= waypoint[0].transform.position.y && currentCar.transform.position.x <= waypoint[0].transform.position.x)
        {
        }

        float distance = 1 - (GetDistance(currentCar, waypoint[0]));
        Debug.Log(distance);
        SetProgress(distance);
    }

    private float GetDistance(GameObject a, GameObject b)
    {
        return Vector2.Distance(a.transform.position, b.transform.position);
    }

    void SetProgress(float p)
    {
        barProgression.value = p;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceProgres : MonoBehaviour
{
    [Header("Comonent")]
    [SerializeField] private GameObject currentCar;
    [SerializeField] private GameObject testSpawn;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject parentGate;

    [Header("Variable")] 
    [SerializeField] private int distBtwGate = 3;
    [SerializeField] private int numberOfGate;
    private float percentOfAvancement;
    private float valueOfOneGate;

    // Start is called before the first frame update
    void Start()
    {
        valueOfOneGate = 100 / numberOfGate;
        for (int i = 0; i < lr.positionCount -1; i += distBtwGate)//loop to spawn a Gate
        {
            if (lr.GetPosition(i + 1) != null)
            {
                GameObject gate = Instantiate(testSpawn, lr.GetPosition(i), Quaternion.identity);//spawn a gate on the point

                gate.transform.SetParent(parentGate.transform);
                gate.name = "gate_" + i;

                gate.GetComponent<Gate>().setDir(lr.GetPosition(i + 1));//rotate it face to the next point 
            }

            numberOfGate++;
        }
    }

    void EnterInAGate(string gateName)
    {
        Debug.Log("EHEHEHEHEHEHEHEE");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.name);
        if (col.tag == "Water")
        {
            Destroy(gameObject);
        }

        if (col.tag == "Gate")
        {
            EnterInAGate(col.name);
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceProgres : MonoBehaviour
{
    [Header("Comonent")]
    [SerializeField] private GameObject currentCar;
    [SerializeField] private GameObject testSpawn;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject parentGate;
    [SerializeField] private Text avancementText;

    [Header("Variable")] 
    [SerializeField] private int distBtwGate = 3;
    [SerializeField] private int numberOfGate;
    [SerializeField] private GameObject[] allGate;
    private float percentOfAvancement;
    private float valueOfOneGate;
    private float lastGatePassed = 0;
    private int lastGate = 0;
    private int gateObjectif;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < lr.positionCount -1; i += distBtwGate)//loop to spawn a Gate
        {
            if (lr.GetPosition(i + 1) != null)
            {
                GameObject gate = Instantiate(testSpawn, lr.GetPosition(i), Quaternion.identity);//spawn a gate on the point

                allGate[i/3] = gate;

                gate.transform.SetParent(parentGate.transform);//place it in the hiearchy + rename it
                gate.name = i.ToString();

                gate.GetComponent<Gate>().setDir(lr.GetPosition(i + 1));//rotate it face to the next point 
            }

            numberOfGate++;
        }
        valueOfOneGate = 100 / numberOfGate;//take a value of one gate
        Debug.Log(allGate[allGate.Length - 1]);
    }

    void EnterInAGate(string gateName)
    {
        //Debug.Log("curent gate passed : " + gateName);
        if (gateName == gateObjectif.ToString())//test if the gate we reach is the next one in the list
        {
            Debug.Log("last Objectif : " + gateObjectif + " / gate passed : " + gateName );
            if (gateName == 42.ToString())
            {
                gateObjectif = 0;
                Debug.Log("MAX GATE");
            }
            else
            {
                gateObjectif += distBtwGate;
                Debug.Log("Objectif: " + gateObjectif );
            }
            Debug.Log("Next Objectif : " + gateObjectif);

            percentOfAvancement += valueOfOneGate;
            avancementText.text = percentOfAvancement.ToString() + "%";

        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
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

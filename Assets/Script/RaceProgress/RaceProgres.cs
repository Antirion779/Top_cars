using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceProgres : MonoBehaviour
{

    /*
     * suite de waypoint entre tout les point de la ligne renderer chaques points tir un trait sur son axe local x dés que tu passe au travers --> %up + tu setup le suivant comme objectif
     * 
     */


    [Header("Comonent")]
    [SerializeField] private GameObject currentCar;
    [SerializeField] private GameObject testSpawn;
    [SerializeField] private LineRenderer lr;

    [Header("Variable")]
    [SerializeField] private float objectif;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < lr.positionCount; i+=3)
        {

            GameObject gate = Instantiate(testSpawn, lr.GetPosition(i), Quaternion.identity);
            gate.name = "gate_" + i;
            gate.GetComponent<Gate>().setDir(lr.GetPosition(i+1));
            Debug.Log("Le point numéro : " + i + " est à la position : " +lr.GetPosition(i));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}

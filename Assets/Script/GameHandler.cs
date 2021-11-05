using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [Header("Time scale")]
    [SerializeField] float simulationTimeScale = 1.0f;

    [Header("Simulation")]
    [SerializeField] int numberOfSimulation = 10;
    int currentSimulation;

    [SerializeField] float simulationDuration = 60f;
    float endOfSimulationTime;

    [Header("AI")]
    [SerializeField] int AIcount = 10;
    [SerializeField] GameObject aiPrefab;
    [SerializeField] Transform aiSpawn;
    GameObject[] AIs;

    [Header("UI"), SerializeField] GameObject onStartUI; 
    [SerializeField] GameObject endOfSimulationUI;

    [Header("Simulation UI"), SerializeField] GameObject simulationUI;
    [SerializeField] TMPro.TMP_Text simulationCounterDisplayer;
    [SerializeField] TMPro.TMP_Text currentTimerDisplayer;

    private void Awake()
    {
        onStartUI.SetActive(true);
        simulationUI.SetActive(false);
        endOfSimulationUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentSimulation > 0)
        {
            if (endOfSimulationTime <= Time.time)
            {
                if (currentSimulation < numberOfSimulation) RestartSimulation();
                else StopSimulation();
            }

            simulationCounterDisplayer.text = "Simulation: " + currentSimulation;
            currentTimerDisplayer.text = ((int)(endOfSimulationTime - Time.time)).ToString();
        }
    }

    void SpawnAIs()
    {
        AIs = new GameObject[AIcount];
        for (int i = 0; i < AIcount; i++)
        {
            AIs[i] = Instantiate(aiPrefab, aiSpawn.position, Quaternion.identity);
        }
    }

    public void StartSimulation() // Meant to be called by UI button
    {
        SpawnAIs();

        Time.timeScale = simulationTimeScale;

        simulationUI.gameObject.SetActive(true);
        onStartUI.gameObject.SetActive(false);

        RestartSimulation();
    }

    private void RestartSimulation()
    {
        // Pass to next simulation
        currentSimulation++;
        endOfSimulationTime = Time.time + simulationDuration;

        // Reset AIs
        foreach (GameObject ai in AIs)
        {
            // Reset AI ??

            // Tp
            ai.transform.position = aiSpawn.position;
        }
    }

    void StopSimulation()
    {
        Time.timeScale = 0;

        simulationUI.gameObject.SetActive(false);
        endOfSimulationUI.gameObject.SetActive(true);
    }
}

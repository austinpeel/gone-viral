using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    TextMeshProUGUI susceptibleTMP;
    TextMeshProUGUI infectedTMP;
    TextMeshProUGUI removedTMP;

    SimulationController sim;
    PersonStatus personStatus;

    int currentSusceptibleCount;
    int currentInfectedCount;
    int currentRemovedCount;

    private void Start()
    {
        susceptibleTMP = transform.Find("Susceptible TMP").GetComponent<TextMeshProUGUI>();
        infectedTMP = transform.Find("Infected TMP").GetComponent<TextMeshProUGUI>();
        removedTMP = transform.Find("Removed TMP").GetComponent<TextMeshProUGUI>();

        sim = GameObject.Find("Interaction Zone").GetComponent<SimulationController>();
        currentInfectedCount = sim.infectedIndices.Count;
        currentRemovedCount = 0;
        currentSusceptibleCount = sim.N - currentInfectedCount - currentRemovedCount;

        personStatus = sim.personPrefab.GetComponent<PersonStatus>();
    }

    private void Update()
    {
        if (currentInfectedCount != sim.infectedIndices.Count)
        {
            UpdateTMPs();
        }
    }

    void UpdateTMPs()
    {
        // Get new counts
        currentInfectedCount = sim.infectedIndices.Count;
        currentRemovedCount = sim.removedIndices.Count;
        currentSusceptibleCount = sim.N - currentInfectedCount - currentRemovedCount;

        // Update text
        susceptibleTMP.text = "Susceptible\t <color=#" + ColorUtility.ToHtmlStringRGB(personStatus.susceptibleColor) + ">" + currentSusceptibleCount + "</color>";
        infectedTMP.text    = "Infected\t <color=#" + ColorUtility.ToHtmlStringRGB(personStatus.infectedColor)    + ">" + currentInfectedCount + "</color>";
        removedTMP.text     = "Removed\t <color=#" + ColorUtility.ToHtmlStringRGB(personStatus.removedColor)     + ">" + currentRemovedCount + "</color>";
    }
}

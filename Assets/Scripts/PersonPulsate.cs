using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonPulsate : MonoBehaviour
{
    Transform infectiousArea;
    SimulationController sim;  // TODO Make this more efficient so each person doesn't need an instance of SimulationController ?

    float maxScale;
    float minScale = 1;
    float conversionFactor = 3.18f;
    float frequency = 1.4f;
    bool isPulsating;

    private void Awake()
    {
        minScale = 1;
        infectiousArea = transform.GetChild(0);
        infectiousArea.GetComponent<SpriteRenderer>().color = GetComponent<PersonStatus>().infectedColor - new Color(0, 0, 0, 0.8f);
        infectiousArea.localScale = new Vector3(minScale, minScale, 1);
        infectiousArea.gameObject.SetActive(false);
    }

    private void Start()
    {
        sim = transform.parent.parent.GetComponent<SimulationController>();
        maxScale = sim.infectionRadius;
    }

    private void Update()
    {
        // Susceptible --> Infected
        if (!isPulsating && transform.CompareTag("Infected"))
        {
            isPulsating = true;
            infectiousArea.gameObject.SetActive(true);
        }

        // Infected --> Removed
        if (isPulsating && transform.CompareTag("Removed"))
        {
            isPulsating = false;
            infectiousArea.gameObject.SetActive(false);
        }

        // Pulsate
        if (isPulsating && !sim.isPaused)
        {
            maxScale = sim.infectionRadius;

            float currentScale = minScale + ((conversionFactor * maxScale - minScale) / frequency) * (sim.clockTime % frequency);
            infectiousArea.localScale = new Vector3(currentScale, currentScale, 1);
        }

        //if (sim.isPaused)
        //{
        //    if (maxScale != sim.infectionRadius)
        //    {
        //        maxScale = sim.infectionRadius;
        //        if (infectiousArea.localScale.x > maxScale)
        //        {
        //            infectiousArea.localScale = new Vector3(maxScale, maxScale, 1);
        //        }
        //    }
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public TextMeshProUGUI displayTMP;
    Slider slider;

    bool isInfectionRateSlider;

    private void Start()
    {
        slider = transform.GetComponent<Slider>();
        if (slider.name == "Infection Rate Slider")
        {
            isInfectionRateSlider = true;
        }
    }

    private void Update()
    {
        if (displayTMP)
        {
            if (isInfectionRateSlider)
            {
                displayTMP.text = slider.value.ToString("F2");
            }
            else
            {
                displayTMP.text = slider.value.ToString("F1");
            }
            
        }
    }
}

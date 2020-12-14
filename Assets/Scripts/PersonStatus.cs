using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonStatus : MonoBehaviour
{
    public Color susceptibleColor = new Color(100 / 255f, 178 / 255f, 250 / 255f);
    public Color infectedColor = new Color(253 / 255f, 102 / 255f, 102 / 255f);
    public Color removedColor = new Color(230 / 255f, 230 / 255f, 230 / 255f);

    SpriteRenderer sr;

    private void Awake()
    {
        sr = transform.GetComponent<SpriteRenderer>();
    }

    public void SetStatus(string status)
    {
        if (status == "susceptible")
        {
            sr.color = susceptibleColor;
            sr.sortingLayerName = "Susceptible";
        }
        else if (status == "infected")
        {
            sr.color = infectedColor;
            sr.sortingLayerName = "Infected";
        }
        else if (status == "removed")
        {
            sr.color = removedColor;
            sr.sortingLayerName = "Default";
        }
    }
}

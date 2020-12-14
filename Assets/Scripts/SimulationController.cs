using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationController : MonoBehaviour
{
    // Coordinate bounds of the interaction zone
    public float[] bounds = new float[] { -10f, 10f, -10f, 10f };
    // Number of people in the interaction zone
    public int N = 200;
    // Linear 'walking' speed of each person
    [HideInInspector]
    public float moveSpeed = 1f;
    // Radius within which an infected person can infect susceptible people
    [HideInInspector]
    public float infectionRadius = 1.2f;
    // Probability of getting infected during each frame of contact with an infected person
    [HideInInspector]
    public float infectionRate = 0.2f;
    // How long a person remains infected (in seconds)
    [HideInInspector]
    public float infectionTime = 5f;

    public GameObject personPrefab;

    // Container for all of the people instances
    Transform peopleContainer;

    // Ordered collection of all people in the interaction zone
    List<Transform> people;
    // Normalized vector directions (2D) of each person
    List<Vector2> directions;

    // Lists to keep track of who has been removed or infected and for how long
    [HideInInspector]
    public List<int> infectedIndices;
    List<float> infectedTimes;
    [HideInInspector]
    public List<int> removedIndices;

    // Play controls
    [HideInInspector]
    public bool isPaused = true;
    [HideInInspector]
    public float clockTime;
    TextMeshProUGUI playButtonTMP;
    Slider infectionRadiusSlider;
    Slider infectionRateSlider;
    Slider infectionTimeSlider;
    Slider moveSpeedSlider;

    private void Awake()
    {
        // Assign transform references
        Transform canvas = GameObject.Find("Canvas").transform;
        playButtonTMP = canvas.Find("Play Button").Find("Play Button (TMP)").GetComponent<TextMeshProUGUI>();
        infectionRadiusSlider = canvas.Find("Infection Radius Slider").GetComponent<Slider>();
        infectionRateSlider = canvas.Find("Infection Rate Slider").GetComponent<Slider>();
        infectionTimeSlider = canvas.Find("Infection Time Slider").GetComponent<Slider>();
        moveSpeedSlider = canvas.Find("Move Speed Slider").GetComponent<Slider>();

        peopleContainer = GameObject.Find("People").transform;
    }

    private void Start()
    {
        // Set slider initial values
        infectionRadiusSlider.value = infectionRadius;
        infectionRateSlider.value = infectionRate;
        infectionTimeSlider.value = infectionTime;
        moveSpeedSlider.value = moveSpeed;

        ResetSimulation();
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        // Only run if not paused
        clockTime += Time.deltaTime;
        MovePeople();
        InfectNewPeople();
        RemovePeople();
    }

    // Called from each slider's OnValueChanged() in the Inspector
    public void UpdateParameters()
    {
        infectionRadius = infectionRadiusSlider.value;
        infectionRate = infectionRateSlider.value;
        infectionTime = infectionTimeSlider.value;
        moveSpeed = moveSpeedSlider.value;
        //print("Updating all params !");
    }

    private void MovePeople()
    {
        // Loop over all people
        for (int i = 0; i < N; i++)
        {
            Transform person = people[i];
            Vector2 direction = directions[i];
            Vector2 delta = moveSpeed * direction * Time.deltaTime;

            // Check that displacement will not end up outside the interaction zone
            // If it will, bounce off the wall instead
            if (person.position.x + delta.x <= bounds[0] || person.position.x + delta.x >= bounds[1])
            {
                delta.x = -delta.x;
                directions[i] = new Vector2(-direction.x, direction.y);
            }
            if (person.position.y + delta.y <= bounds[2] || person.position.y + delta.y >= bounds[3])
            {
                delta.y = -delta.y;
                directions[i] = new Vector2(direction.x, -direction.y);
            }

            person.Translate(delta.x, delta.y, 0);
        }
    }

    private void InfectNewPeople()
    {
        // Prepare a temporary list of newly infected people indices
        List<int> indicesToAdd = new List<int>();

        // Loop only over currently infected people
        foreach (int index in infectedIndices)
        {
            Transform infectedPerson = people[index];

            // Determine who is close enough to possibly get infected
            for (int i = 0; i < N; i++)
            {
                // Don't compare an infected person to him/herself or to other infected/removed people
                if (index == i || infectedIndices.Contains(i) || removedIndices.Contains(i))
                {
                    continue;
                }

                Transform otherPerson = people[i];
                float distance = Vector3.Distance(infectedPerson.position, otherPerson.position);

                // Only susceptible people can be infected
                if (distance <= infectionRadius)
                {
                    // Decide if other person should get infected
                    // TODO Calculate this more properly
                    int randomDraw = (infectionRate == 1) ? 0 : Random.Range(0, (int)Mathf.Round(1 / (infectionRate * Time.deltaTime)));
                    if (randomDraw == 0)
                    {
                        // The same susceptible person could being currently infected by multiple people
                        // Only count this person once
                        if (!indicesToAdd.Contains(i))
                        {
                            otherPerson.tag = "Infected";
                            otherPerson.GetComponent<PersonStatus>().SetStatus("infected");
                            indicesToAdd.Add(i);
                        }
                    }
                }
            }
        }

        // Add newly infected people to the global list
        foreach (int index in indicesToAdd)
        {
            infectedIndices.Add(index);
            infectedTimes.Add(0f);
        }
    }

    private void RemovePeople()
    {
        // Index within the infectedIndices list, not the unique person ID
        List<int> indicesToRemove = new List<int>();

        // Loop only over currently infected people
        for (int i = 0; i < infectedIndices.Count; i++)
        {
            infectedTimes[i] += Time.deltaTime;

            if (infectedTimes[i] > infectionTime)
            {
                // Flag this person to be removed
                indicesToRemove.Add(i);
            }
        }

        // Remove people
        // TODO An exception is thrown if the user lowers infectionTime too quickly to zero
        for (int i = 0; i < indicesToRemove.Count; i++)
        {
            try
            {
                // Update status
                int personID = infectedIndices[i];
                //print("Removing " + personID + " at index " + i);

                Transform infectedPerson = people[personID];
                infectedPerson.tag = "Removed";
                infectedPerson.GetComponent<PersonStatus>().SetStatus("removed");

                // Update lists
                removedIndices.Add(personID);
                infectedIndices.RemoveAt(i);
                infectedTimes.RemoveAt(i);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    public void TogglePlayPause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            playButtonTMP.text = "Play";
        }
        else
        {
            playButtonTMP.text = "Pause";
        }
    }

    public void ResetSimulation()
    {
        // Clear all existing people
        foreach (Transform person in peopleContainer)
        {
            Destroy(person.gameObject);
        }

        // Create new lists to hold the position/velocity of each person
        people = new List<Transform>();
        directions = new List<Vector2>();

        // Lists of unique IDs for currently infected and removed people
        infectedIndices = new List<int>();
        infectedTimes = new List<float>();
        removedIndices = new List<int>();

        if (!personPrefab)
        {
            print("No person prefab !");
            N = 0;
            return;
        }

        // Distribute N people with random initial positions and walking directions
        for (int i = 0; i < N; i++)
        {
            // Instantiate a person
            float x = Random.Range(bounds[0], bounds[1]);
            float y = Random.Range(bounds[2], bounds[3]);
            Vector3 startPosition = new Vector3(x, y, 0);
            GameObject person = Instantiate(personPrefab, startPosition, Quaternion.Euler(Vector3.zero), peopleContainer);

            // Set this person's status
            person.tag = "Susceptible";
            person.GetComponent<PersonStatus>().SetStatus("susceptible");

            // Add this person to the global list of people transforms
            people.Add(person.transform);

            // Initialize this person's velocity with a random direction
            float angle = Random.Range(0, 2 * Mathf.PI);
            directions.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
        }

        // Pick a random person to start as infected
        int index = Random.Range(0, N);
        people[index].tag = "Infected";
        people[index].GetComponent<PersonStatus>().SetStatus("infected");
        infectedIndices.Add(index);
        infectedTimes.Add(0f);

        // Pause if not already
        if (!isPaused)
        {
            isPaused = true;
            playButtonTMP.text = "Play";
        }
    }
}

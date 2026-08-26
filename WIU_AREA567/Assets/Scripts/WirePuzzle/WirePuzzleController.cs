using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleController : MonoBehaviour
{
    [Header("Wire Connections")]
    [SerializeField]
    private WireConnectionUI[] connections;

    [Header("Right Endpoints")]
    [SerializeField]
    private RectTransform[] rightEndpoints;

    [Header("Events")]
    public UnityEvent OnPuzzleCompleted;
    public UnityEvent OnPuzzleOpened;
    public UnityEvent OnPuzzleClosed;

    private int completedConnections;
    private bool isCompleted;

    // The specific door currently using this puzzle.
    private WirePuzzleDoor activeDoor;

    private Vector2[] originalEndpointPositions;

    public bool IsCompleted => isCompleted;

    private void Awake()
    {
        SaveEndpointPositions();
    }

    public void OpenForDoor(WirePuzzleDoor door)
    {
        if (door == null)
            return;

        activeDoor = door;

        ResetPuzzle();
        RandomiseEndpoints();

        gameObject.SetActive(true);

        OnPuzzleOpened?.Invoke();

        Debug.Log(
            "Wire puzzle opened for: " +
            door.gameObject.name
        );
    }

    public void RegisterConnection()
    {
        if (isCompleted)
            return;

        completedConnections++;

        Debug.Log(
            "Wire connected: " +
            completedConnections +
            "/" +
            connections.Length
        );

        if (completedConnections >=
            connections.Length)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        if (isCompleted)
            return;

        isCompleted = true;

        Debug.Log("WIRE PUZZLE COMPLETED!");

        OnPuzzleCompleted?.Invoke();

        if (activeDoor != null)
        {
            activeDoor.UnlockDoor();
        }

        activeDoor = null;
        OnPuzzleClosed?.Invoke();
        gameObject.SetActive(false);
    }

    public void ResetPuzzle()
    {
        completedConnections = 0;
        isCompleted = false;

        foreach (WireConnectionUI connection
                 in connections)
        {
            if (connection != null)
            {
                connection.ResetConnection();
            }
        }
    }

    public void ClosePuzzle()
    {
        if (!isCompleted)
        {
            ResetPuzzle();
        }

        activeDoor = null;

        OnPuzzleClosed?.Invoke();

        gameObject.SetActive(false);
    }

    private void SaveEndpointPositions()
    {
        if (rightEndpoints == null)
            return;

        originalEndpointPositions =
            new Vector2[
                rightEndpoints.Length
            ];

        for (int i = 0;
             i < rightEndpoints.Length;
             i++)
        {
            if (rightEndpoints[i] != null)
            {
                originalEndpointPositions[i] =
                    rightEndpoints[i]
                        .anchoredPosition;
            }
        }
    }

    private void RandomiseEndpoints()
    {
        if (rightEndpoints == null ||
            originalEndpointPositions == null)
        {
            return;
        }

        Vector2[] shuffledPositions =
            (Vector2[])
            originalEndpointPositions.Clone();

        for (int i =
             shuffledPositions.Length - 1;
             i > 0;
             i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            Vector2 temp =
                shuffledPositions[i];

            shuffledPositions[i] =
                shuffledPositions[randomIndex];

            shuffledPositions[randomIndex] =
                temp;
        }

        for (int i = 0;
             i < rightEndpoints.Length;
             i++)
        {
            if (rightEndpoints[i] != null)
            {
                rightEndpoints[i]
                    .anchoredPosition =
                    shuffledPositions[i];
            }
        }
    }
}
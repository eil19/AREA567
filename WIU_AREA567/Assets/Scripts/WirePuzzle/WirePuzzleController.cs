using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleController : MonoBehaviour
{
    [Header("Wire Connections")]
    [SerializeField]
    private WireConnectionUI[] connections;

    [Header("Events")]
    public UnityEvent OnPuzzleCompleted;

    private int completedConnections;
    private bool isCompleted;

    public bool IsCompleted => isCompleted;

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

        Debug.Log(
            "WIRE PUZZLE COMPLETED!"
        );

        OnPuzzleCompleted?.Invoke();
    }

    public void ResetPuzzle()
    {
        if (isCompleted)
            return;

        completedConnections = 0;

        foreach (WireConnectionUI connection
                 in connections)
        {
            if (connection != null)
            {
                connection.ResetConnection();
            }
        }

        Debug.Log(
            "Wire puzzle reset."
        );
    }

    public void ClosePuzzle()
    {
        if (!isCompleted)
        {
            ResetPuzzle();
        }

        gameObject.SetActive(false);
    }
}
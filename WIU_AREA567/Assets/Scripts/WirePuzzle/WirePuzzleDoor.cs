using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleDoor : MonoBehaviour,
    IInteractable
{
    [Header("Puzzle")]
    [SerializeField]
    private GameObject puzzlePanel;

    [Header("Events")]
    public UnityEvent OnDoorUnlocked;

    private bool unlocked;

    public bool IsUnlocked => unlocked;

    public void Interact(
        GameObject interactor)
    {
        if (unlocked)
        {
            OpenDoor();
            return;
        }

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
        }

        Debug.Log(
            "Door locked. Opening wire puzzle."
        );
    }

    public void UnlockDoor()
    {
        if (unlocked)
            return;

        unlocked = true;

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        Debug.Log(
            "DOOR UNLOCKED!"
        );

        OnDoorUnlocked?.Invoke();

        OpenDoor();
    }

    private void OpenDoor()
    {
        Debug.Log(
            "Door opened successfully."
        );

        // Later:
        // animator.SetTrigger("Open");
        // blockingCollider.enabled = false;
    }
}
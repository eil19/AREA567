using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleDoor :
    MonoBehaviour,
    IInteractable
{
    [Header("Puzzle")]
    [SerializeField]
    private WirePuzzleController puzzleController;

    [Header("Door")]
    [SerializeField]
    private Collider2D blockingCollider;

    [SerializeField]
    private Animator animator;

    [Header("Events")]
    public UnityEvent OnDoorUnlocked;
    public UnityEvent OnDoorOpened;

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

        if (puzzleController == null)
        {
            puzzleController =
                FindFirstObjectByType<
                    WirePuzzleController>();
        }

        if (puzzleController == null)
        {
            Debug.LogWarning(
                "WirePuzzleController " +
                "could not be found."
            );

            return;
        }

        puzzleController.OpenForDoor(this);
    }

    public void UnlockDoor()
    {
        if (unlocked)
            return;

        unlocked = true;

        Debug.Log(
            gameObject.name +
            " unlocked."
        );

        OnDoorUnlocked?.Invoke();

        OpenDoor();
    }

    private void OpenDoor()
    {
        if (blockingCollider != null)
        {
            blockingCollider.enabled =
                false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        Debug.Log(
            gameObject.name +
            " opened."
        );

        OnDoorOpened?.Invoke();
    }
}
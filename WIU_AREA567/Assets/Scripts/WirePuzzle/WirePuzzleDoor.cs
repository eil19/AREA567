using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleDoor : MonoBehaviour,
    IInteractable
{
    [Header("Puzzle")]
    [SerializeField]
    private WirePuzzleController puzzleController;

    [Header("Door Collision")]
    [SerializeField]
    private Collider2D blockingCollider;

    [Header("Door Visuals")]
    [SerializeField]
    private GameObject lockedVisual;

    [SerializeField]
    private GameObject unlockedLeftVisual;

    [SerializeField]
    private GameObject unlockedRightVisual;

    [Header("Persistence")]
    [SerializeField]
    private string doorID;

    [Header("Events")]
    public UnityEvent OnDoorUnlocked;
    public UnityEvent OnDoorOpened;

    private bool unlocked;

    public bool IsUnlocked => unlocked;
    public string DoorID => doorID;

    private void Start()
    {
        if (DoorRunData.IsDoorUnlocked(doorID))
        {
            RestoreUnlockedState();
        }
        else
        {
            ApplyLockedVisual();
        }
    }

    public void Interact(GameObject interactor)
    {
        // Door is already open.
        if (unlocked)
        {
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
                "WirePuzzleController could not be found."
            );

            return;
        }

        puzzleController.OpenForDoor(this);
    }

    public void UnlockDoor()
    {
        if (unlocked)
        {
            return;
        }

        unlocked = true;

        // Save unlocked state for this run.
        DoorRunData.UnlockDoor(doorID);

        ApplyUnlockedVisual();

        OnDoorUnlocked?.Invoke();
        OnDoorOpened?.Invoke();

        Debug.Log(
            gameObject.name +
            " unlocked."
        );
    }

    private void ApplyLockedVisual()
    {
        unlocked = false;

        if (lockedVisual != null)
        {
            lockedVisual.SetActive(true);
        }

        if (unlockedLeftVisual != null)
        {
            unlockedLeftVisual.SetActive(false);
        }

        if (unlockedRightVisual != null)
        {
            unlockedRightVisual.SetActive(false);
        }

        if (blockingCollider != null)
        {
            blockingCollider.enabled = true;
        }
    }

    private void ApplyUnlockedVisual()
    {
        if (lockedVisual != null)
        {
            lockedVisual.SetActive(false);
        }

        if (unlockedLeftVisual != null)
        {
            unlockedLeftVisual.SetActive(true);
        }

        if (unlockedRightVisual != null)
        {
            unlockedRightVisual.SetActive(true);
        }

        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }
    }

    private void RestoreUnlockedState()
    {
        unlocked = true;

        ApplyUnlockedVisual();
    }
}
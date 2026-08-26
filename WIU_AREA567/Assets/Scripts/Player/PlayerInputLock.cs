using UnityEngine;

public class PlayerInputLock : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (playerInteractor == null)
        {
            playerInteractor = GetComponent<PlayerInteractor>();
        }
    }

    // crafting, chest, wire puzzle, cutscenes
    public void LockAll()
    {
        if (playerController != null)
        {
            playerController.SetInputLocked(true);
        }
        if (playerInteractor != null)
        {
            playerInteractor.SetInteractionLocked(true);
        }
    }

    // dialogue still needs interact to advance dialogue
    public void LockMovementOnly()
    {
        if (playerController != null)
        {
            playerController.SetInputLocked(true);
        }

        if (playerInteractor != null)
        {
            playerInteractor.SetInteractionLocked(false);
        }
    }

    public void UnlockAll()
    {
        if (playerController != null)
        {
            playerController.SetInputLocked(false);
        }
        if (playerInteractor != null)
        {
            playerInteractor.SetInteractionLocked(false);
        }
    }
}
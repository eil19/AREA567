using UnityEngine;

// Scene-level bridge between the reusable Player prefab and the prompt
// canvases that exist only in this scene. Keeping these references here avoids
// placing scene-object references on Player.prefab.
public class PromptUIBindings : MonoBehaviour
{
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private FloatingPromptUI pickupPrompt;
    [SerializeField] private FloatingPromptUI interactPrompt;

    private void Awake()
    {
        if (playerInteractor == null)
            playerInteractor = FindFirstObjectByType<PlayerInteractor>();
    }

    private void OnEnable()
    {
        if (playerInteractor == null) return;

        playerInteractor.OnPickupFocused.AddListener(ShowPickupPrompt);
        playerInteractor.OnPickupLostFocus.AddListener(HidePickupPrompt);
        playerInteractor.OnInteractableFocused.AddListener(ShowInteractPrompt);
        playerInteractor.OnInteractableLostFocus.AddListener(HideInteractPrompt);
    }

    private void OnDisable()
    {
        if (playerInteractor == null) return;

        playerInteractor.OnPickupFocused.RemoveListener(ShowPickupPrompt);
        playerInteractor.OnPickupLostFocus.RemoveListener(HidePickupPrompt);
        playerInteractor.OnInteractableFocused.RemoveListener(ShowInteractPrompt);
        playerInteractor.OnInteractableLostFocus.RemoveListener(HideInteractPrompt);
    }

    private void ShowPickupPrompt(GameObject focusedObject) => pickupPrompt?.Show(focusedObject);
    private void HidePickupPrompt() => pickupPrompt?.Hide();
    private void ShowInteractPrompt(GameObject focusedObject) => interactPrompt?.Show(focusedObject);
    private void HideInteractPrompt() => interactPrompt?.Hide();
}

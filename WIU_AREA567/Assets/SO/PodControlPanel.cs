using System.Collections;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class PodControlPanel : MonoBehaviour, IInteractable
{
    Inventory inventory;

    [Header("Linked Alien")]
    [SerializeField] private AlienInstance linkedAlien;

    [Header("TEMP TEST")]
    [SerializeField] private ItemEffect testSplashPotionEffect;

    [Header("Required Item")]
    //[SerializeField] private ItemData splashPotionData;

    [Header("Potion count display")]
    [SerializeField] private ItemData splashPotionData;
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text potionCountText;
    [SerializeField] private TMP_Text errorText;

    [Header("Reaction Sequence")]
    [SerializeField] private CinemachineCamera closeUpCamera;
    [SerializeField] private string reactionTrigger = "SplashReact";
    [SerializeField] private float reactionFallbackDuration = 2f;

    [Header("Camera Delay Settings")]
    [SerializeField] private float cameraBlendDelay = 1.5f; 

    private PlayerInteractor playerInteractor;
    private bool isFocused;
    private bool isSequencePlaying;
    private GameObject currentInteractor;
    private Coroutine hideErrorCoroutine;


    [ContextMenu("TEST: Add Splash Potion to Inventory")]
    private void TestAddSplashPotion()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[PodControlPanel] No Inventory.Instance in scene.");
            return;
        }
        if (splashPotionData == null)
        {
            Debug.LogWarning("[PodControlPanel] splashPotionData not assigned.");
            return;
        }

        var testItem = new ItemInstance(splashPotionData, testSplashPotionEffect, 1);
        inventory.AddItem(testItem);
        Debug.Log($"[PodControlPanel] Added test splash potion. Count now: {GetSplashPotionCount()}");
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerInteractor = playerObj.GetComponent<PlayerInteractor>();
        }
 
        if (playerInteractor != null)
        {
            playerInteractor.OnInteractableFocused.AddListener(HandleFocused);
            playerInteractor.OnInteractableLostFocus.AddListener(HandleLostFocus);
        }
 
        if (promptRoot != null) promptRoot.SetActive(false);


        if (inventory == null)
        {
            inventory = FindFirstObjectByType<Inventory>();
        }
    }
 
    private void OnDestroy()
    {
        if (playerInteractor != null)
        {
            playerInteractor.OnInteractableFocused.RemoveListener(HandleFocused);
            playerInteractor.OnInteractableLostFocus.RemoveListener(HandleLostFocus);
        }
    }
 
    private void Update()
    {
        // Block UpdatePrompt from running if sequence is playing
        if (isFocused && !isSequencePlaying) UpdatePrompt();
    }
 
    private void HandleFocused(GameObject focusedObject)
    {
        if (focusedObject != gameObject) return;
        isFocused = true;
        if (!isSequencePlaying) UpdatePrompt();
    }
 
    private void HandleLostFocus()
    {
        if (!isFocused) return;
        isFocused = false;
        if (promptRoot != null) promptRoot.SetActive(false);
        ClearErrorText(); // Hide error text if the player walks away
    }
 
    private void UpdatePrompt()
    {
        if (linkedAlien == null || linkedAlien.identified)
        {
            if (promptRoot != null) promptRoot.SetActive(false);
            return;
        }
 
        if (promptRoot != null) promptRoot.SetActive(true);
        if (potionCountText != null)
        {
            potionCountText.text = $"Splash Potions: {GetSplashPotionCount()}";
        }
    }
 
    private int GetSplashPotionCount()
    {
        if(inventory == null || splashPotionData == null) return 0;
        return inventory.GetItemQuantity(splashPotionData);
    }

    public bool CanInteract()
    {
        // Prevent interaction if no alien linked, alien is already identified, sequence is playing
        if (linkedAlien == null) return false;
        if (linkedAlien.identified) return false;
        if (isSequencePlaying) return false;

        return true;
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;

        AlienInteractionTarget.SetCurrent(linkedAlien);

        currentInteractor = interactor;

        int potionCount = GetSplashPotionCount();
        if (potionCount <= 0)
        {
            ShowError("No potions in inventory! You need a splash potion to start.", 3f);
            return;
        }

        ClearErrorText();
        StartSplashSequence();
    }

    private void ShowError(string message, float duration)
    {
        if (errorText == null) return;

        // Stop previous timer if one is already running
        if (hideErrorCoroutine != null)
        {
            StopCoroutine(hideErrorCoroutine);
        }

        errorText.text = message;
        errorText.gameObject.SetActive(true);

        // Auto-hide error after duration
        hideErrorCoroutine = StartCoroutine(HideErrorRoutine(duration));
    }

    private IEnumerator HideErrorRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearErrorText();
    }

    private void ClearErrorText()
    {
        if (hideErrorCoroutine != null)
        {
            StopCoroutine(hideErrorCoroutine);
            hideErrorCoroutine = null;
        }

        if (errorText != null)
        {
            errorText.text = "";
            errorText.gameObject.SetActive(false);
        }
    }
    private void StartSplashSequence()
    {
        isSequencePlaying = true;

        bool consumed = inventory.TryConsumeItem(splashPotionData, 1);
        if (!consumed)
        {
            isSequencePlaying = false;
            return;
        }

        if (promptRoot != null) promptRoot.SetActive(false); // hide "Press E" while sequence runs
        StartCoroutine(SplashSequenceRoutine());
    }

    private IEnumerator SplashSequenceRoutine()
    {
        //Switch Camera
        if (TESTCameraSwitch.Instance != null)
        {
            TESTCameraSwitch.Instance.SwitchToCloseUp(closeUpCamera);
        }

        //Wait for camera to fully transition into position
        yield return new WaitForSeconds(cameraBlendDelay);

        //Trigger reaction animation after camera is locked on
        linkedAlien.TriggerSplashReaction();

        //Wait for reaction animation duration
        yield return new WaitForSeconds(reactionFallbackDuration);

        //Open Guess UI
        if (AlienGuessUI.Instance != null)
        {
            AlienGuessUI.Instance.OnPanelClosed += HandleGuessPanelClosed;
            AlienGuessUI.Instance.Show(linkedAlien);
        }
        else
        {
            TESTCameraSwitch.Instance?.SwitchToTopDown();
            isSequencePlaying = false;
        }
    }

    private void HandleGuessPanelClosed()
    {
        errorText.gameObject.SetActive(false);
        errorText.text = "";
        AlienGuessUI.Instance.OnPanelClosed -= HandleGuessPanelClosed;
        TESTCameraSwitch.Instance?.SwitchToTopDown();
        isSequencePlaying = false;

        // Reset sequence flag
        isSequencePlaying = false;

        // Force focus state to false so the player MUST step away and return
        isFocused = false;

        // Hide the prompt UI immediately
        if (promptRoot != null) promptRoot.SetActive(false);
    }
}

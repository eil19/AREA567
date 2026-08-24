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

    [Header("Taming Items")]
    [SerializeField] private ItemData essenceData;
    [SerializeField] private ItemEffect essenceEffect;

    [Header("Experiment Items")]
    [SerializeField] private ItemData splashPotionData;
    [SerializeField] private ItemEffect splashPotionEffect;


    [Header("Experiment prompt")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text potionCountText;
    [SerializeField] private TMP_Text errorText;

    [Header("Taming Prompt")]
    [SerializeField] private GameObject tamePromptRoot;
    [SerializeField] private TMP_Text tamePromptText;
    [SerializeField] private TMP_Text tameResultText;
    [SerializeField] private float tameResultDisplayDuration = 1.5f;

    [Header("Reaction Sequence")]
    [SerializeField] private CinemachineCamera closeUpCamera;
    [SerializeField] private string reactionTrigger = "SplashReact";
    [SerializeField] private float reactionFallbackDuration = 2f;
    [SerializeField] private float cameraBlendDelay = 1.5f; 

    private PlayerInteractor playerInteractor;
    private bool isFocused;
    private bool isSequencePlaying;
    private GameObject currentInteractor;
    private Coroutine hideErrorCoroutine;
    private Coroutine hideTameResultCoroutine;


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

        var testItem = new ItemInstance(splashPotionData, splashPotionEffect, 1);
        inventory.AddItem(testItem);
        Debug.Log($"[PodControlPanel] Added test splash potion. Count now: {GetItemCount(splashPotionData)}");
    }

    [ContextMenu("TEST: Add Essence to Inventory")]
    private void TestAddEssence()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[PodControlPanel] No Inventory.Instance in scene.");
            return;
        }
        if (essenceData == null)
        {
            Debug.LogWarning("[PodControlPanel] essence not assigned.");
            return;
        }

        var testItem = new ItemInstance(essenceData, essenceEffect, 1);
        inventory.AddItem(testItem);
        Debug.Log($"[PodControlPanel] Added essence. Count now: {GetItemCount(splashPotionData)}");
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
        if (tamePromptRoot != null) tamePromptRoot.SetActive(false);

        ClearErrorText();
        ClearTameResultText();

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
        if (tamePromptRoot != null) tamePromptRoot.SetActive(false);
        ClearErrorText(); // Hide error text if the player walks away
    }
 
    private void UpdatePrompt()
    {
        if (linkedAlien == null)
        {
            if (promptRoot != null) promptRoot.SetActive(false);
            if (tamePromptRoot != null) tamePromptRoot.SetActive(false);
            return;
        }

        // Alien identified and not yet tamed -> show taming panel
        if (linkedAlien.identified && !linkedAlien.isTamed)
        {
            if (promptRoot != null) promptRoot.SetActive(false);
            if (tamePromptRoot != null) tamePromptRoot.SetActive(true);
            if (tamePromptText != null) tamePromptText.text = "PRESS \"E\" TO ATTEMPT TAMING\r\n1 ESSENCE REQUIRED PER ATTEMPT";
            return;
        }

        // Alien tamed already, or no valid state -> hide both
        if (linkedAlien.isTamed)
        {
            if (promptRoot != null) promptRoot.SetActive(false);
            if (tamePromptRoot != null) tamePromptRoot.SetActive(false);
            return;
        }

        // Not yet identified -> show potion prompt (original behavior)
        if (promptRoot != null) promptRoot.SetActive(true);
        if (tamePromptRoot != null) tamePromptRoot.SetActive(false);
        if (potionCountText != null)
        {
            potionCountText.text = $"Splash Potions: {GetItemCount(splashPotionData)}";
        }
    }

    private int GetItemCount(ItemData data)
    {
        if (inventory == null || data == null) return 0;
        return inventory.GetItemQuantity(data);
    }

    public void Interact(GameObject interactor)
    {
        if (linkedAlien == null)
            return;

        if (isSequencePlaying)
            return;

        if (linkedAlien.isTamed)
            return;

        currentInteractor = interactor;

        //experiment
        if (!linkedAlien.identified)
        {
            if (GetItemCount(splashPotionData) <= 0)
            {
                ShowError("No potions in inventory.", 3f);
                return;
            }

            ClearErrorText();
            StartSplashSequence();
        }
        //tame
        else if (!linkedAlien.isTamed)
        {
            if (GetItemCount(essenceData) <= 0)
            {
                ShowError("No essence in inventory.", 3f);
                return;
            }

            ClearErrorText();
            StartCoroutine(TameSequenceRoutine());
        }
    }
    private IEnumerator TameSequenceRoutine()
    {
        isSequencePlaying = true;
        if (tamePromptRoot != null) tamePromptRoot.SetActive(false);

        bool consumed = inventory.TryConsumeItem(essenceData, 1);
        if (!consumed)
        {
            isSequencePlaying = false;
            ShowError("No essence in inventory.", 3f);
            yield break;
        }

        //Switch Camera
        if (CameraSwitch.Instance != null)
        {
            CameraSwitch.Instance.SwitchToCloseUp(closeUpCamera);
        }

        //Wait for camera to fully transition into position
        yield return new WaitForSeconds(cameraBlendDelay);

        bool success = linkedAlien.AttemptTame();

        ShowTameResult(success ? "Tamed!" : "Taming failed, try again.", tameResultDisplayDuration);

        //Hold on the result before switching back
        yield return new WaitForSeconds(tameResultDisplayDuration);

        CameraSwitch.Instance?.SwitchToTopDown();

        isSequencePlaying = false;

        if (!success && isFocused)
        {
            // Player can try again — re-show the tame prompt if still focused
            UpdatePrompt();
        }
    }

    private void ShowTameResult(string message, float duration)
    {
        if (tameResultText == null) return;

        if (hideTameResultCoroutine != null)
        {
            StopCoroutine(hideTameResultCoroutine);
        }

        tameResultText.text = message;
        tameResultText.gameObject.SetActive(true);

        hideTameResultCoroutine = StartCoroutine(HideTameResultRoutine(duration));
    }

    private IEnumerator HideTameResultRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearTameResultText();
    }

    private void ClearTameResultText()
    {
        if (hideTameResultCoroutine != null)
        {
            StopCoroutine(hideTameResultCoroutine);
            hideTameResultCoroutine = null;
        }

        if (tameResultText != null)
        {
            tameResultText.text = "";
            tameResultText.gameObject.SetActive(false);
        }
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
        if (CameraSwitch.Instance != null)
        {
            CameraSwitch.Instance.SwitchToCloseUp(closeUpCamera);
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
            CameraSwitch.Instance?.SwitchToTopDown();
            isSequencePlaying = false;
        }
    }   

    private void HandleGuessPanelClosed()
    {
        AlienGuessUI.Instance.OnPanelClosed -= HandleGuessPanelClosed;
        CameraSwitch.Instance?.SwitchToTopDown();
        isSequencePlaying = false;

        // Force focus to reset so player MUST step away and re-enter trigger
        isFocused = false;

        if (promptRoot != null) promptRoot.SetActive(false);
        if (tamePromptRoot != null) tamePromptRoot.SetActive(false);
        ClearErrorText();

        if (linkedAlien != null)
        {
            AlienInteractionTarget.ClearIfCurrent(linkedAlien);
        }
    }
}

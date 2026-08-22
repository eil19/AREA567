using System.Collections;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class PodControlPanel : MonoBehaviour
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

    [Header("Reaction Sequence")]
    [SerializeField] private CinemachineCamera closeUpCamera;
    [SerializeField] private string reactionTrigger = "SplashReact";
    [SerializeField] private float reactionFallbackDuration = 2f;

    private PlayerInteractor playerInteractor;
    private bool isFocused;
    private bool isSequencePlaying;
    private GameObject currentInteractor;

    [ContextMenu("TEST: Add Splash Potion to Inventory")]
    private void TestAddSplashPotion()
    {
        if (Inventory.Instance == null)
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
        Inventory.Instance.AddItem(testItem);
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
        // Keep the count live in case the player picks up more potions while standing here
        if (isFocused) UpdatePrompt();
    }
 
    private void HandleFocused(GameObject focusedObject)
    {
        if (focusedObject != gameObject) return;
        isFocused = true;
        UpdatePrompt();
    }
 
    private void HandleLostFocus()
    {
        if (!isFocused) return;
        isFocused = false;
        if (promptRoot != null) promptRoot.SetActive(false);
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
        if (Inventory.Instance == null || splashPotionData == null) return 0;
 
        int total = 0;
        foreach (var item in Inventory.Instance.Items)
        {
            if (item != null && item.itemData == splashPotionData)
            {
                total += item.quantity;
            }
        }
        return total;
    }
 
    public void Interact(GameObject interactor)
    {
        if (linkedAlien == null) return;
        if (isSequencePlaying) return; // ignore E spam mid-sequence

        AlienInteractionTarget.SetCurrent(linkedAlien);
 
        if (linkedAlien.identified)
        {
            Debug.Log("[TubeControlPanel] This alien is already identified.");
            return;
        }

        if (GetSplashPotionCount() <= 0)
        {
            Debug.Log("[PodControlPanel] No splash potions available.");
            return;
        }

        currentInteractor = interactor;
        StartSplashSequence();
    }

    private void StartSplashSequence()
    {
        isSequencePlaying = true;

        // 1) consume the potion
        //Inventory.Instance.RemoveOneOfItem(splashPotionData);
        UpdatePrompt();

        // 2) swap to the close-up camera
        if (TESTCameraSwitch.Instance != null)
        {
            TESTCameraSwitch.Instance.SwitchToCloseUp(closeUpCamera);
        }

        // 4) apply the effect / mark identified
        if (testSplashPotionEffect != null)
        {
            testSplashPotionEffect.Use(currentInteractor);
        }
        else
        {
            linkedAlien.MarkIdentified();
        }

        linkedAlien.TriggerSplashReaction();

        // 5) show the guess UI, then switch the camera back once it closes
        if (AlienGuessUI.Instance != null)
        {
            AlienGuessUI.Instance.OnPanelClosed += HandleGuessPanelClosed;
        }
        else
        {
            TESTCameraSwitch.Instance?.SwitchToTopDown();
            isSequencePlaying = false;
        }
    }

    private void HandleGuessPanelClosed()
    {
        AlienGuessUI.Instance.OnPanelClosed -= HandleGuessPanelClosed;
        TESTCameraSwitch.Instance?.SwitchToTopDown();
        isSequencePlaying = false;
    }
}

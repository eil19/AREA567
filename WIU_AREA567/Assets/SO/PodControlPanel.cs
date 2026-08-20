using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

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

    private PlayerInteractor playerInteractor;
    private bool isFocused;

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
 
        AlienInteractionTarget.SetCurrent(linkedAlien);
 
        if (linkedAlien.identified)
        {
            Debug.Log("[TubeControlPanel] This alien is already identified.");
            return;
        }
 
        // TEMP direct call
        if (testSplashPotionEffect != null)
        {
            testSplashPotionEffect.Use(interactor);
        }
        else
        {
            Debug.LogWarning("[TubeControlPanel] No testSplashPotionEffect assigned — nothing will happen until Inventory is wired up.");
        }
    }
}

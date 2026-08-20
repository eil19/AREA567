using UnityEngine;

public class PodControlPanel : MonoBehaviour
{
    [Header("Linked Alien")]
    [SerializeField] private AlienInstance linkedAlien;

    private bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //private void Update()
    //{
    //    if (!playerInRange || linkedAlien == null) return;

    //    if (!linkedAlien.identified && Input.GetKeyDown(KeyCode.E) && PlayerInventory.HasItem(ItemType.SplashPotion))
    //    {
    //        UseSplashPotion();
    //    }

    //    if (linkedAlien.identified && !linkedAlien.tamingAttempted && Input.GetKeyDown(KeyCode.F) && PlayerInventory.HasItem(ItemType.BondingCharm))
    //    {
    //        UseBondingCharm();
    //    }
    //}

    //private void UseSplashPotion()
    //{
    //    PlayerInventory.ConsumeItem(ItemType.SplashPotion);

    //    var stateController = linkedAlien.GetComponent<StateController>();
    //    var action = ScriptableObject.CreateInstance<IdentifyAlienAction>();
    //    action.Act(stateController);
    //}

    //private void UseBondingCharm()
    //{
    //    PlayerInventory.ConsumeItem(ItemType.BondingCharm);

    //    var stateController = linkedAlien.GetComponent<StateController>();
    //    var action = ScriptableObject.CreateInstance<TryTameAction>();
    //    action.Act(stateController);
       
    //}
}

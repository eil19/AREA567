using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChestUI : MonoBehaviour
{
    [SerializeField] private ChestStorage chestStorage;
    [SerializeField] private Inventory inventory;

    [SerializeField]
    private ChestSlotUI[] chestSlots;

    [SerializeField] private Image dragIcon;

    public UnityEvent OnChestClosed;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<Inventory>();
        }

        InitialiseSlots();

        if (chestStorage != null)
        {
            chestStorage.OnStorageChanged.AddListener(Refresh);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (chestStorage != null)
        {
            chestStorage.OnStorageChanged.RemoveListener(Refresh);
        }
    }

    private void InitialiseSlots()
    {
        for (int i = 0;
             i < chestSlots.Length;
             i++)
        {
            chestSlots[i].Initialise(
                i,
                chestStorage,
                inventory,
                dragIcon
            );
        }
    }

    public void Refresh()
    {
        if (chestStorage == null)
            return;

        for (int i = 0;
             i < chestSlots.Length;
             i++)
        {
            chestSlots[i].UpdateSlot(
                chestStorage.GetItem(i)
            );
        }
    }

    public void CloseChest()
    {
        gameObject.SetActive(false);
        OnChestClosed?.Invoke();
    }
}
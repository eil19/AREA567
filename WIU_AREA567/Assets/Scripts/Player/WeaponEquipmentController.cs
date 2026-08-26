using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponEquipmentController :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController playerController;

    [Header("Events")]
    public UnityEvent<ItemData> OnWeaponEquipped;
    public UnityEvent OnWeaponUnequipped;

    private int equippedSlot = -1;

    private void Start()
    {
        if (inventory == null)
        {
            inventory =
                FindFirstObjectByType<Inventory>();
        }

        if (playerController == null)
        {
            playerController =
                GetComponent<PlayerController>();
        }

        Unequip();
    }

    private void Update()
    {
        if (playerController == null ||
            playerController.IsInputLocked)
        {
            return;
        }

        if (InputSystem.actions[
            "SelectWeapon1"].WasPressedThisFrame())
        {
            ToggleSlot(0);
        }

        if (InputSystem.actions[
            "SelectWeapon2"].WasPressedThisFrame())
        {
            ToggleSlot(1);
        }

        if (InputSystem.actions[
            "SelectWeapon3"].WasPressedThisFrame())
        {
            ToggleSlot(2);
        }
    }

    private void ToggleSlot(int slotIndex)
    {
        // Same key = unequip.
        if (equippedSlot == slotIndex)
        {
            Unequip();
            return;
        }

        ItemInstance item =
            inventory?.GetItem(slotIndex);

        if (item == null ||
            item.itemData == null ||
            item.itemData.itemType !=
                ItemType.Weapon)
        {
            return;
        }

        equippedSlot = slotIndex;

        playerController.SetEquippedWeapon(
            item.itemData.weaponType
        );

        OnWeaponEquipped?.Invoke(
            item.itemData
        );

        Debug.Log(
            "Equipped: " +
            item.itemData.itemName
        );
    }

    public void Unequip()
    {
        equippedSlot = -1;

        if (playerController != null)
        {
            playerController.SetEquippedWeapon(
                WeaponType.Unarmed
            );
        }

        OnWeaponUnequipped?.Invoke();
    }
}
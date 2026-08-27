using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedWeaponUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private WeaponEquipmentController weaponEquipment;

    private void Start()
    {
        if (weaponEquipment == null)
        {
            weaponEquipment = FindFirstObjectByType<WeaponEquipmentController>();
        }

        ShowUnarmed();

        if (weaponEquipment == null)
        {
            Debug.LogWarning("EquippedWeaponUI could not find WeaponEquipmentController.", this);
            return;
        }

        weaponEquipment.OnWeaponEquipped.AddListener(ShowWeapon);
        weaponEquipment.OnWeaponUnequipped.AddListener(ShowUnarmed);
    }

    private void OnDestroy()
    {
        if (weaponEquipment == null)
        {
            return;
        }

        weaponEquipment.OnWeaponEquipped.RemoveListener(ShowWeapon);
        weaponEquipment.OnWeaponUnequipped.RemoveListener(ShowUnarmed);
    }

    private void ShowWeapon(ItemData weapon)
    {
        if (weapon == null)
        {
            ShowUnarmed();
            return;
        }

        if (weaponText != null)
        {
            weaponText.text = "EQUIPPED: " + GetWeaponLabel(weapon.weaponType);
        }

        bool hasIcon = weaponIcon != null && weapon.itemImage != null;
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(hasIcon);
            weaponIcon.sprite = weapon.itemImage;
        }
    }

    private void ShowUnarmed()
    {
        if (weaponText != null)
        {
            weaponText.text = "EQUIPPED: UNARMED";
        }

        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(false);
        }
    }

    private static string GetWeaponLabel(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Melee => "MELEE",
            WeaponType.Ranged => "RANGED",
            WeaponType.Taser => "TASER",
            _ => "UNARMED"
        };
    }
}

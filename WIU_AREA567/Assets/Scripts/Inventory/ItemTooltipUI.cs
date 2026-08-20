using TMPro;
using UnityEngine;

public class ItemTooltipUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;

    private void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(ItemData itemData)
    {
        if (itemData == null) return;

        itemName.text = itemData.name;
        itemDescription.text = itemData.description;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}

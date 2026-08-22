using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingOutputUI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("References")]
    [SerializeField] private CraftingSystem craftingSystem;
    [SerializeField] private Image dragIcon;

    public CraftingRecipe CurrentRecipe =>
        craftingSystem != null ? craftingSystem.CurrentRecipe : null;

    public CraftingSystem CraftingSystem => craftingSystem;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CraftingRecipe recipe = CurrentRecipe;
        if (recipe == null || recipe.outputItem == null || dragIcon == null) return;

        dragIcon.sprite = recipe.outputItem.itemImage;
        dragIcon.gameObject.SetActive(true);
        dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CurrentRecipe == null || dragIcon == null) return;
        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
        }
    }
}
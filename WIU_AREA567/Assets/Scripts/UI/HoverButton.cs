using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HoverButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private Vector3 originalScale;

    [SerializeField] private float hoverScale = 1.5f; 

    [Header("Text content")]
    [SerializeField] private TextMeshProUGUI buttonText;

    public UnityEvent OnHoverEnter;

    private string originalText;
    private Coroutine arrowAnimation;
    private bool hovering;

    private void Start()
    {
        originalScale = transform.localScale;
        originalText = buttonText.text.Trim();
        ResetVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;

        // scale slightly bigger
        transform.localScale = originalScale * hoverScale;
        // bold
        buttonText.fontStyle = FontStyles.Bold;

        if (arrowAnimation != null)
        {
            StopCoroutine(arrowAnimation);
        }

        // show arrows
        arrowAnimation = StartCoroutine(AnimateArrows());
        OnHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisuals();
    }

    public void ResetVisuals()
    {
        hovering = false;

        // scale back to original size
        transform.localScale = originalScale;
        // un-bold
        buttonText.fontStyle = FontStyles.Normal;
        buttonText.ForceMeshUpdate();
        // hide arrows
        if (arrowAnimation != null)
        {
            StopCoroutine(arrowAnimation);
            arrowAnimation = null;
        }
        buttonText.text = originalText;
    }

    private IEnumerator AnimateArrows()
    {
        while (hovering)
        {
            buttonText.text = "< " + originalText + " >";
            yield return new WaitForSecondsRealtime(0.25f);

            buttonText.text = "<< " + originalText + " >>";
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}
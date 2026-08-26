using UnityEngine;

// Floats above whichever interactable/pickup is currently focused. Attach to
// a World Space Canvas. Reusable for both Interact (E) and Pickup
// (Right-Click) prompts - just wire Show()/Hide() to the respective events
// on PlayerInteractor.

public class FloatingPromptUI : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 0.75f, 0f);

    [SerializeField]
    private Camera worldCamera;

    private Transform target;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }
    }

    public void Show(GameObject focusedObject)
    {
        if (focusedObject == null)
            return;

        target = focusedObject.transform;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;

            if (worldCamera == null)
                return;
        }

        Vector3 worldPosition =
            target.position + offset;

        transform.position =
            worldCamera.WorldToScreenPoint(
                worldPosition
            );
    }
}
using UnityEngine;

// Floats above whichever interactable/pickup is currently focused. Attach to
// a World Space Canvas. Reusable for both Interact (E) and Pickup
// (Right-Click) prompts - just wire Show()/Hide() to the respective events
// on PlayerInteractor.
public class FloatingPromptUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.75f, 0f);

    private Transform target;

    public void Show(GameObject focusedObject)
    {
        target = focusedObject.transform;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}

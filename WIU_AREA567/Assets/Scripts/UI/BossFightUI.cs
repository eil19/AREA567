using UnityEngine;
using UnityEngine.Events;

public class BossFightUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    public UnityEvent OnConfirmed;
    public UnityEvent OnCancelled;

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void Confirm()
    {
        if (panel != null)
            panel.SetActive(false);

        OnConfirmed?.Invoke();
    }

    public void Cancel()
    {
        if (panel != null)
            panel.SetActive(false);

        OnCancelled?.Invoke();
    }
}
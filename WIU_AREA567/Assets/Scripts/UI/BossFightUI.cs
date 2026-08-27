using UnityEngine;
using UnityEngine.Events;

public class BossFightConfirmationUI : MonoBehaviour
{
    [SerializeField]
    private GameObject bossFightPanel;

    [Header("Events")]
    public UnityEvent OnBossFightConfirmed;

    public UnityEvent OnBossFightCancelled;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        if (bossFightPanel != null)
        {
            bossFightPanel.SetActive(true);
        }
    }

    public void Hide()
    {
        if (bossFightPanel != null)
        {
            bossFightPanel.SetActive(false);
        }
    }

    public void Confirm()
    {
        Hide();

        OnBossFightConfirmed?.Invoke();
    }

    public void Cancel()
    {
        Hide();

        OnBossFightCancelled?.Invoke();
    }
}
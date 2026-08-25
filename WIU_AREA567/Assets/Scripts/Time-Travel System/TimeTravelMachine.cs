using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public class TimeTravelMachine : MonoBehaviour,
    IInteractable
{
    [SerializeField] private TimeTravelController timeTravelController;

    [Header("Events")]
    public UnityEvent OnTravelLocked;
    public UnityEvent OnTravelAllowed;

    private void Start()
    {
        if (timeTravelController == null)
        {
            timeTravelController = FindFirstObjectByType<TimeTravelController>();
        }
    }

    public void Interact(GameObject interactor)
    {
        if (timeTravelController == null) return;

        // only gate present -> past
        if (timeTravelController.CurrentTimeline == Timeline.Present)
        {
            if (OrganisationManager.Instance == null) return;
            if (!OrganisationManager.Instance.HasMetThreshold)
            {
                Debug.Log("Time travel is locked. Organise more of the laboratory first.");
                OnTravelLocked?.Invoke();
                return;
            }
        }
        OnTravelAllowed?.Invoke();
        timeTravelController.ToggleTimeline();
    }
}
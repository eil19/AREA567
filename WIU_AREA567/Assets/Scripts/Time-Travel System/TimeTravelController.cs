using UnityEngine;
using UnityEngine.Events;

public class TimeTravelController : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private Timeline currentTimeline = Timeline.Present;

    [Header("Environment")]
    [SerializeField] private GameObject presentEnvironment;
    [SerializeField] private GameObject pastEnvironment;

    [Header("Events")]
    public UnityEvent OnTravelToPast;
    public UnityEvent OnTravelToPresent;

    public Timeline CurrentTimeline { get; internal set; }

    private void Start()
    {
        UpdateTimeline();
    }

    public void TravelToPast()
    {
        if (currentTimeline == Timeline.Past) return;

        currentTimeline = Timeline.Past;
        UpdateTimeline();
        OnTravelToPast?.Invoke();
    }

    public void TravelToPresent()
    {
        if (currentTimeline == Timeline.Present) return;
        
        currentTimeline = Timeline.Present;
        UpdateTimeline();
        OnTravelToPresent?.Invoke();
    }

    public void ToggleTimeline()
    {
        if (currentTimeline == Timeline.Present)
        {
            TravelToPast();
        }
        else
        {
            TravelToPresent();
        }
    }

    private void UpdateTimeline()
    {
        if (presentEnvironment != null)
        {
            presentEnvironment.SetActive(currentTimeline == Timeline.Present);
        }

        if (pastEnvironment != null)
        {
            pastEnvironment.SetActive(currentTimeline == Timeline.Past);
        }
    }
}

public enum Timeline
{
    Present,
    Past
}

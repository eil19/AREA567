using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimeTravelController : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private Timeline currentTimeline = Timeline.Present;

    [Header("Environment")]
    [SerializeField] private GameObject presentEnvironment;
    [SerializeField] private GameObject pastEnvironment;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.8f;

    private bool isTravelling = false;

    [Header("Timeline Events")]
    public UnityEvent OnTravelToPast;
    public UnityEvent OnTravelToPresent;

    [Header("Transition Events")]
    public UnityEvent OnTimeTravelStarted;
    public UnityEvent OnTimeTravelMidpoint;
    public UnityEvent OnTimeTravelFinished;

    public Timeline CurrentTimeline => currentTimeline;
    public bool IsTravelling => isTravelling;

    private void Start()
    {
        UpdateTimeline();
    }

    public void TravelToPast()
    {
        if (currentTimeline == Timeline.Past || isTravelling)
            return;

        StartCoroutine(TimeTravelSequence(Timeline.Past));
    }

    public void TravelToPresent()
    {
        if (currentTimeline == Timeline.Present || isTravelling)
            return;

        StartCoroutine(TimeTravelSequence(Timeline.Present));
    }

    public void ToggleTimeline()
    {
        if (isTravelling)
            return;

        if (currentTimeline == Timeline.Present)
        {
            TravelToPast();
        }
        else
        {
            TravelToPresent();
        }
    }

    private IEnumerator TimeTravelSequence(Timeline targetTimeline)
    {
        // prevent another time travel from starting
        isTravelling = true;

        Debug.Log("Time travel started.");

        OnTimeTravelStarted?.Invoke();

        // wait for first half of transition
        float halfDuration = transitionDuration / 2f;

        yield return new WaitForSeconds(halfDuration);

        // transition midpoint
        currentTimeline = targetTimeline;

        UpdateTimeline();

        Debug.Log("Timeline changed to: " + currentTimeline);

        OnTimeTravelMidpoint?.Invoke();

        // tell other systems which timeline entered
        if (currentTimeline == Timeline.Past)
        {
            OnTravelToPast?.Invoke();
        }
        else
        {
            OnTravelToPresent?.Invoke();
        }

        // wait for second half
        yield return new WaitForSeconds(halfDuration);

        // transition completely finished
        isTravelling = false;

        Debug.Log("Time travel finished.");

        OnTimeTravelFinished?.Invoke();
    }

    private void UpdateTimeline()
    {
        if (presentEnvironment != null)
        {
            presentEnvironment.SetActive(
                currentTimeline == Timeline.Present);
        }

        if (pastEnvironment != null)
        {
            pastEnvironment.SetActive(
                currentTimeline == Timeline.Past);
        }
    }
}

public enum Timeline
{
    Present,
    Past
}
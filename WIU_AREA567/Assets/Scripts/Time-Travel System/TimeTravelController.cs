using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TimeTravelController : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private Timeline currentTimeline = Timeline.Present;

    [Header("Timeline Scenes")]
    [SerializeField] private string presentSceneName;
    [SerializeField] private string pastSceneName;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.8f;

    [Header("Shader")]
    [SerializeField] private Material timeTravelMaterial;
    [SerializeField] private float rippleDistortionStrength = 0.05f;

    private bool isTravelling = false;
    private Vector3 savedPlayerPosition;

    [Header("Timeline Events")]
    public UnityEvent OnTravelToPast;
    public UnityEvent OnTravelToPresent;

    [Header("Transition Events")]
    public UnityEvent OnTimeTravelStarted;
    public UnityEvent OnTimeTravelMidpoint;
    public UnityEvent OnTimeTravelFinished;

    public Timeline CurrentTimeline => currentTimeline;
    public bool IsTravelling => isTravelling;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (timeTravelMaterial != null)
        {
            timeTravelMaterial.SetFloat("_TransitionProgress", 0.0f);
            timeTravelMaterial.SetFloat("_DistortionStrength", 0.0f);
        }
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

        // save current player position
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            savedPlayerPosition = player.transform.position;
        }

        // reset shader
        if (timeTravelMaterial != null)
        {
            timeTravelMaterial.SetFloat(
                "_TransitionProgress", 0.0f);
            timeTravelMaterial.SetFloat(
                "_DistortionStrength", rippleDistortionStrength);
        }

        float elapsed = 0.0f;
        bool timelineSwitched = false;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = 
                Mathf.Clamp01(elapsed / transitionDuration);

            // update shader ripple
            if (timeTravelMaterial != null)
            {
                timeTravelMaterial.SetFloat("_TransitionProgress", progress);
            }

            // switch timeline once at mid point
            if (!timelineSwitched && progress >= 0.5f)
            {
                timelineSwitched = true;
                currentTimeline = targetTimeline;

                string targetScene =
                    currentTimeline == Timeline.Past
                    ? pastSceneName : presentSceneName;

                SceneManager.LoadScene(targetScene);
                // wait for new scene objects
                yield return null;

                GameObject newPlayer =
                    GameObject.FindGameObjectWithTag("Player");
                if (newPlayer != null)
                {
                    newPlayer.transform.position = savedPlayerPosition;
                }

                Debug.Log("Timeline changed to: " + currentTimeline);
                OnTimeTravelMidpoint?.Invoke();
                if (currentTimeline == Timeline.Past)
                {
                    OnTravelToPast?.Invoke();
                }
                else
                {
                    OnTravelToPresent?.Invoke();
                }
            }

            yield return null;
        }

        // turn distortion off
        if (timeTravelMaterial != null)
        {
            timeTravelMaterial.SetFloat("_TransitionProgress", 0.0f);
            timeTravelMaterial.SetFloat("_DistortionStrength", 0.0f);
        }

        // transition completely finished
        isTravelling = false;

        Debug.Log("Time travel finished.");

        OnTimeTravelFinished?.Invoke();
    }
}

public enum Timeline
{
    Present,
    Past
}
using UnityEngine;
using UnityEngine.Events;

public class PastTimeController : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimit = 60.0f;

    [Header("Events")]
    public UnityEvent<float> OnTimeChanged;
    public UnityEvent OnTimeExpired;

    private float remainingTime;
    private bool timerRunning;

    public float RemainingTime => remainingTime;
    public float TimeLimit => timeLimit;
    public bool IsRunning => timerRunning;

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (!timerRunning) return;
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
            OnTimeChanged?.Invoke(remainingTime);
            ExpireTimer();
            return;
        }
        OnTimeChanged?.Invoke(remainingTime);
    }

    public void StartTimer()
    {
        remainingTime = timeLimit;
        timerRunning = true;
        OnTimeChanged?.Invoke(remainingTime);
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    private void ExpireTimer()
    {
        if (!timerRunning) return;

        timerRunning = false;
        Debug.Log("Past time limit expired.");
        OnTimeExpired?.Invoke();
    }
}
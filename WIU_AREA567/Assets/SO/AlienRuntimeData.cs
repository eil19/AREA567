using UnityEngine;

public class AlienRuntimeData : MonoBehaviour
{
    [Header("Attack timing (per-instance)")]
    public float timeLastShot = 0f;
    public float timeLastSummon = 0f;

    [Header("State timer (per-instance)")]
    public float stateTimerStart = 0f;

}

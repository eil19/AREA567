using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMover : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float waitTime = 2.0f;
    public bool loopWaypoints = true;
    public Transform[] waypoints;

    public UnityEvent<Vector2> OnMoveDirectionChanged;
    public UnityEvent OnStartedMoving;
    public UnityEvent OnStoppedMoving;

    private int currentWaypointIndex;
    private bool isWaiting;
    private bool isPaused;

    private Vector2 lastDirection;

    private void Start()
    {
        OnStoppedMoving?.Invoke();
    }

    private void Update()
    {
        // pause game 
        if (isWaiting || isPaused) return;
        if (waypoints == null || waypoints.Length == 0) return;

        // move to waypoint
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        if (direction != lastDirection)
        {
            lastDirection = direction;
            OnMoveDirectionChanged?.Invoke(direction);
        }

        transform.position =
            Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            // wait waypoint
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        OnStoppedMoving?.Invoke();
        yield return new WaitForSeconds(waitTime);

        // looping -> increment currentwaypointindex and wrap around if needed
        // not looping -> increment currentwaypointindex but do not exceed last 
        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length
            : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;

        if (!isPaused)
        {
            OnStartedMoving?.Invoke();
        }
    }

    public void PauseMovement()
    {
        isPaused = true;
        OnStoppedMoving?.Invoke();
    }

    public void ResumeMovement()
    {
        isPaused = false;

        if (!isWaiting)
        {
            OnStartedMoving?.Invoke();
        }
    }
}
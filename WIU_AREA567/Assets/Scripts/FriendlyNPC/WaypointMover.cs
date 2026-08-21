using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float waitTime = 2.0f;
    public bool loopWaypoints = true;
    public Transform[] waypoints;

    private int currentWaypointIndex;
    private bool isWaiting;

    private void Update()
    {
        // pause game 
        if (isWaiting) return;

        // move to waypoint
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
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
        yield return new WaitForSeconds(waitTime);

        // looping -> increment currentwaypointindex and wrap around if needed
        // not looping -> increment currentwaypointindex but do not exceed last 
        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length
            : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}
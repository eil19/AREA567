using UnityEngine;

[CreateAssetMenu(fileName = "FollowPlayerAction", menuName = "Scriptable Objects/Actions/FollowPlayerAction")]
public class FollowPlayerAction : StateAction
{
    [Header("Chase")]
    [SerializeField] public float followRange = 6f;
    [SerializeField] public float stopDistance = 4.0f;

    [SerializeField] public float leashDistance = 8f;
    [SerializeField] public float returnStopDistance = 1.0f;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        var rb = controller.GetComponent<Rigidbody2D>();
        var player = GameObject.FindGameObjectWithTag("Player");

        if (alien == null || rb == null || alien.alienType == null) return;

        float distanceFromHome = Vector2.Distance(rb.position, alien.homePosition);
        float distanceToPlayer = player != null
            ? Vector2.Distance(rb.position, player.transform.position)
            : Mathf.Infinity;

        bool playerOutOfRange = distanceToPlayer >= followRange;
        bool tooFarFromHome = distanceFromHome > leashDistance;

        if (playerOutOfRange || tooFarFromHome)
        {
            // Either the player's not close enough, wandered too far from spawn
            Vector2 toHome = (Vector2)alien.homePosition - rb.position;

            if (toHome.magnitude <= returnStopDistance)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = toHome.normalized * alien.alienType.moveSpeed;
            return;
        }

        // Player is both in range and we're within our leash, chase.
        if (distanceToPlayer <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)player.transform.position - rb.position).normalized;
        rb.linearVelocity = direction * alien.alienType.moveSpeed;
    }
}

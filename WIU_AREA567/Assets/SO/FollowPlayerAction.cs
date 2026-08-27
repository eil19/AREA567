using UnityEngine;

[CreateAssetMenu(fileName = "FollowPlayerAction", menuName = "Scriptable Objects/Actions/FollowPlayerAction")]
public class FollowPlayerAction : StateAction
{
    using UnityEngine;

[CreateAssetMenu(fileName = "FollowPlayerAction", menuName = "Scriptable Objects/Actions/FollowPlayerAction")]
public class FollowPlayerAction : StateAction
{
    [Header("Chase")]
    public float followRange = 6f;
    public float stopDistance = 1.5f;

    [Header("Return Home")]
    [Tooltip("How close to homePosition counts as 'arrived' before stopping.")]
    public float returnStopDistance = 0.2f;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        var mover = controller.GetComponent<AlienPathMover>();
        var player = GameObject.FindGameObjectWithTag("Player");

        if (alien == null || mover == null || alien.alienType == null) return;

        float distanceToPlayer = player != null
            ? Vector2.Distance(controller.transform.position, player.transform.position)
            : Mathf.Infinity;

        if (player != null && distanceToPlayer <= followRange)
        {
            if (distanceToPlayer <= stopDistance)
            {
                mover.Stop();
                return;
            }

            mover.MoveTowards(player.transform.position, alien.alienType.moveSpeed);
        }
        else
        {
            float distanceToHome = Vector2.Distance(controller.transform.position, alien.homePosition);
            if (distanceToHome <= returnStopDistance)
            {
                mover.Stop();
                return;
            }

            mover.MoveTowards(alien.homePosition, alien.alienType.moveSpeed);
        }
    }
}
}

using UnityEngine;

[CreateAssetMenu(fileName = "FollowPlayerAction", menuName = "Scriptable Objects/Actions/FollowPlayerAction")]
public class FollowPlayerAction : StateAction
{
    public float stopDistance = 1.5f;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        var rb = controller.GetComponent<Rigidbody2D>();
        var player = GameObject.FindGameObjectWithTag("Player");

        if (alien == null || rb == null || alien.alienType == null || player == null) return;

        Vector2 toPlayer = (Vector2)player.transform.position - rb.position;
        float distance = toPlayer.magnitude;

        if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero; 
            return;
        }

        Vector2 direction = toPlayer.normalized;
        rb.linearVelocity = direction * alien.alienType.moveSpeed;
    }
}

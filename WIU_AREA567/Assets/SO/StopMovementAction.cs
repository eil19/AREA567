using UnityEngine;

[CreateAssetMenu(fileName = "StopMovementAction", menuName = "Scriptable Objects/Actions/StopMovementAction")]
public class StopMovementAction : StateAction
{
    public override void Act(StateController controller)
    {
        if (controller.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}

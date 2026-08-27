using UnityEngine;

[CreateAssetMenu(fileName = "RiseToHoverAction", menuName = "Scriptable Objects/Actions/RiseToHoverAction")]
public class RiseToHoverAction : StateAction
{
    public float riseHeight = 1.5f;
    public float riseSpeed = 4f;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        var rb = controller.GetComponent<Rigidbody2D>();
        if (alien == null || rb == null) return;

        Vector2 riseTarget = (Vector2)alien.homePosition + Vector2.up * riseHeight;
        rb.position = Vector2.MoveTowards(rb.position, riseTarget, riseSpeed * Time.deltaTime);
    }
}

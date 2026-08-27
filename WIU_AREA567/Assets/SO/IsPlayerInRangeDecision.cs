using UnityEngine;

[CreateAssetMenu(fileName = "IsPlayerInRangeDecision", menuName = "Scriptable Objects/Decisions/IsPlayerInRangeDecision")]
public class IsPlayerInRangeDecision : StateDecision
{
    public float followDistance = 3f;

    public override bool Decide(StateController controller)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // Was ">" - that returned true when the player is FAR away, which
        // sent the boss into Follow while distant and Remain while close.
        return Vector2.Distance(controller.transform.position, player.transform.position) <= followDistance;
    }
}

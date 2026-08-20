using UnityEngine;

[CreateAssetMenu(fileName = "IsPlayerFarDecision", menuName = "Scriptable Objects/Decisions/IsPlayerFar")]
public class IsPlayerWithinRange : StateDecision
{
    public float followDistance = 3f;

    public override bool Decide(StateController controller)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;
        return Vector2.Distance(controller.transform.position, player.transform.position) > followDistance;
    }
}

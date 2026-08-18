using UnityEngine;

[CreateAssetMenu(fileName = "IsHitDecision", menuName = "Scriptable Objects/Decisions/Is Hit")]
public class IsHitDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        return controller.isHit;
    }
}
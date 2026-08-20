using UnityEngine;

[CreateAssetMenu(fileName = "IsHitDecision", menuName = "Scriptable Objects/Decisions/Is Hit")]
public class IsHitDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (controller.isHit)
        {
            controller.isHit = false;
            return true;
        }
        return false;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "IsTasedDecision", menuName = "Scriptable Objects/Decisions/IsTasedDecision")]
public class IsTasedDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (controller.isTased)
        {
            controller.isTased = false;
            return true;
        }
        return false;
    }
}

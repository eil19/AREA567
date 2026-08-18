using UnityEngine;

[CreateAssetMenu(fileName = "IsIdentifiedDecision", menuName = "Scriptable Objects/Decisions/Is Identified")]
public class IsIdentifiedDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        return alien != null && alien.identified;
    }
}
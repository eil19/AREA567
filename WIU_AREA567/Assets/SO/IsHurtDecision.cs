using UnityEngine;

[CreateAssetMenu(fileName = "IsHurtDecision", menuName = "Scriptable Objects/FSM/Decisions/Is Hurt")]
public class IsHurtDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        return alien != null && alien.tamingAttempted && alien.tamingSucceeded;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "TamingFailedDecision", menuName = "Scriptable Objects/FSM/Decisions/Taming Failed")]
public class TamingFailedDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        return alien != null && alien.tamingAttempted && !alien.tamingSucceeded;
    }
}
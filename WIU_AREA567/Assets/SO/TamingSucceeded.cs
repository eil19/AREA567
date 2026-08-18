using UnityEngine;

[CreateAssetMenu(fileName = "TamingSucceeded", menuName = "Scriptable Objects/Decisions/Taming Succeeded")]
public class TamingSucceededDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        return alien != null && alien.tamingAttempted && alien.tamingSucceeded;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "TamingFailedDecision", menuName = "Scriptable Objects/Decisions/Taming Failed")]
public class TamingFailedDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            if (alien.tameFailTrigger)
            {
                alien.tameFailTrigger = false;
                return true;
            }
        }
        return false;
    }
}
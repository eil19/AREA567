using UnityEngine;

[CreateAssetMenu(fileName = "TamingSucceeded", menuName = "Scriptable Objects/Decisions/Taming Succeeded")]
public class TamingSucceededDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            if (alien.tameSuccessTrigger)
            {
                alien.tameSuccessTrigger = false;
                return true;
            }
        }
        return false;
    }
}

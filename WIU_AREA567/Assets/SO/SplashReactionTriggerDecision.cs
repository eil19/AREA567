using UnityEngine;

[CreateAssetMenu(fileName = "SplashReactionTriggerDecision", menuName = "Scriptable Objects/Decisions/SplashReactionTriggerDecision")]
public class SplashReactTriggerDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (controller.TryGetComponent(out AlienInstance alien))
        {
            return alien.splashReactTrigger;
        }
        return false;
    }
}
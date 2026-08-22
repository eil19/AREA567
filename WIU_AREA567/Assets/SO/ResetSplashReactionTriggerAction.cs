using UnityEngine;

[CreateAssetMenu(fileName = "ResetSplashReactTriggerAction", menuName = "Scriptable Objects/Actions/ResetSplashReactTriggerAction")]
public class ResetSplashReactTriggerAction : StateAction
{
    public override void Act(StateController controller)
    {
        if (controller.TryGetComponent(out AlienInstance alien))
        {
            alien.splashReactTrigger = false;
        }
    }
}
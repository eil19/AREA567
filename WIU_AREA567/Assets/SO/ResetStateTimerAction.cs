using UnityEngine;

[CreateAssetMenu(fileName = "ResetStateTimerAction", menuName = "Scriptable Objects/Actions/ResetStateTimerAction")]
public class ResetStateTimerAction : StateAction
{
    public override void Act(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            alien.stateTimerStart = Time.time;
        }
    }
}

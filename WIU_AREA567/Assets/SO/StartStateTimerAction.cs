using UnityEngine;

[CreateAssetMenu(fileName = "StartStateTimerAction", menuName = "Scriptable Objects/Actions/StartStateTimerAction")]
public class StartStateTimerAction : StateAction
{
    public override void Act(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            alien.stateTimerStart = Time.time;
        }
    }
}
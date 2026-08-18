using UnityEngine;

[CreateAssetMenu(fileName = "TimerElapsedDecision", menuName = "Scriptable Objects/Decisions/Timer Elapsed")]
public class TimerElapsedDecision : StateDecision
{
    public float duration = 5f;

    public override bool Decide(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            return Time.time - alien.stateTimerStart >= duration;
        }

        return false;
    }
}


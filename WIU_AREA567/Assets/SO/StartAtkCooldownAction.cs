using UnityEngine;

[CreateAssetMenu(fileName = "StartAtkCooldownAction", menuName = "Scriptable Objects/Actions/StartAtkCooldownAction")]
public class StartAtkCooldownAction : StateAction
{
    public override void Act(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            alien.lastAttackTime = Time.time;
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "StartSpecialAtkCooldown", menuName = "Scriptable Objects/Actions/StartSpecialAtkCooldown")]
public class StartSpecialAtkCooldown : StateAction
{
    public override void Act(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            alien.lastSpecialAttackTime = Time.time;
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "SpecialAtkCooldownDecision", menuName = "Scriptable Objects/Decisions/SpecialAtkCooldownDecision")]
public class SpecialAtkCooldownDecision : StateDecision
{
    public float specialAtkCooldown = 10f;

    public override bool Decide(StateController controller)
    {
        if (controller.TryGetComponent<AlienInstance>(out AlienInstance alien))
        {
            return Time.time - alien.lastSpecialAttackTime >= specialAtkCooldown;
        }

        return false;
    }
}

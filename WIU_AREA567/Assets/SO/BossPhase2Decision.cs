using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase2Decision", menuName = "Scriptable Objects/Decisions/BossPhase2decision")]
public class BossPhase2Decision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (!controller.TryGetComponent(out BossDamageable boss)) return false;
        return boss.Phase == 2;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "ToughnessBarBrokenDecision", menuName = "Scriptable Objects/Decisions/ToughnessBarBrokenDecision")]
public class ToughnessBarBrokenDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (!controller.TryGetComponent(out BossDamageable boss)) return false;
        if (boss.IsProtected) return false;
        return boss.ToughnessBroken;
    }
}

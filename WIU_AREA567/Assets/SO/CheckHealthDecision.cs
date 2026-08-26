using UnityEngine;

[CreateAssetMenu(fileName = "CheckHealthDecision", menuName = "Scriptable Objects/Decisions/CheckHealthDecision")]
public class CheckHealthDecision : StateDecision
{
    public float healthToCheck;

    public override bool Decide(StateController controller)
    {
        if (controller.TryGetComponent<Damageable>(out var damageable))
        {
            return damageable.CurrentHealth <= healthToCheck;
        }

        return false;
    }
}

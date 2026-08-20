using UnityEngine;

[CreateAssetMenu(fileName = "CanAtkAgainDecision", menuName = "Scriptable Objects/Decisions/CanAtkAgainDecision")]
public class CanAtkAgainDecision : StateDecision
{
    public float attackCooldown = 3f;
    
    public override bool Decide(StateController controller)
    {
        if (!controller.TryGetComponent<AlienInstance>(out AlienInstance alien) || alien.alienType == null)
            return false;

        bool cooldownDone = Time.time - alien.lastAttackTime >= attackCooldown;
        bool enemyNear = Physics2D.OverlapCircle(controller.transform.position, alien.alienType.detectRadius, alien.alienType.enemyLayer) != null;

        return cooldownDone && enemyNear;
    }
}

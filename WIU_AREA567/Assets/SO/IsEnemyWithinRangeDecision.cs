using UnityEngine;

[CreateAssetMenu(fileName = "IsEnemyInRangeDecision", menuName = "Scriptable Objects/Decisions/IsEnemyInRange")]
public class IsEnemyInRangeDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        if (!controller.TryGetComponent<AlienInstance>(out AlienInstance alien) || alien.alienType == null)
            return false;

        return Physics2D.OverlapCircle(controller.transform.position, alien.alienType.detectRadius, alien.alienType.enemyLayer) != null;
    }
}

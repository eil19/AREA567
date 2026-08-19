using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "AlienAtkAction", menuName = "Scriptable Objects/Actions/AlienAtkAction")]
public class AlienAtkAction : StateAction
{
    [Header("Radius")]
    public float radius = 2f;

    [Header("Damage Multipliers")]
    public float playerDamageMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    [Header("Optional Knockback")]
    public float knockbackForce = 0f;

    public override void Act(StateController controller)
    {

        var alien = controller.GetComponent<AlienInstance>();
        if (alien == null || alien.alienType == null) return;

        float baseDamage = alien.alienType.damage;
        float radius = alien.alienType.detectRadius;

        // Combine both masks into one overlap check so targets standing on either
        Vector2 origin = controller.transform.position;
        LayerMask combinedMask = playerLayer | enemyLayer;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, combinedMask);

        foreach (Collider2D hit in hits)
        {
            float multiplier;

            // Don't damage yourself if you happen to share the target layer
            if (hit.gameObject == controller.gameObject) continue;

            if (!hit.TryGetComponent<Damageable>(out Damageable damageable)) continue;

            int hitLayerMask = 1 << hit.gameObject.layer;


            if ((playerLayer.value & hitLayerMask) != 0)
            {
                multiplier = playerDamageMultiplier;
            }
            else if ((enemyLayer.value & hitLayerMask) != 0)
            {
                multiplier = enemyDamageMultiplier;
            }
            else
            {
                continue; // shouldn't happen given combinedMask, but guards against edge cases
            }

            if (multiplier <= 0f) continue; // 0 multiplier means "don't damage this target type"

            int damage = Mathf.RoundToInt(baseDamage * multiplier);

            if (knockbackForce > 0f)
            {
                Vector2 direction = (Vector2)hit.transform.position - origin;
                damageable.TakeDamage(damage, direction, knockbackForce);
            }
            else
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}



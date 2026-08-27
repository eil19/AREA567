using UnityEngine;

[CreateAssetMenu(fileName = "CanHealDecision", menuName = "Scriptable Objects/Decisions/Can Heal")]
public class CanHealDecision : StateDecision
{
    public LayerMask allyLayer;
    public float healRadius = 4f;
    public float healCooldown = 3f;

    public override bool Decide(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        if (alien == null) return false;

        if (Time.time - alien.lastHealTime < healCooldown) return false;

        Vector2 origin = controller.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, healRadius, allyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == controller.gameObject) continue;
            if (!hit.TryGetComponent(out Damageable damageable)) continue;
            if (damageable.CurrentHealth < damageable.MaxHealth) return true;
        }

        return false;
    }
}
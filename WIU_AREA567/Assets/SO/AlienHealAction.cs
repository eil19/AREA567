using UnityEngine;

[CreateAssetMenu(fileName = "AlienHealAction", menuName = "Scriptable Objects/Actions/AlienHealAction")]
public class AlienHealAction : StateAction
{
    [Header("Heal")]
    public int healAmount = 15;
    public float healRadius = 4f;
    public LayerMask allyLayer;

    [Header("Cooldown")]
    public float healCooldown = 3f;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        if (alien == null) return;

        if (Time.time - alien.lastHealTime < healCooldown) return;

        Vector2 origin = controller.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, healRadius, allyLayer);

        bool healedSomeone = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == controller.gameObject) continue; // don't heal self here - add a self-heal action separately if wanted
            if (!hit.TryGetComponent(out Damageable damageable)) continue;
            if (damageable.CurrentHealth >= damageable.MaxHealth) continue; // already full

            damageable.Heal(healAmount);
            healedSomeone = true;
        }

        if (healedSomeone)
        {
            alien.lastHealTime = Time.time;
        }
    }
}
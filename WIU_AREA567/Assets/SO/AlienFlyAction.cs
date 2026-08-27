using UnityEngine;

[CreateAssetMenu(fileName = "AlienFlyAction", menuName = "Scriptable Objects/Actions/AlienFlyAction")]
public class AlienFlyAction : StateAction
{
    [Header("Fireball")]
    public GameObject fireballPrefab;
    public float fireballSpeed = 6f;
    public int fireballDamage = 10;
    public float attackRadius = 6f;
    public float attackCooldown = 2f;
    public LayerMask enemyLayer;

    [Header("Hover Positioning")]
    public bool hoverAboveAllies = true;
    public float hoverHeight = 2f;
    public float moveSpeed = 3f;
    public float stopDistance = 0.3f;
    public float allySearchRadius = 6f;
    public LayerMask allyLayer;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        var rb = controller.GetComponent<Rigidbody2D>();

        if (alien == null) return;

        Vector2 origin = rb.position;

        if (hoverAboveAllies)
        {
            Vector2 allyCentre = AlienActionUtils.GetAllyCentre(origin, allySearchRadius, allyLayer, controller.gameObject);
            Vector2 hoverTarget = allyCentre + Vector2.up * hoverHeight;

            // Direct transform movement - swap for Rigidbody2D.MovePosition()
            controller.transform.position = Vector2.MoveTowards(origin, hoverTarget, moveSpeed * Time.deltaTime);
            rb.linearVelocity = hoverTarget * moveSpeed * Time.deltaTime;

            Vector2 toTarget = hoverTarget - origin;
            float distance = toTarget.magnitude;

            if (distance <= stopDistance)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.linearVelocity = toTarget.normalized * moveSpeed;
            }
        }

        if (Time.time - alien.lastAttackTime < attackCooldown) return;
        if (fireballPrefab == null) return;

        Collider2D nearestEnemy = AlienActionUtils.FindNearest(origin, attackRadius, enemyLayer, controller.gameObject);
        if (nearestEnemy == null) return;

        Vector2 direction = (Vector2)nearestEnemy.transform.position - origin;

        GameObject fireballObj = Object.Instantiate(fireballPrefab, origin, Quaternion.identity);
        if (fireballObj.TryGetComponent(out AlienFireball fireball))
        {
            fireball.Launch(direction, fireballSpeed, fireballDamage, enemyLayer);
        }

        alien.lastAttackTime = Time.time;
    }
}
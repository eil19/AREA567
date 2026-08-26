using UnityEngine;

[CreateAssetMenu(fileName = "AlienTankAction", menuName = "Scriptable Objects/Actions/AlienTankAction")]
public class AlienTankAction : StateAction
{
    [Header("Shield Maintenance")]
    public bool maintainShield = true;

    [Header("Positioning")]
    public float moveSpeed = 2.5f;
    public float standOffDistance = 1.5f; // how far in front of the ally centre to plant itself
    public float allySearchRadius = 6f;
    public float enemySearchRadius = 8f;
    public LayerMask allyLayer;
    public LayerMask enemyLayer;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        if (alien == null) return;

        if (maintainShield && controller.TryGetComponent(out AlienShield shield))
        {
            shield.TryRegenerate();
        }

        Vector2 origin = controller.transform.position;

        Collider2D nearestEnemy = AlienActionUtils.FindNearest(origin, enemySearchRadius, enemyLayer, controller.gameObject);
        if (nearestEnemy == null) return; // nothing to block right now - hold position

        Vector2 allyCentre = AlienActionUtils.GetAllyCentre(origin, allySearchRadius, allyLayer, controller.gameObject);
        Vector2 enemyPos = nearestEnemy.transform.position;

        Vector2 toEnemy = (enemyPos - allyCentre).normalized;
        Vector2 targetPosition = allyCentre + toEnemy * standOffDistance;

        // Direct transform movement - swap for Rigidbody2D.MovePosition() if
        // your aliens move via physics elsewhere in the project.
        controller.transform.position = Vector2.MoveTowards(origin, targetPosition, moveSpeed * Time.deltaTime);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "FireBallsAction", menuName = "Scriptable Objects/Actions/FireBallsAction")]
public class FireBallsAction : StateAction
{
    public GameObject fireballPrefab;
    public float fireballSpeed = 6f;
    public int fireballDamage = 10;
    public LayerMask targetLayer;
    public string targetTag = "Enemy";

    public override void Act(StateController controller)
    {
        if (fireballPrefab == null) return;

        Vector2 origin = controller.transform.position;

        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        Vector2 directionToTarget = targetObj != null
            ? ((Vector2)targetObj.transform.position - origin).normalized
            : Vector2.right; // fallback if no target found

        // Perpendicular to the aim direction, so the two shots fan out to either side of the target line.
        Vector2 perpendicular = new Vector2(-directionToTarget.y, directionToTarget.x);

        Fire(origin, directionToTarget);
        Fire(origin, -directionToTarget);
    }

    private void Fire(Vector2 origin, Vector2 direction)
    {
        GameObject fireballObj = Object.Instantiate(fireballPrefab, origin, Quaternion.identity);
        if (fireballObj.TryGetComponent(out AlienFireball fireball))
        {
            fireball.Launch(direction, fireballSpeed, fireballDamage, targetLayer);
        }
    }
}

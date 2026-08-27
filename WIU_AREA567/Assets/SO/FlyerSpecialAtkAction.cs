using UnityEngine;

[CreateAssetMenu(fileName = "FlyerSpecialAtkAction", menuName = "Scriptable Objects/Actions/FlyerSpecialAtkAction")]
public class FlyerSpecialAtkAction : StateAction
{
    [Header("Special Attack")]
    public GameObject fireballPrefab;
    public float fireballSpeed = 6f;
    public int fireballDamage = 10;
    public float specialAttackCooldown = 8f;

    [Header("Rise")]
    public float riseHeight = 1.5f;
    public float riseSpeed = 4f;

    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        var rb = controller.GetComponent<Rigidbody2D>();

        if (alien == null || rb == null) return;
        if (fireballPrefab == null) return;

        if (Time.time - alien.lastSpecialAttackTime < specialAttackCooldown) return;

        // Rise up before firing.
        Vector2 riseTarget = alien.homePosition + Vector3.up * riseHeight;
        rb.position = Vector2.MoveTowards(rb.position, riseTarget, riseSpeed * Time.deltaTime);

        FireSideways(controller, rb.position, Vector2.left);
        FireSideways(controller, rb.position, Vector2.right);

        alien.lastSpecialAttackTime = Time.time;
    }

    private void FireSideways(StateController controller, Vector2 origin, Vector2 direction)
    {
        GameObject fireballObj = Object.Instantiate(fireballPrefab, origin, Quaternion.identity);
        if (fireballObj.TryGetComponent(out AlienFireball fireball))
        {
            fireball.Launch(direction, fireballSpeed, fireballDamage, LayerMask.GetMask("Enemy"));
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "SummonMinionAction", menuName = "Scriptable Objects/Actions/SummonMinionAction")]
public class SummonMinionAction : StateAction
{
    public GameObject minionPrefab;
    public int minionCount = 3;
    public float spawnRadius = 2.5f;
    public float summonCooldown = 12f;
    public float spawnArcDegrees = 90f;
    public float maxVerticalOffset = 0.4f;
    public string targetTag = "Player";

    public override void Act(StateController controller)
    {
        if (!controller.TryGetComponent(out BossDamageable boss)) return;

        // Don't summon a new wave while the previous one is still alive.
        if (boss.IsProtected) return;
        if (minionPrefab == null) return;

        // Cooldown starts from when the last minion died, not from the last summon.
        if (Time.time < boss.lastMinionDeathTime + summonCooldown) return;


        GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
        Vector2 forwardDirection = Vector2.right; // fallback if the player can't be found
        if (playerObj != null)
        {
            forwardDirection = ((Vector2)playerObj.transform.position - (Vector2)controller.transform.position).normalized;
        }

        for (int i = 0; i < minionCount; i++)
        {
            float angleOffset = Random.Range(-spawnArcDegrees / 2f, spawnArcDegrees / 2f);
            Vector2 spawnDirection = Quaternion.Euler(0f, 0f, angleOffset) * forwardDirection;
            float radius = Random.Range(spawnRadius * 0.7f, spawnRadius * 1.3f);

            Vector3 spawnPos = controller.transform.position + (Vector3)(spawnDirection * radius);
            spawnPos.y = Mathf.Clamp(spawnPos.y, controller.transform.position.y - maxVerticalOffset, controller.transform.position.y + maxVerticalOffset);

            GameObject minionObj = Object.Instantiate(minionPrefab, spawnPos, minionPrefab.transform.rotation);

            if (minionObj.TryGetComponent(out AlienMinion minion))
            {
                minion.SetProtectedBoss(boss);
            }
        }
    }
}

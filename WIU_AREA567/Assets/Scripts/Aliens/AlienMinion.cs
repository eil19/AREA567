using UnityEngine;

public class AlienMinion : MonoBehaviour
{
    private BossDamageable protectedBoss;
    private Damageable ownHealth;

    public void SetProtectedBoss(BossDamageable boss)
    {
        protectedBoss = boss;
        protectedBoss.RegisterProtector();
    }

    private void Awake()
    {
        ownHealth = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        if (ownHealth != null)
        {
            ownHealth.OnDeath.AddListener(HandleOwnDeath);
        }
    }

    private void OnDisable()
    {
        if (ownHealth != null)
        {
            ownHealth.OnDeath.RemoveListener(HandleOwnDeath);
        }
    }

    private void HandleOwnDeath()
    {
        if (protectedBoss != null)
        {
            protectedBoss.UnregisterProtector();
            protectedBoss = null; // avoid double-unregister if this fires twice
        }
    }
}

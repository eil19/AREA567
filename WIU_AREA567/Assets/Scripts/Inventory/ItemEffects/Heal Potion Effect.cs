using UnityEngine;

[CreateAssetMenu(fileName = "HealPotionEffect", 
    menuName = "Inventory/Effects/HealPotionEffect")]
public class HealPotionEffect : ItemEffect
{
    public int healAmount = 0;

    public override void Use(GameObject user)
    {
        var health = user.GetComponent<Damageable>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }
}
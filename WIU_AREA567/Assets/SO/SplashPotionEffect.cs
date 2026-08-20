using UnityEngine;

[CreateAssetMenu(fileName = "SplashPotionEffect", menuName = "Inventory/Effects/Splash Potion")]
public class SplashPotionEffect : ItemEffect
{
    public override void Use(GameObject user)
    {
        var alien = AlienInteractionTarget.Current;

        if (alien == null)
        {
            Debug.Log("[SplashPotionEffect] No alien pod nearby to use this on.");
            return;
        }

        if (alien.identified)
        {
            Debug.Log("[SplashPotionEffect] This alien is already identified.");
            return;
        }

        // popup automatically once that animation finishes.
        alien.MarkIdentified();

    }
}
using UnityEngine;

public class BossAllyLoader : MonoBehaviour
{
    [Header("Boss Allies")]
    [SerializeField] private GameObject healerAlly;
    [SerializeField] private GameObject damagerAlly;
    [SerializeField] private GameObject flyerAlly;

    private void Start()
    {
        SetAllyState(
            healerAlly,
            AlienRunData.IsTamed("Healer")
        );

        SetAllyState(
            damagerAlly,
            AlienRunData.IsTamed("Damager")
        );

        SetAllyState(
            flyerAlly,
            AlienRunData.IsTamed("Flyer")
        );
    }

    private void SetAllyState(
        GameObject ally,
        bool shouldBeActive)
    {
        if (ally == null)
            return;

        ally.SetActive(shouldBeActive);
    }
}
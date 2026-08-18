using UnityEngine;

public class AlienInstance : MonoBehaviour
{
    public AlienType alienType;

    [HideInInspector] public bool identified = false;
    [HideInInspector] public bool tamingAttempted = false;
    [HideInInspector] public bool tamingSucceeded = false;

    private Animator animator; // swap for Animation component if not using an Animator Controller

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void MarkIdentified()
    {
        identified = true;
    }

    public void SpawnEssence()
    {
        if (alienType != null && alienType.essencePrefab != null)
        {
            Instantiate(alienType.essencePrefab, transform.position, Quaternion.identity);
        }
    }

    public void AttemptTame()
    {
        tamingAttempted = true;
        tamingSucceeded = Random.value <= (1f - alienType.tameDifficulty);
    }
}
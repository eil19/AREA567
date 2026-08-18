using UnityEngine;

public class AlienInstance : MonoBehaviour
{
    public AlienType alienType;

    [SerializeField] public bool identified = false;
    [SerializeField] public bool tamingAttempted = false;
    [SerializeField] public bool tamingSucceeded = false;
    [SerializeField] public bool isHit = false;
    [SerializeField] private bool isTased = false;
    [SerializeField] public float stateTimerStart = 0;

    private Animator animator; 

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
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AlienInstance : MonoBehaviour
{
    public AlienType alienType;

    [HideInInspector] public bool identified = false;
    [HideInInspector] public bool tameSuccessTrigger = false;
    [HideInInspector] public bool tameFailTrigger = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] private bool isTased = false;
    [HideInInspector] public bool splashReactTrigger = false;
    [HideInInspector] private bool _guessed = false;
    public bool Guessed { get { return _guessed; } }        
    [HideInInspector] public float stateTimerStart = 0;    

    [HideInInspector] public float lastAttackTime = -999f;

    private Animator animator;

    private Vector3 offset = new Vector3(0, -2, 0);

    [ContextMenu("TEST: Force Identify")]
    private void TestForceIdentify()
    {
        identified = true;
    }

    [ContextMenu("TEST: Force Tame Success")]
    private void TestForceTameSuccess()
    {
        tameSuccessTrigger = true;
    }

    [ContextMenu("TEST: Force Tame Fail")]
    private void TestForceTameFail()
    {
        tameFailTrigger = true;
    }

    [ContextMenu("TEST: Force Hit")]
    private void TestForceHit()
    {
        if (TryGetComponent<StateController>(out StateController controller))
        {
            controller.isHit = true;
        }
    }

    [ContextMenu("TEST: Force Tase")]
    private void TestForceDaze()
    {
        if (TryGetComponent<StateController>(out StateController controller))
        {
            controller.isTased = true;
        } 
    }

    [ContextMenu("TEST: Force Drop")]
    private void TestForceCorrect()
    {
        SpawnEssence();
    }

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

    public bool SubmitGuess(AlienCategory guess)
    {
        if (_guessed) return false; // already guessed, don't allow retries
        _guessed = true;

        bool correct = alienType != null && guess == alienType.category;
        if (correct)
        {
            SpawnEssence();
        }
        return correct;
    }

    public void AttemptTame()
    {
        bool success = Random.value <= (1f - alienType.tameDifficulty);
        if (success)
            tameSuccessTrigger = true;
        else
            tameFailTrigger = true;
    }

    public void SetTased()
    {
       isTased = true;
    }

    public void TriggerSplashReaction()
    {
        splashReactTrigger = true;
    }
}
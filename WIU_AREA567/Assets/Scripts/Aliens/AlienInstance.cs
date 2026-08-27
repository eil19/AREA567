using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AlienInstance : MonoBehaviour
{
    public AlienType alienType;
    [HideInInspector] public bool identified = false;
    [HideInInspector] public bool tameSuccessTrigger = false;
    [HideInInspector] public bool tameFailTrigger = false;
    [HideInInspector] public bool isTamed = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] private bool isTased = false;
    [HideInInspector] public bool splashReactTrigger = false;
    [HideInInspector] private bool _guessed = false;
    public bool Guessed { get { return _guessed; } }        
    [HideInInspector] public float stateTimerStart = 0;    

    [HideInInspector] public float lastAttackTime = -999f;
    //healing for healer alien
    [HideInInspector] public float lastHealTime = -999f;
    //flying n projectiles for flyer alien
    [HideInInspector] public float lastSpecialAttackTime = -999f;

    private Animator animator;

    [SerializeField]
    private Vector3 essenceSpawnOffset = new Vector3(0f, -0.5f, 0f);

    [Header("Persistence")]
    [SerializeField] private string alienID;

    private void Start()
    {
        identified =
            AlienRunData.IsIdentified(
                alienID
            );

        isTamed =
            AlienRunData.IsTamed(
                alienID
            );

        if (isTamed)
        {
            tameSuccessTrigger = true;

            SetTamedLayer();

            Debug.Log(
                gameObject.name +
                " restored as TAMED."
            );
        }

        if (identified)
        {
            _guessed = true;
        }
    }

    [ContextMenu("TEST: Force Identify")]
    private void TestForceIdentify()
    {
        identified = true;
    }

    [ContextMenu("TEST: Force Tame")]
    private void TestForceTame()
    {
        AttemptTame();
    }

    [ContextMenu("TEST: Force Tame Success")]
    private void TestForceTameSuccess()
    {
        tameSuccessTrigger = true;
        isTamed = true;
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

    [HideInInspector] public Vector3 homePosition;

    private Damageable damageable;
    private StateController stateController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        stateController = GetComponent<StateController>();
        homePosition = transform.position;
    }

    public void MarkIdentified()
    {
        identified = true;

        AlienRunData.MarkIdentified(
            alienID
        );
    }

    public void SpawnEssence()
    {
        if (alienType == null || alienType.essencePrefab == null) return;

        GameObject drop = Instantiate(alienType.essencePrefab, transform.position + essenceSpawnOffset, Quaternion.identity);

        if (drop.TryGetComponent<Item>(out var item))
        {
            item.Initialise(alienType.essenceItemData, 1);
        }
    }

    public bool SubmitGuess(AlienCategory guess)
    {
        // If already correctly identified, block
        if (identified) return false;

        bool correct = alienType != null && guess == alienType.category;
        if (correct)
        {
            identified = true;
            _guessed = true;

            AlienRunData.MarkIdentified(
                alienID
            );

            SpawnEssence();
        }
        else
        {
            Debug.Log("[AlienInstance] Incorrect guess! Try again with another potion.");
        }

        return correct;
    }

    public bool AttemptTame()
    {
        bool success = Random.value <= (1f - alienType.tameDifficulty);
        if (success)
        {
            tameSuccessTrigger = true;
            isTamed = true;

            AlienRunData.MarkTamed(
                alienID
            );

            SetTamedLayer();
        }
        else
        {
            tameFailTrigger = true;
        }
        return success;
    }

    private void SetTamedLayer()
    {
        int tamedLayer = LayerMask.NameToLayer("Tamed");
        if (tamedLayer == -1)
        {
            Debug.LogWarning($"{gameObject.name}: No layer named \"Tamed\" exists. Add one in Edit > Project Settings > Tags and Layers.");
            return;
        }

        gameObject.layer = tamedLayer;

    }

    public void SetTased()
    {
        isTased = true;

        if (stateController != null)
        {
            stateController.isTased = true;
        }

        Debug.Log(
            gameObject.name +
            " was tased."
        );
    }

    public void TriggerSplashReaction()
    {
        splashReactTrigger = true;
    }
}
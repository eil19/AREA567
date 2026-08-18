using UnityEngine;

public class StateController : MonoBehaviour
{
    public State currentState;
    public State remainState;

    [HideInInspector] public bool isHurt = false;
    private Damageable damageable;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.OnDamaged += HandleTakeDamage;
        }
    }

    private void HandleTakeDamage(int amount)
    {
        isHurt = true; // Signal the FSM that we took hit
    }

    void Start()
    {
        currentState.Initialize(this);
    }

    void Update()
    {
        currentState.Execute(this);
        currentState.CheckTransitions(this);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState.End(this);
            currentState = nextState;
            currentState.Initialize(this);
        }
    }
}
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

[CreateAssetMenu(fileName = "State", menuName = "Scriptable Objects/State")]
public class State : ScriptableObject
{
    [SerializeField] private List<StateAction> initializeActions;
    [SerializeField] private List<StateAction> executeActions;
    [SerializeField] private List<StateAction> endActions;
    [SerializeField] private List<StateTransition> transitions;

    public void Initialize(StateController controller)
    {
        foreach (StateAction action in initializeActions)
        {
            action.Act(controller);
        }
    }

    public void Execute(StateController controller)
    {
        foreach (StateAction action in executeActions)
        {
            action.Act(controller);
        }
    }

    public void End(StateController controller)
    {
        foreach (StateAction action in endActions)
        {
            action.Act(controller);
        }
    }

    public void CheckTransitions(StateController controller)
    {
        foreach (StateTransition transition in transitions)
        {
            bool decisionSucceeded = transition.decision.Decide(controller);
            State nextState = decisionSucceeded ? transition.trueState : transition.falseState;

            // Only act if this transition actually wants to change state.
            if (nextState != controller.remainState)
            {
                controller.TransitionToState(nextState);
                return;
            }
        }
    }
}
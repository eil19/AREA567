using UnityEngine;

[CreateAssetMenu(fileName = "ShowGuessUIAction", menuName = "Scriptable Objects/Actions/ShowGuessUIAction")]
public class ShowGuessUIAction : StateAction
{
    public override void Act(StateController controller)
    {
        var alien = controller.GetComponent<AlienInstance>();
        if (alien == null) return;

        if (AlienGuessUI.Instance == null)
        {
            Debug.LogWarning("[ShowGuessUIAction] No AlienGuessUI in the scene — add one under your Canvas.");
            return;
        }

        AlienGuessUI.Instance.Show(alien);
    }
}

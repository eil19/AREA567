using UnityEngine;

[CreateAssetMenu(fileName = "FlipTowardsTargetAction", menuName = "Scriptable Objects/Actions/FlipTowardsTargetAction")]
public class FlipTowardsTargetAction : StateAction
{
    public string targetTag;

    public override void Act(StateController controller)
    {
        var targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj == null) return;

        float hDiff = (targetObj.transform.position - controller.transform.position).x;

        if (hDiff >= 0.01f)
        {
            // ensure the x scale is negative (face one direction); flip sign if needed
            var scale = controller.transform.localScale;
            if (scale.x < 0f) scale.x *= -1f;
            controller.transform.localScale = scale;
        }
        else if (hDiff <= -0.01f)
        {
            var scale = controller.transform.localScale;
            if (scale.x > 0f) scale.x *= -1f;
            controller.transform.localScale = scale;
        }
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "DestroyAfterAnimationAction", menuName = "Scriptable Objects/Actions/DestroyAfterAnimationAction")]
public class DestroyAfterAnimation : StateAction
{

    public float extraDelay = 0f;

    public override void Act(StateController controller)
    {
        float clipLength = 0f;

        if (controller.TryGetComponent<Animator>(out Animator anim))
        {
            clipLength = anim.GetCurrentAnimatorStateInfo(0).length;
        }

        Destroy(controller.gameObject, clipLength + extraDelay);
    }
}
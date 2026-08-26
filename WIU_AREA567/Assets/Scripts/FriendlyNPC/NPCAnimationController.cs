using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;
    private Vector2 moveDirection = Vector2.down;
    private bool isMoving;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;

        if (isMoving)
        {
            PlayWalkAnimation();
        }
    }

    public void StartMoving()
    {
        isMoving = true;
        PlayWalkAnimation();
    }

    public void StopMoving()
    {
        isMoving = false;
        animator.Play("Idle");
    }

    private void PlayWalkAnimation()
    {
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.x > 0)
            {
                animator.Play("Walk_Right");
            }
            else
            {
                animator.Play("Walk_Left");
            }
        }
        else
        {
            if (moveDirection.y > 0)
            {
                animator.Play("Walk_Up");
            }
            else
            {
                animator.Play("Walk_Down");
            }
        }
    }
}
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private Vector3 startLocalPosition;
    [SerializeField] private Vector3 endLocalPosition;

    private int direction = 1;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 target = direction == 1 ? endLocalPosition : startLocalPosition;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            target,
            moveSpeed * Time.fixedDeltaTime
        );

        Vector3 moveDirection = target - transform.localPosition;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsJumping", false);
            animator.SetFloat("MovementForward", 1f);
        }

        if (Vector3.Distance(transform.localPosition, target) < 0.01f)
        {
            direction *= -1;
        }
    }
}
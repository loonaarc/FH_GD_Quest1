using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float raycastLength = 1.2f;
    [SerializeField] private Transform cameraTransform;

    [Header("Audio")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private CharacterController controller;
    private Animator animator;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector3 velocity;
    private Vector3 characterMovement;
    private Vector3 platformVelocity;

    private float inputX;
    private float inputZ;
    private bool jumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        if (footstepAudioSource != null)
        {
            footstepAudioSource.clip = footstepClip;
            footstepAudioSource.loop = true;
            footstepAudioSource.playOnAwake = false;
        }

        if (sfxAudioSource != null)
        {
            sfxAudioSource.loop = false;
            sfxAudioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (moveAction != null)
        {
            Vector2 inputMovement = moveAction.ReadValue<Vector2>();
            inputX = inputMovement.x;
            inputZ = inputMovement.y;
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 inputRightDirection = cameraTransform.right;
        Vector3 inputForwardDirection = cameraTransform.forward;

        inputRightDirection.y = 0f;
        inputForwardDirection.y = 0f;

        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        characterMovement =
            inputRightDirection * inputX * moveSpeed * Time.fixedDeltaTime +
            inputForwardDirection * inputZ * moveSpeed * Time.fixedDeltaTime;

        Vector3 moveDirection =
            inputRightDirection * inputX +
            inputForwardDirection * inputZ;

        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
        }

        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (jumpClip != null && sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(jumpClip, 1f);
            }
        }

        GetPlatformVelocity();

        velocity.y += gravity * Time.fixedDeltaTime;

        Vector3 verticalMovement = new Vector3(0f, velocity.y * Time.fixedDeltaTime, 0f);
        Vector3 combinedMovement = characterMovement + verticalMovement;

        if (isGrounded)
        {
            combinedMovement += platformVelocity * Time.fixedDeltaTime;
        }

        controller.Move(combinedMovement);

        SetAnimationState();
        HandleFootstepAudio();

        jumpPressed = false;
    }



    private void HandleFootstepAudio()
    {
        if (footstepAudioSource == null || footstepClip == null)
        {
            return;
        }

        bool isMoving = new Vector2(inputX, inputZ).sqrMagnitude > 0.01f;
        bool shouldPlayFootsteps = isGrounded && isMoving;

        if (shouldPlayFootsteps && !footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play();
        }
        else if (!shouldPlayFootsteps && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }
    }

    private void GetPlatformVelocity()
    {
        platformVelocity = Vector3.zero;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastLength))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            {
                MovingPlatform movingPlatform = hit.collider.GetComponent<MovingPlatform>();

                if (movingPlatform != null)
                {
                    platformVelocity = movingPlatform.GetVelocity();
                }
            }
        }
    }

    private void SetAnimationState()
    {
        if (animator == null)
        {
            return;
        }

        Vector2 inputMovement = new Vector2(inputX, inputZ);

        animator.SetBool("IsJumping", velocity.y > 0.1f || !controller.isGrounded);
        animator.SetBool("IsRunning", inputMovement.sqrMagnitude > 0.01f);
        animator.SetFloat("MovementForward", Mathf.Clamp01(inputMovement.magnitude));
    }
}
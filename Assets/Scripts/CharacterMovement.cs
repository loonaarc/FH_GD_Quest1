using UnityEngine;
// 🧠 Gives access to Unity classes like MonoBehaviour, Vector3, CharacterController, Physics, RaycastHit, etc.
// ⚙️ "using" imports the UnityEngine namespace so we can write Vector3 instead of UnityEngine.Vector3.

public class CharacterMovement : MonoBehaviour
// 🧠 This script controls the player character:
//    - reads input
//    - moves left/right/forward/back
//    - applies gravity
//    - allows jumping
//    - adds movement from a moving platform underneath the player
// ⚙️ "public class CharacterMovement" declares a class named CharacterMovement.
// ⚙️ ": MonoBehaviour" means it inherits from Unity's MonoBehaviour base class,
//    so Unity can call methods like Awake(), Update(), and FixedUpdate().
{
    [SerializeField] private float moveSpeed = 5f;
    // 🧠 How fast the character moves horizontally.
    // ⚙️ float = decimal number type.
    // ⚙️ private = only this class can access it directly in code.
    // ⚙️ [SerializeField] lets it still appear in the Unity Inspector.

    [SerializeField] private float gravity = -9.81f;
    // 🧠 Downward acceleration applied every physics step.
    // ⚙️ Negative because downward is usually the negative y direction.

    [SerializeField] private float jumpHeight = 1.5f;
    // 🧠 Desired jump height.
    // ⚙️ Used in the jump velocity formula later.

    [SerializeField] private float raycastLength = 1.2f;
    // 🧠 How far downward we check for a moving platform below the player.
    // ⚙️ A raycast is like an invisible line shot into the world.

    private CharacterController controller;
    // 🧠 Reference to the CharacterController component on this player.
    // ⚙️ CharacterController is Unity's built-in component for capsule-like player movement.

    private Vector3 velocity;
    // 🧠 Stores the player's current velocity, mainly the vertical part for gravity/jumping.
    // ⚙️ Vector3 has x, y, z components.

    private Vector3 characterMovement;
    // 🧠 Stores the player's own horizontal movement for this physics step.
    // ⚙️ This is the movement from keyboard input, not from the platform.

    private Vector3 platformVelocity;
    // 🧠 Stores the movement speed/direction of the platform under the player.
    // ⚙️ We get this from the MovingPlatform script.

    private float inputX;
    // 🧠 Stores horizontal input value from player input.
    // ⚙️ Usually A/D or left/right arrow keys.
    // ⚙️ Typically between -1 and 1.

    private float inputZ;
    // 🧠 Stores vertical/forward-backward input value.
    // ⚙️ Usually W/S or up/down arrow keys.
    // ⚙️ Also typically between -1 and 1.

    private bool jumpPressed;
    // 🧠 Remembers whether jump was pressed.
    // ⚙️ bool = true or false.
    // ⚙️ Important because input is read in Update() but movement happens in FixedUpdate().

    private bool isGrounded;
    // 🧠 Stores whether the character is currently touching the ground.
    // ⚙️ We read this from the CharacterController.

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        // 🧠 Gets the CharacterController attached to the same GameObject as this script.
        // ⚙️ GetComponent<T>() searches the GameObject for a component of type T.
        // ⚙️ Here T = CharacterController.
    }

    private void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        // 🧠 Reads left/right input every rendered frame.
        // ⚙️ Input.GetAxis("Horizontal") usually returns:
        //    -1 for left, 0 for no input, 1 for right,
        //    and can also return in-between smoothed values.

        inputZ = Input.GetAxis("Vertical");
        // 🧠 Reads forward/backward input every rendered frame.
        // ⚙️ Same idea as Horizontal, but for forward/backward.

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        // 🧠 If jump was pressed this frame, remember it.
        // ⚙️ GetButtonDown returns true only on the frame the button was initially pressed.
        // ⚙️ We store it in jumpPressed so FixedUpdate() can use it later.
    }

    private void FixedUpdate()
    {
        isGrounded = controller.isGrounded;
        // 🧠 Check if the CharacterController considers the player grounded.
        // ⚙️ controller.isGrounded is a built-in property.

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // 🧠 If we're on the ground and still moving downward, clamp the downward velocity
        //    to a small negative value.
        // ⚙️ This helps the controller stay "stuck" to the ground instead of hovering.
        // ⚙️ We use -2f, not 0, because a tiny downward force tends to make grounding more stable.

        Vector3 move = new Vector3(inputX, 0f, inputZ);
        // 🧠 Build a movement direction from player input.
        // ⚙️ x = left/right input
        // ⚙️ y = 0 because this vector is only for horizontal movement
        // ⚙️ z = forward/backward input

        characterMovement = move * moveSpeed * Time.fixedDeltaTime;
        // 🧠 Convert the input direction into actual horizontal movement for this physics step.
        // ⚙️ moveSpeed = units per second
        // ⚙️ Time.fixedDeltaTime = duration of one physics step
        // ⚙️ Multiplying by fixedDeltaTime makes movement framerate-independent in physics.

        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        // 🧠 If jump was pressed and the player is on the ground, start a jump.
        // ⚙️ This sets the initial upward velocity.
        // ⚙️ Mathf.Sqrt(...) calculates the square root.
        // ⚙️ Formula comes from physics so the jump roughly reaches jumpHeight.

        GetPlatformVelocity();
        // 🧠 Check whether we're standing on a moving platform,
        //    and if yes, get that platform's velocity.

        velocity.y += gravity * Time.fixedDeltaTime;
        // 🧠 Apply gravity to vertical velocity.
        // ⚙️ gravity is acceleration
        // ⚙️ multiplying by fixedDeltaTime converts acceleration into change in velocity for this step

        Vector3 verticalMovement = new Vector3(0f, velocity.y * Time.fixedDeltaTime, 0f);
        // 🧠 Convert vertical velocity into actual vertical movement for this physics step.
        // ⚙️ x and z are 0 because this vector is only vertical movement.
        // ⚙️ velocity.y * fixedDeltaTime = distance moved vertically this step.

        Vector3 combinedMovement = characterMovement + verticalMovement;
        // 🧠 Start with the player's own movement:
        //    horizontal movement from input + vertical movement from gravity/jump.
        // ⚙️ Vector3 addition adds x to x, y to y, z to z.

        if (isGrounded)
        {
            combinedMovement += platformVelocity * Time.fixedDeltaTime;
        }
        // 🧠 If the player is grounded, also add the movement of the platform underneath them.
        // ⚙️ platformVelocity is in units per second,
        //    so we multiply by fixedDeltaTime to convert it into movement for this step.
        // ⚙️ This is what makes the player move together with the platform.
        // ⚙️ Using isGrounded is better than !jumpPressed here:
        //    - if you are standing on the platform, you move with it
        //    - if you are in the air, you don't
        //    - if you just pressed jump, you're still grounded only very briefly, then you leave it

        controller.Move(combinedMovement);
        // 🧠 Actually move the player by the total movement we calculated this physics step.
        // ⚙️ CharacterController.Move expects a movement delta (distance to move now),
        //    not a velocity.

        jumpPressed = false;
        // 🧠 Reset the jump flag so one button press only triggers one jump.
        // ⚙️ Next jump requires a new Input.GetButtonDown in Update().
    }

    private void GetPlatformVelocity()
    {
        platformVelocity = Vector3.zero;
        // 🧠 Default assumption: no moving platform underneath us.
        // ⚙️ Vector3.zero means (0, 0, 0).

        RaycastHit hit;
        // 🧠 This variable will store information if the raycast hits something.
        // ⚙️ RaycastHit is a struct containing hit object, point, normal, collider, etc.

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastLength))
        {
            // 🧠 Shoot a ray straight downward from the player's position.
            //    If it hits something within raycastLength, this if-block runs.
            //
            // ⚙️ Physics.Raycast(origin, direction, out hit, maxDistance)
            //    origin = transform.position
            //    direction = Vector3.down
            //    out hit = write hit info into variable "hit"
            //    maxDistance = raycastLength
            //
            // ⚙️ "out hit" means the method fills the variable with extra information.

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            {
                // 🧠 Check whether the thing we hit is on the "Platforms" layer.
                // ⚙️ Layers are Unity's way of categorizing objects.
                // ⚙️ LayerMask.NameToLayer("Platforms") gives the integer id of that layer.
                // ⚙️ hit.collider.gameObject.layer gives the layer number of the hit object.

                MovingPlatform movingPlatform = hit.collider.GetComponent<MovingPlatform>();
                // 🧠 Try to get the MovingPlatform script from the object we hit.
                // ⚙️ If the object has that component, we can ask it for its velocity.

                if (movingPlatform != null)
                {
                    platformVelocity = movingPlatform.GetVelocity();
                }
                // 🧠 If the platform actually has a MovingPlatform script,
                //    store its current velocity.
                // ⚙️ If not, platformVelocity stays zero.
            }
        }
    }
}
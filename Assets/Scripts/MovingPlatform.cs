using UnityEngine;
// 🧠 Gives access to Unity classes (Vector3, Time, Mathf, etc.)

public class MovingPlatform : MonoBehaviour
// 🧠 Platform that:
//    - moves between start and end
//    - can be turned on/off
//    - remembers its own progress
//    - calculates velocity
{
    [SerializeField] private float platformSpeed = 1f;
    // 🧠 Speed in units per second

    [SerializeField] private Vector3 start;
    // 🧠 Start position

    [SerializeField] private Vector3 end;
    // 🧠 End position

    [SerializeField] private bool startActive = false;
    // 🧠 Should platform start moving immediately?

    private Vector3 lastPosition;
    // 🧠 Position from previous physics step

    private Vector3 velocity;
    // 🧠 Current velocity (units per second)

    private bool isMoving;
    // 🧠 Whether platform is currently moving

    private float progress = 0f;
    // 🧠 Where we are between start and end
    // ⚙️ Range: 0 → 1
    // 0 = start
    // 1 = end

    private int direction = 1;
    // 🧠 Movement direction
    // ⚙️ 1 = forward (start → end)
    // ⚙️ -1 = backward (end → start)

    private void Start()
    {
        isMoving = startActive;
        // 🧠 Set initial movement state

        transform.localPosition = start;
        // 🧠 Ensure platform starts exactly at "start"

        lastPosition = transform.localPosition;
        // 🧠 Initialize lastPosition for velocity calculation
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // 🧠 Only update movement if platform is active

            float distance = Vector3.Distance(start, end);
            // 🧠 Total distance between start and end
            // ⚙️ Distance formula internally

            if (distance > 0.001f)
            {
                // 🧠 Avoid division by zero or tiny values

                progress += direction * (platformSpeed / distance) * Time.fixedDeltaTime;
                // 🧠 Update progress based on speed and direction

                // ⚙️ platformSpeed = units per second
                // ⚙️ divide by distance → converts speed into "progress per second"
                // ⚙️ multiply by deltaTime → progress change this frame

                if (progress >= 1f)
                {
                    progress = 1f;
                    direction = -1;
                }
                // 🧠 If we reached the end:
                //    clamp to 1 and reverse direction

                else if (progress <= 0f)
                {
                    progress = 0f;
                    direction = 1;
                }
                // 🧠 If we reached the start:
                //    clamp to 0 and reverse direction

                transform.localPosition = Vector3.Lerp(start, end, progress);
                // 🧠 Move platform based on progress
                // ⚙️ Lerp converts progress (0–1) into position
            }
        }

        velocity = (transform.localPosition - lastPosition) / Time.fixedDeltaTime;
        // 🧠 Calculate velocity

        lastPosition = transform.localPosition;
        // 🧠 Store position for next frame
    }

    public Vector3 GetVelocity()
    {
        return velocity;
        // 🧠 Allows player script to read platform velocity
    }

    public void SetActiveMovement(bool active)
    {
        isMoving = active;
        // 🧠 External control (lever)
    }

    public bool IsMoving()
    {
        return isMoving;
        // 🧠 Allows other scripts to check if platform is moving
    }
}
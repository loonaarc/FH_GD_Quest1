using UnityEngine;
// 🧠 Gives access to Unity features (Transform, Quaternion, Input, etc.)

public class Lever : MonoBehaviour
// 🧠 Interactive lever that:
//    - detects player proximity
//    - toggles a platform
//    - updates its visual state (position + rotation)
{
    [SerializeField] private MovingPlatform targetPlatform;
    // 🧠 Platform this lever controls

    [SerializeField] private Transform leverHandle;
    // 🧠 The visual part that moves (usually a child object)

    [SerializeField] private Vector3 offLocalPosition;
    // 🧠 Position of handle when lever is OFF (relative to parent)

    [SerializeField] private Vector3 onLocalPosition;
    // 🧠 Position of handle when lever is ON

    [SerializeField] private Vector3 offLocalRotation;
    // 🧠 Rotation of handle when OFF (Euler angles)

    [SerializeField] private Vector3 onLocalRotation;
    // 🧠 Rotation of handle when ON

    private bool playerInRange = false;
    // 🧠 Whether player is inside trigger

    private bool isActivated = false;
    // 🧠 Current state of lever (ON/OFF)

    private void Start()
    {
        UpdateLeverVisual();
        // 🧠 Ensure visual matches logical state at game start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = true;
        }
    }
    // 🧠 Player enters interaction zone → can press E

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = false;
        }
    }
    // 🧠 Player leaves → cannot interact anymore

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isActivated = !isActivated;
            // 🧠 Toggle state

            if (targetPlatform != null)
            {
                targetPlatform.SetActiveMovement(isActivated);
            }
            // 🧠 Tell platform to start/stop

            UpdateLeverVisual();
            // 🧠 Update lever appearance
        }
    }

    private void UpdateLeverVisual()
    {
        if (leverHandle != null)
        {
            if (isActivated)
            {
                leverHandle.localPosition = onLocalPosition;
                // 🧠 Move handle to ON position

                leverHandle.localRotation = Quaternion.Euler(onLocalRotation);
                // 🧠 Rotate handle to ON rotation
            }
            else
            {
                leverHandle.localPosition = offLocalPosition;
                // 🧠 Move handle to OFF position

                leverHandle.localRotation = Quaternion.Euler(offLocalRotation);
                // 🧠 Rotate handle to OFF rotation
            }
        }
    }
}
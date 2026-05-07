using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    [SerializeField] private MovingPlatform targetPlatform;
    [SerializeField] private Transform leverHandle;

    [SerializeField] private Vector3 offLocalPosition;
    [SerializeField] private Vector3 onLocalPosition;

    [SerializeField] private Vector3 offLocalRotation;
    [SerializeField] private Vector3 onLocalRotation;

    private bool playerInRange = false;
    private bool isActivated = false;

    private InputAction interactAction;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Start()
    {
        UpdateLeverVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = false;
        }
    }

    private void FixedUpdate()
    {
        if (playerInRange && interactAction != null && interactAction.WasPressedThisFrame())
        {
            isActivated = !isActivated;

            if (targetPlatform != null)
            {
                targetPlatform.SetActiveMovement(isActivated);
            }

            UpdateLeverVisual();
        }
    }

    private void UpdateLeverVisual()
    {
        if (leverHandle == null)
        {
            return;
        }

        if (isActivated)
        {
            leverHandle.localPosition = onLocalPosition;
            leverHandle.localRotation = Quaternion.Euler(onLocalRotation);
        }
        else
        {
            leverHandle.localPosition = offLocalPosition;
            leverHandle.localRotation = Quaternion.Euler(offLocalRotation);
        }
    }
}
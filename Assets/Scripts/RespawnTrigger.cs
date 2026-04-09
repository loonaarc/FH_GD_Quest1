using UnityEngine;
// 🧠 Gives access to Unity features (Transform, Collider, etc.)
// ⚙️ Imports UnityEngine namespace

public class RespawnTrigger : MonoBehaviour
// 🧠 Script that teleports the player back to a respawn point when they enter a trigger
// ⚙️ MonoBehaviour → allows Unity event functions like OnTriggerEnter
{
    [SerializeField] private Transform respawnPoint;
    // 🧠 The position where the player will be teleported to
    // ⚙️ Transform = position, rotation, scale of an object
    // ⚙️ We store the Transform instead of Vector3 → more flexible (can move it in scene)

    private void OnTriggerEnter(Collider other)
    // 🧠 Called automatically when something enters this trigger collider
    // ⚙️ Must have:
    //     - this object: Collider with "Is Trigger" enabled
    //     - other object: Collider + usually CharacterController / Rigidbody

    // ⚙️ "Collider other" = the object that entered the trigger
    {
        CharacterController controller = other.gameObject.GetComponent<CharacterController>();
        // 🧠 Check if the entering object is the player (has CharacterController)

        // ⚙️ other.gameObject → gets the GameObject of the collider
        // ⚙️ GetComponent<CharacterController>():
        //     tries to find that component on the object
        //     returns null if not found

        if (controller != null)
        // 🧠 If the object IS a player (or anything with CharacterController)
        // ⚙️ null = "nothing found"
        {
            Respawn(controller);
            // 🧠 Call respawn logic
        }
    }

    private void Respawn(CharacterController controller)
    // 🧠 Handles teleporting the player
    // ⚙️ private = only used inside this script
    {
        controller.enabled = false;
        // 🧠 Temporarily disable the CharacterController

        // ⚠️ VERY IMPORTANT:
        // If you don’t disable it, Unity may block movement due to collisions
        // → teleport might fail or behave weirdly

        controller.transform.position = respawnPoint.position;
        // 🧠 Move player to respawn location

        // ⚙️ controller.transform → access the GameObject’s Transform
        // ⚙️ position = world position
        // ⚙️ respawnPoint.position = position of assigned object

        controller.enabled = true;
        // 🧠 Re-enable controller so player can move again
    }
}
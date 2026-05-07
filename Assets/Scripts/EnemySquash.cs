using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySquash : MonoBehaviour
{
    [SerializeField] private float squashDuration = 0.15f;
    [SerializeField] private float disappearDelay = 0.25f;
    [SerializeField] private AudioClip squashClip;

    private bool isSquashed = false;
    private Vector3 originalScale;
    private AudioSource audioSource;

    private void Awake()
    {
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy trigger touched by: " + other.name);

        if (isSquashed)
        {
            return;
        }

        CharacterMovement player = other.GetComponentInParent<CharacterMovement>();

        if (player == null)
        {
            Debug.Log("Touched object is not player");
            return;
        }

        Debug.Log("Player touched enemy");

        bool playerIsAboveEnemy = other.transform.position.y > transform.position.y ;

        if (playerIsAboveEnemy)
        {
            Debug.Log("Player is above enemy");
            StartCoroutine(SquashAndDisappear());
        }
    }

    private IEnumerator SquashAndDisappear()
    {
        isSquashed = true;

        if (squashClip != null)
        {
            audioSource.PlayOneShot(squashClip);
        }

        Vector3 squashedScale = new Vector3(
            originalScale.x * 1.2f,
            originalScale.y * 0.25f,
            originalScale.z * 1.2f
        );

        float timer = 0f;

        while (timer < squashDuration)
        {
            float t = timer / squashDuration;
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = squashedScale;

        yield return new WaitForSeconds(disappearDelay);

        gameObject.SetActive(false);
    }
}
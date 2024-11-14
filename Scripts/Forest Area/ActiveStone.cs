using UnityEngine;

public class ActiveStone : MonoBehaviour
{
    public OpenStoneGate openStoneGate;
    public float activationDistance = 3f;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private AudioSource activationSound;
    [SerializeField] private CursorController cursorController; // Reference to the CursorController
    [SerializeField] private GameObject particleEffect; // Particle effect to activate

    private bool isActivated = false; // To check if the object has already been activated

    private void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (cursorController == null)
        {
            cursorController = GameObject.FindObjectOfType<CursorController>();
        }

        // Ensure the particle effect is initially disabled
        if (particleEffect != null)
        {
            particleEffect.SetActive(false);
        }
    }

    private void Update()
    {
        if (isActivated) return; // Prevent multiple activations

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer <= activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (cursorController != null && cursorController.IsCursorInteractable)
            {
                Activate();
            }
        }
    }

    private void Activate()
    {
        isActivated = true; // Mark as activated

        // Play the activation sound effect
        if (activationSound != null)
        {
            activationSound.Play();
        }

        // Activate the particle effect
        if (particleEffect != null)
        {
            particleEffect.SetActive(true);
        }

        // Notify the OpenStoneGate script
        openStoneGate.ActivateObject(gameObject);
        Debug.Log("Activated");
    }

    public void ResetActivation()
    {
        isActivated = false; // Allow reactivation
        Invoke("DisableParticle", 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }

    private void DisableParticle()
    {
        if (particleEffect != null)
        {
            particleEffect.SetActive(false); // Turn off the particle effect
        }
    }
}

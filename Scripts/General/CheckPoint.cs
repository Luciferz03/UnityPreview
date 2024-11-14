using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [Tooltip("Unique key to identify this checkpoint. Use a unique name for each checkpoint.")]
    public string checkpointKey; // Customizable key for each checkpoint

    [Tooltip("The distance within which the player can activate the checkpoint.")]
    public float activationDistance = 3f; // Customizable activation distance

    [Header("References")]
    [Tooltip("Player's Transform component.")]
    [SerializeField] private Transform playerTransform; // Reference to the player

    [Tooltip("Sound to play when checkpoint is activated.")]
    [SerializeField] private AudioSource activationSound; // Customizable sound effect

    [Tooltip("Reference to the CursorController (optional).")]
    [SerializeField] private CursorController cursorController; // Reference to the cursor controller (optional)

    [Tooltip("Particle effect to activate when checkpoint is triggered.")]
    [SerializeField] private GameObject particleEffect; // Customizable particle effect

    [Header("Optional Settings")]
    [Tooltip("If checked, this checkpoint can only be activated once.")]
    public bool singleActivation = true; // Option for single or multiple activations

    private bool isActivated = false; // Track activation status

    private void Start()
    {
        // Check if the player has previously saved progress at this checkpoint
        LoadPlayerPosition();

        // Load the checkpoint's activation status using the checkpointKey
        isActivated = PlayerPrefs.GetInt(checkpointKey, 0) == 1;

        if (isActivated && particleEffect != null)
        {
            particleEffect.SetActive(true);
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        // Check if the checkpoint is within activation range and the player presses "E"
        if (!isActivated && distanceToPlayer <= activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (cursorController == null || (cursorController != null && cursorController.IsCursorInteractable))
            {
                ActivateCheckpoint();
            }
        }
    }

    private void ActivateCheckpoint()
    {
        if (singleActivation)
        {
            isActivated = true;
            PlayerPrefs.SetInt(checkpointKey, 1); // Save the activation status in PlayerPrefs
        }

        // Play the particle effect if assigned
        if (particleEffect != null)
        {
            particleEffect.SetActive(true);
        }

        // Play the sound effect if assigned
        if (activationSound != null)
        {
            activationSound.Play();
        }

        // Save the player's progress
        SaveProgress();

        // Set this as the new checkpoint
        SetCheckpoint();

        Debug.Log("Checkpoint activated! Progress saved.");
    }

    private void SaveProgress()
    {
        // Save the player's position
        Vector3 playerPosition = playerTransform.position;
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
        PlayerPrefs.Save();

        Debug.Log("Saved Player Position: X = " + PlayerPrefs.GetFloat("PlayerPosX") +
          ", Y = " + PlayerPrefs.GetFloat("PlayerPosY") +
          ", Z = " + PlayerPrefs.GetFloat("PlayerPosZ"));

    }

    private void SetCheckpoint()
    {
        // Implement your checkpoint system logic
        RespawnController.Instance.SetCheckpoint(transform.position);
    }

    public void LoadPlayerPosition()
    {
            // Load player's position from saved data
            float posX = PlayerPrefs.GetFloat("PlayerPosX", playerTransform.position.x);
            float posY = PlayerPrefs.GetFloat("PlayerPosY", playerTransform.position.y);
            float posZ = PlayerPrefs.GetFloat("PlayerPosZ", playerTransform.position.z);

            // Disable the CharacterController before setting the position
            playerTransform.GetComponent<CharacterController>().enabled = false;

            // Move the player to the saved position
            playerTransform.position = new Vector3(posX, posY, posZ);

            // Reset the velocity if needed
            PlayerMovement playerMovement = playerTransform.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.velocity.y = 0f; // Reset vertical velocity
            }

            // Re-enable the CharacterController after setting the position
            playerTransform.GetComponent<CharacterController>().enabled = true;

            // Optional: Freeze movement for a short time after loading (if needed)
            if (playerMovement != null)
            {
                StartCoroutine(playerMovement.DisableMovementTemporarily(0.2f)); // Brief freeze (0.2 seconds)
            }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }

 

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{

    public float detectionDistance = 3f; // Distance within which the player can detect interactable objects
    public float viewAngleThreshold = 30f; // Angle threshold for detecting objects within the player's view
    public Color defaultCursorColor = Color.white; // Default cursor color
    public Color interactCursorColor = Color.green; // Cursor color when an interactable object is detected
    public bool IsCursorInteractable;


    [SerializeField] private Transform playerTransform; // Reference to the player transform
    public string interactableTag = "Interactable"; // Tag used to identify interactable objects

    [SerializeField] private Image cursorImage; // Reference to the Image component representing the cursor

    private void Start()
    {
        // Set the initial cursor color
        SetCursorColor(defaultCursorColor);
    }

    private void Update()
    {
        DetectInteractableObject();
    }

    private void DetectInteractableObject()
    {
        // Draw the raycast in the Scene view for debugging
        //Debug.DrawRay(playerTransform.position, playerTransform.forward * detectionDistance, Color.red);

        // Raycast from the player's position forward
        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, playerTransform.forward, out hit, detectionDistance))
        {
            // Calculate the angle between the player's forward direction and the direction to the hit point
            Vector3 directionToHit = (hit.point - playerTransform.position).normalized;
            float angleToHit = Vector3.Angle(playerTransform.forward, directionToHit);

            // Check if the object is within the view angle threshold and has the correct tag
            if (angleToHit <= viewAngleThreshold && hit.collider.CompareTag(interactableTag))
            {
                SetCursorColor(interactCursorColor);
                IsCursorInteractable = true;
                return;
            }

        }
        SetCursorColor(defaultCursorColor);
        IsCursorInteractable = false;
    }

    private void SetCursorColor(Color color)
    {
        // Set the color of the Image component representing the cursor
        if (cursorImage != null)
        {
            cursorImage.color = color;
        }
    }
}

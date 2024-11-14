using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [Tooltip("Unique key to identify this tutorial. Use a unique name for each tutorial area.")]
    public string tutorialKey = "Tutorial_1"; // Unique key for each tutorial
    public GameObject tutorialUI; // Reference to the tutorial UI

    [Header("Trigger Settings")]
    [Tooltip("If true, the game will pause when the tutorial UI is shown.")]
    public bool pauseGameOnShow = true; // Option to pause the game when the tutorial is shown

    private bool hasShownTutorial;

    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        // Load the saved state of the tutorial (0 = not shown, 1 = shown)
        hasShownTutorial = PlayerPrefs.GetInt(tutorialKey, 0) == 1;

        // Ensure the tutorial UI is initially disabled
        if (tutorialUI != null)
        {
            tutorialUI.SetActive(false);
        }

        // Hide the cursor at the start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasShownTutorial)
        {
            ShowTutorial();
            playerMovement.ontutorial = true;
        }
    }

    private void ShowTutorial()
    {
        if (tutorialUI != null)
        {
            tutorialUI.SetActive(true);
            hasShownTutorial = true;

            // Show the cursor when the tutorial is shown
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Save the tutorial state so it doesn't show again
            PlayerPrefs.SetInt(tutorialKey, 1);

            if (pauseGameOnShow)
            {
                Time.timeScale = 0f; // Pause the game
            }
        }
    }

    public void CloseTutorial()
    {
        if (tutorialUI != null)
        {

            tutorialUI.SetActive(false);

            // Hide the cursor when the tutorial is closed
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerMovement.ontutorial = false;

            if (pauseGameOnShow)
            {
                Time.timeScale = 1f; // Resume the game

            }
        }
    }

    // Call this method from the menu to show the tutorial again
    public void OpenTutorialFromMenu()
    {
        if (tutorialUI != null)
        {
            tutorialUI.SetActive(true);

            // Show the cursor when the tutorial is opened from the menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (pauseGameOnShow)
            {
                Time.timeScale = 0f; // Optionally pause the game again
            }
        }
    }
}

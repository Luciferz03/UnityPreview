using System.Collections.Generic;
using UnityEngine;

public class OpenStoneGate : MonoBehaviour
{
    [SerializeField] private List<ActiveStone> objectsToActivateInOrder; // Use ActiveStone instead of GameObject
    [SerializeField] private Animator stoneGateForestAnimator;
    [SerializeField] private string stoneGateAnimationName;

    public AudioSource activationGateSound; // Sound effect to play on activation

    private int currentIndex = 0;

    private void Start()
    {
        // Ensure the list is not empty and has the correct number of objects
        if (objectsToActivateInOrder == null || objectsToActivateInOrder.Count != 6)
        {
            Debug.LogError("Please assign exactly 6 ActiveStone objects to the list.");
        }
    }

    public void ActivateObject(GameObject activatedObject)
    {
        // Check if the activated object is the correct one in the sequence
        if (objectsToActivateInOrder[currentIndex].gameObject == activatedObject)
        {
            currentIndex++;

            // If the sequence is complete, trigger the door animation
            if (currentIndex >= objectsToActivateInOrder.Count)
            {
                Invoke("OpenStoneGates", 2f);
            }
        }
        else
        {
            // Reset the sequence if the wrong object is activated
            Debug.Log("Wrong object activated. Resetting sequence.");
            ResetAllStones();
            currentIndex = 0;
        }
    }

    private void ResetAllStones()
    {
        foreach (ActiveStone stone in objectsToActivateInOrder)
        {
            stone.ResetActivation();
        }
    }

    private void OpenStoneGates()
    {
        if (stoneGateForestAnimator != null)
        {
            stoneGateForestAnimator.SetBool(stoneGateAnimationName, true);
        }

        if (activationGateSound != null)
        {
            activationGateSound.Play();
        }

        Debug.Log("Stone gate opened!");
    }
}

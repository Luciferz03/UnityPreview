using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public static RespawnController Instance;

    private Vector3 checkpointPosition;

    private void Awake()
    {
        // Implement the Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = checkpointPosition;
        // Reset player health or other stats if necessary
    }
}
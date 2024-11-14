using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleFloatingObstacle : MonoBehaviour
{
    [Header("Floating Movement Settings")]
    public float amplitude = 0.5f;  // The maximum height difference (up and down distance)
    public float frequency = 1f;    // Speed of the floating movement (oscillation frequency)
    private Vector3 startPos;        // The starting position of the object

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the object
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FloatUpAndDown();
    }

    // Function to handle the up and down movement
    void FloatUpAndDown()
    {
        // Calculate the new position using a sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the new position while keeping the x and z axes unchanged
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}

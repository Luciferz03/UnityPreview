using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovementObstacle : MonoBehaviour
{
    [Header("Square Movement Settings")]
    public float sideLength = 5f;     // Length of each side of the square
    public float speed = 2f;          // Speed of movement between points

    private Vector3[] squarePoints;   // The four corners of the square
    private int currentPointIndex = 0; // Current target point index
    private Vector3 targetPoint;      // The current target point
    private bool initialized = false; // To ensure square points are set

    // Start is called before the first frame update
    void Start()
    {
        // Use the object's current position as the starting point
        InitializeSquare(transform.position);
        targetPoint = squarePoints[currentPointIndex]; // Set the initial target point
    }

    // Initialize the square points based on the object's initial position and sideLength
    void InitializeSquare(Vector3 startPoint)
    {
        if (!initialized)
        {
            // Calculate the four corners of the square
            squarePoints = new Vector3[4];
            squarePoints[0] = startPoint; // Bottom-left corner
            squarePoints[1] = startPoint + new Vector3(sideLength, 0, 0); // Bottom-right corner
            squarePoints[2] = startPoint + new Vector3(sideLength, 0, sideLength); // Top-right corner
            squarePoints[3] = startPoint + new Vector3(0, 0, sideLength); // Top-left corner

            initialized = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveInSquare();
    }

    void MoveInSquare()
    {
        // Move towards the current target point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // If we reach the target point, move to the next one
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % squarePoints.Length; // Cycle to the next point
            targetPoint = squarePoints[currentPointIndex]; // Update the target point
        }
    }
}

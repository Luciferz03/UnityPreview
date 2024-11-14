using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovementObstacle : MonoBehaviour
{
    [Header("Circular Movement Settings")]
    public float radius = 5f;          // The radius of the circle
    public float speed = 2f;           // The speed of rotation (in radians per second)
    public Vector3 rotationAxis = Vector3.up;  // The axis around which the object will move (up = Y axis by default)

    private Vector3 centerPosition;    // The center point of the circle
    private float angle;               // Current angle of the object in radians

    // Start is called before the first frame update
    void Start()
    {
        // Set the center position to the object's initial position
        centerPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update the angle over time (don't multiply by Time.deltaTime in FixedUpdate)
        angle += speed * Time.fixedDeltaTime;

        // Calculate the new position using trigonometry
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Vector3 offset = new Vector3(x, 0, z);

        // Rotate the offset to respect the custom rotation axis
        offset = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, rotationAxis) * offset;

        // Set the new position of the object
        transform.position = centerPosition + offset;
    }
}
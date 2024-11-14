using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    [SerializeField] bool Left, Right;
    [SerializeField] float rotationSpeed = 50f; // Adjust the rotation speed

    private void FixedUpdate()
    {
        if (Left)
        {
            // Rotate left (counter-clockwise) on the y-axis around its local pivot
            transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime, Space.Self);
        }
        if (Right)
        {
            // Rotate right (clockwise) on the y-axis around its local pivot
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}

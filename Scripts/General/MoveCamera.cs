using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SlimUI.ModernMenu.UISettingsManager;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    private void FixedUpdate()
    {
        transform.position = cameraPosition.position;
    }
}

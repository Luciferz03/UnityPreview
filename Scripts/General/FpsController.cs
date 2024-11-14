using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsController : MonoBehaviour
{
    [Header("Default Target Frame Rate")]
    public int defaultTargetFPS = 60; // Default target FPS, in case user hasn't set it via settings

    void Start()
    {
        // Apply the default target FPS at the start of the game
        SetTargetFPS(defaultTargetFPS);
    }

    // Method to set the target FPS dynamically
    public void SetTargetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        Debug.Log("Target FPS set to: " + fps);
    }

    // This method could be called from your settings UI
    public void OnFPSSettingChanged(int selectedFPS)
    {
        SetTargetFPS(selectedFPS);
    }
}
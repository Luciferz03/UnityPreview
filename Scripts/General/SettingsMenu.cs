//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SettingsMenu : MonoBehaviour
//{
//    public Dropdown fpsDropdown; // Reference to the FPS dropdown in the UI
//    public FpsController fpsController; // Reference to the FPSController script

//    void Start()
//    {
//        // Load the saved FPS setting
//        int savedFPS = PlayerPrefs.GetInt("FPSSetting", 60); // Default to 60 if no setting is saved
//        fpsController.SetTargetFPS(savedFPS);

//        // Set the dropdown to reflect the saved value
//        fpsDropdown.value = fpsDropdown.options.FindIndex(option => option.text == savedFPS.ToString());
//    }

//    public void OnFPSDropdownChanged()
//    {
//        int selectedFPS = int.Parse(fpsDropdown.options[fpsDropdown.value].text);
//        fpsController.SetTargetFPS(selectedFPS);

//        // Save the FPS setting
//        PlayerPrefs.SetInt("FPSSetting", selectedFPS);
//    }
//}

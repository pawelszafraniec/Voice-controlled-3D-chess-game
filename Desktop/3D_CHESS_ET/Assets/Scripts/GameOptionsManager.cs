using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class handling game options
 */
public class GameOptionsManager : MonoBehaviour
{
	public Dropdown timeDropdown;
	public GameObject customOptions;
	public Text PlayerWhiteName;
	public Text PlayerBlackName;
	public Text CustomTimer;
	public Text CustomIncrement;
	public Toggle readMovesToggle;
	public Toggle cameraRotationToggle;

	public static string timeValue;
	public static string playerWhiteName;
	public static string playerBlackName;

	public static int customTimerValue;
	public static int customIncrementValue;

	public static bool VoiceReadMovesEnabled;
	public static bool CameraRotationEnabled;

	/**
	 * UPDATE method - runs on each frame of the game
	 */
	public void Update()
	{
		timeValue = timeDropdown.options[timeDropdown.value].text; // assign time value from dropdown
		playerWhiteName = PlayerWhiteName.text; // assign white player name from text field 
		playerBlackName = PlayerBlackName.text; // assign dark player name from text field

		VoiceReadMovesEnabled = readMovesToggle.isOn; // read voice reading system option
		CameraRotationEnabled = cameraRotationToggle.isOn; // read camera rotation option

		SetCustomOptions(); // set custom time options
	}

	/**
	 * Method showing additional input text field options for time
	 */
	public void EnableCustomOptions()
	{
		if(timeDropdown.options[timeDropdown.value].text == "Custom")
		{
			customOptions.SetActive(true);
		}
		else
		{
			customOptions.SetActive(false);
		}
	}

	/**
	 * Method setting up additional enabled time options
	 */
	public void SetCustomOptions()
	{
		if (timeDropdown.options[timeDropdown.value].text == "Custom")
		{
			if (!String.IsNullOrEmpty(CustomTimer.text))
				customTimerValue = int.Parse(CustomTimer.text);
			if (!String.IsNullOrEmpty(CustomIncrement.text))
				customIncrementValue = int.Parse(CustomIncrement.text);
		}
	}

}

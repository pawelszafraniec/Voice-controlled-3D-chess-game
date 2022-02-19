using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;


/*
 * Class responsible for handling controlling pieces with voice using DictationRecognizer class
 */
public class VoiceRecognition : MonoBehaviour
{
	public Image VoiceControl;
	public Text RecognizerReading;

	public static VoiceRecognition Instance { get; set; }
	private DictationRecognizer DictationRecognizer;

	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

	private char[] possible_numbers = { '1', '2', '3', '4', '5', '6', '7', '8' }; // 1-8
	private char[] possible_characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' }; // A-H
	
	public Errors errors_tables;

	//In order to use voice controlling system there must be a microphone device to be used for that purpose
	//System must be in english and phrase recognition system must be supported
	/**
	 * Control function checking environments conditions for handling voice recognition
	 */
	private bool DeviceControl()
	{
		foreach (var device in Microphone.devices)
		{
			Debug.Log("Name: " + device); // must exist
			if (Microphone.devices.Length == 0)
				return false;
		}

		if (Application.systemLanguage == SystemLanguage.English)
		{
			Debug.Log("This system is in English. ");
		}
		else
		{
			return false;
		}

		if (PhraseRecognitionSystem.isSupported)
		{
			Debug.Log("Phrase Recognition is supported!");
		}
		else
		{
			return false;
		}
		return true;
	}

	/**
	 * Restart function for Voice Recognition system
	 */
	public void RestartDictation()
	{
		DictationRecognizer.Stop();
		DictationRecognizer.Dispose();
		DictationRecognizer = new DictationRecognizer();
		DictationRecognizer.AutoSilenceTimeoutSeconds = 20f;
		DictationRecognizer.InitialSilenceTimeoutSeconds = 20f;
		DictationRecognizer.DictationResult += OnRecognizeSpeech;
		DictationRecognizer.Start();
		Debug.Log("RESTART");
	}

	/**
	 * START method - runs when script is being enabled
	 */
	public void Start()
	{
		Instance = this;

		if(DeviceControl())
		{
			DictationRecognizer = new DictationRecognizer();
			DictationRecognizer.AutoSilenceTimeoutSeconds = 20f;
			DictationRecognizer.InitialSilenceTimeoutSeconds = 20f;
			DictationRecognizer.DictationResult += OnRecognizeSpeech;
			DictationRecognizer.Start();
		}
	}

	/**
	 * UPDATE method - runs on each frame of the game
	 */
	public void Update()
	{
		// Change control for DictationRecognizer class control depending on its status
		if (DictationRecognizer.Status.Equals(SpeechSystemStatus.Running))
		{
			VoiceControl.GetComponent<Image>().color = Color.green;
		}
		else
		{
			VoiceControl.GetComponent<Image>().color = Color.red;
		}

	}

	/**
	 * DictationRecognizer method reading the speech handling moving the figure
	 */
	public void OnRecognizeSpeech(string speech, ConfidenceLevel level)
	{
		// assign recognized text
		string ab = speech.ToString();
		if(ab.Length < 4 || ab.Length > 7)
		{
			RecognizerReading.text = "Couldn't recognize the command. Try again.";
		}
		if (ab.Length > 4)
		{
			ab = ab.Replace(" ", "");
			ab = ValidateWords(ab);
		}

		//split speech string by half
		var first = ab.Substring(0, (int)(ab.Length / 2));
		var last = ab.Substring((int)(ab.Length / 2), (int)(ab.Length / 2));

		var startScreen = gameObject.GetComponent<StartScreen>(); // get StartScreen component 

		if (ab == "quit" || ab == "quitgame" || ab == "exit" || ab == "exitgame")
		{
			ChessBoardManager.Instance.popUpExitConfirmation.SetActive(true); // open exit confirmation pop-up window
		}
		if (ChessBoardManager.Instance.popUpExitConfirmation.activeInHierarchy == true) // exit confirmation pop-up opened
		{
			if (ab == "yes")
			{
				//Confirm exit pop-up
				startScreen.LoadStartScene();
			}
			if (ab == "no" || ab == "cancel")
			{
				//Cancel exit pop-up
				startScreen.DoNotExitGame();
			}
		}
		if (ChessBoardManager.Instance.popUpWindowForEndGame.activeInHierarchy == true) // end game pop-up opened
		{
			if (ab == "playagain")
			{
				//Reset game
				startScreen.LoadGameScene();
				RestartDictation();
				Time.timeScale = 1;

			}
		}

		if (first.Length == 2 && last.Length == 2)
		{
			// Lisp validation
			first = ValidateMovement(first, possible_characters, possible_numbers);
			last = ValidateMovement(last, possible_characters, possible_numbers);

			// Position dictionary for mapping spoken position to logic one, e.g. 00 -> A1
			var positionDictionary = ChessBoardManager.Instance.PreparePositionDictionary();

			// Read from the dictionary
			string aFind = positionDictionary.FirstOrDefault(x => x.Value == first).Key;
			string bFind = positionDictionary.FirstOrDefault(x => x.Value == last).Key;

			// Get corresponding coordinates in position array
			Tuple<int, int> helperX = StringHelper.Instance.GetBoardCoordinates(aFind);
			Tuple<int, int> helperY = StringHelper.Instance.GetBoardCoordinates(bFind);

			// Fill RecognizerReading text field
			RecognizerReading.text = "From:" + first + " to:" + last;

			// Execute move
			ExecuteMovement(helperX, helperY);
		}

	}

	/**
	* ExecuteMovement function - invoke SelectChessPiece() and MoveChessPiece() methods
	*/
	private void ExecuteMovement(Tuple<int, int> from, Tuple<int, int> to)
	{
		// Select piece - specify its allowed moves
		ChessBoardManager.Instance.SelectChessPiece(from.Item1, from.Item2);
		// Move the selected piece if target position is allowed
		ChessBoardManager.Instance.MoveChessPiece(to.Item1, to.Item2);
	}

	/**
	* Validate speech text in terms of similar words
	*/
	private string ValidateWords(string input)
	{
		string output = input;
		if(input.Length == 5) // e.g. ey3d5 - ey => a
		{
			string copied = Char.ToString(input[0]) + Char.ToString(input[1]);
			string rest = Char.ToString(input[2]) + Char.ToString(input[3]) + Char.ToString(input[4]);
			if (errors_tables.two_digits_errors_with_A_letter.Contains(copied))
			{
				char corrected = 'A';
				output = corrected + rest;
			}
		}
		else if(input.Length == 6) // e.g. eat3d5 - eat => a
		{
			string copied = Char.ToString(input[0]) + Char.ToString(input[1]) + Char.ToString(input[2]);
			string rest = Char.ToString(input[3]) + Char.ToString(input[4]) + Char.ToString(input[5]);
			if (errors_tables.three_digits_errors_with_A_letter.Contains(copied))
			{
				char corrected = 'A';
				output = corrected + rest;
			}
		}

		return output;
	}

	/**
	* Validate speech text in terms of lisps
	*/
	private string ValidateMovement(string value, char[] letters, char[] numbers)
	{
		char first = value[0];
		char second = value[1];

		if (!letters.Contains(value[0]))
		{
			if (value[0] == '8')
				first = 'A';

		}
		if (!numbers.Contains(value[1]))
		{
			if (value[1] == 'A')
				second = '8';
		}

		string output = Char.ToString(first) + Char.ToString(second);
		return output;
	}
}


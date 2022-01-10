using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class VoiceRecognition : MonoBehaviour
{
	public GameObject imageForVoiceControl;
	public Text RecognizerReading;

	public static VoiceRecognition Instance { get; set; }
	private DictationRecognizer DictationRecognizer;

	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

	private char[] possible_numbers = { '1', '2', '3', '4', '5', '6', '7', '8' };
	private char[] possible_characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

	/*
	 * Necessary for voice activation:
	 * -> Microphone enabled in Capablities in Player project settings
	 * -> English language pack
	 * -> Windows Speech recognition enabled
	 * -> Cortana installed and enabled
	*/
	private void DeviceControl()
	{
		foreach (var device in Microphone.devices)
		{
			Debug.Log("Name: " + device); // must exist
		}

		if (Application.systemLanguage == SystemLanguage.English)
		{
			Debug.Log("This system is in English. "); // must have
		}
		if (Application.systemLanguage == SystemLanguage.Polish)
		{
			Debug.Log("This system is in Polish. ");
		}

		if (PhraseRecognitionSystem.isSupported)
		{
			Debug.Log("Phrase Recognition is supported!"); // must have
		}
		else
		{
			Debug.Log("Phrase recognition is NOT supported!");
		}

		//kontrolka w rogu:
		//na zielono - wszystko ify spelnione
		//na czerwono - nie wszystko spelnione

	}

	public void Start()
	{
		Instance = this;
		DeviceControl();

		DictationRecognizer = new DictationRecognizer();
		DictationRecognizer.AutoSilenceTimeoutSeconds = 10f;
		DictationRecognizer.InitialSilenceTimeoutSeconds = 10f;
		DictationRecognizer.DictationResult += OnRecognizeSpeech;
		DictationRecognizer.Start();
	}

	public void Update()
	{
		if (DictationRecognizer.Status.Equals(SpeechSystemStatus.Stopped)
			|| DictationRecognizer.Status.Equals(SpeechSystemStatus.Failed))
		{
			DictationRecognizer.Start();
			//dla hey/ey/age itd.
			Debug.Log("Dictation recognizer started again after the timeout");
			Debug.Log(DictationRecognizer.AutoSilenceTimeoutSeconds);
			Debug.Log(DictationRecognizer.InitialSilenceTimeoutSeconds);
			Debug.Log(DictationRecognizer.Status);
		}

		if (DictationRecognizer.Status.Equals(SpeechSystemStatus.Running))
		{
			imageForVoiceControl.GetComponent<Image>().color = Color.green;
		}
		else
		{
			imageForVoiceControl.GetComponent<Image>().color = Color.red;
		}

	}

	public void OnRecognizeSpeech(string speech, ConfidenceLevel level)
	{
		Debug.Log(speech.ToString()); //D2D5
		string ab = speech.ToString();
		var first = ab.Substring(0, (int)(ab.Length / 2));
		var last = ab.Substring((int)(ab.Length / 2), (int)(ab.Length / 2));


		Debug.Log(first + "TO" + last);

		var startScreen = gameObject.GetComponent<StartScreen>();
		if (ab == "quit" || ab == "quit game" || ab == "exit" || ab == "exit game")
		{
			ChessBoardManager.Instance.popUpExitConfirmation.SetActive(true);
		}
		if (ChessBoardManager.Instance.popUpExitConfirmation.activeInHierarchy == true)
		{

			if (ab == "yes")
			{
				startScreen.LoadStartScene();
			}
			if (ab == "no" || ab == "cancel")
			{
				startScreen.DoNotExitGame();
			}
		}
		if (ChessBoardManager.Instance.popUpWindowForEndGame.activeInHierarchy == true)
		{
			if (ab == "play again")
			{
				startScreen.LoadGameScene();
			}
		}
		if (ab == "castle king" || ab == "castle king side")

		if (first.Length == 2 && last.Length == 2)
		{

			first = ValidateMovement(first, possible_characters, possible_numbers);
			last = ValidateMovement(last, possible_characters, possible_numbers);

			var positionDictionary = ChessBoardManager.Instance.PreparePositionDictionary();
			string aFind = positionDictionary.FirstOrDefault(x => x.Value == first).Key;
			Debug.Log("From:" + aFind);//

			string bFind = positionDictionary.FirstOrDefault(x => x.Value == last).Key;
			Debug.Log("To:" + bFind);//

			Tuple<int, int> helperX = StringHelper.Instance.GetBoardCoordinates(aFind);
			Debug.Log(aFind + ", x = " + helperX.Item1 + " y = " + helperX.Item2);

			Tuple<int, int> helperY = StringHelper.Instance.GetBoardCoordinates(bFind);
			Debug.Log(bFind + ", x = " + helperY.Item1 + " y = " + helperY.Item2);

			RecognizerReading.text = "From:" + first + " to:" + last;
			ExecuteMovement(helperX, helperY);
		}

	}

	private void ExecuteMovement(Tuple<int, int> from, Tuple<int, int> to)
	{
		ChessBoardManager.Instance.SelectChessPiece(from.Item1, from.Item2);
		ChessBoardManager.Instance.MoveChessPiece(to.Item1, to.Item2);
	}

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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Windows.Speech;

public class VoiceRecognition : MonoBehaviour
{
	public static VoiceRecognition Instance { get; set; }
    private DictationRecognizer DictationRecognizer;

    private Dictionary<string, Action> actions = new Dictionary<string,Action>();

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

	private void Start()
	{
		Instance = this;
		DeviceControl();


		DictationRecognizer = new DictationRecognizer();
		DictationRecognizer.DictationResult += OnRecognizeSpeech;
		DictationRecognizer.Start();

	}

	public void OnRecognizeSpeech(string speech, ConfidenceLevel level)
	{
		Debug.Log(speech.ToString()); //D2D5
		string ab = speech.ToString();
		var first = ab.Substring(0, (int)(ab.Length / 2));
		var last = ab.Substring((int)(ab.Length / 2), (int)(ab.Length / 2));

		Debug.Log(first + "TO" + last);


		var a = ChessBoardManager.Instance.PreparePositionDictionary();
		var aFind = a.FirstOrDefault(x => x.Value == first).Key;
		Debug.Log("From:" + aFind);

		var bFind = a.FirstOrDefault(x => x.Value == last).Key;
		Debug.Log("To:" + bFind);

		Tuple<int,int> helperX = StringHelper.Instance.GetBoardCoordinates(aFind);
		Debug.Log(aFind + ", x = " + helperX.Item1 + " y = " + helperX.Item2);

		Tuple<int, int> helperY = StringHelper.Instance.GetBoardCoordinates(bFind);
		Debug.Log(bFind + ", x = " + helperY.Item1 + " y = " + helperY.Item2);

		ExecuteMovement(helperX, helperY);
	}

	private void ExecuteMovement(Tuple<int,int> from,Tuple<int,int> to)
	{
		ChessBoardManager.Instance.SelectChessPiece(from.Item1, from.Item2);
		ChessBoardManager.Instance.MoveChessPiece(to.Item1, to.Item2);
	}

	private void ValidateMovement(string first, string second)
	{
		//sprawdzenie
	}

	private void VoiceMovement(string inpX, string inpY)
	{
		var a = ChessBoardManager.Instance.PreparePositionDictionary();
		var aFind = a.FirstOrDefault(x => x.Value == inpX).Key;
		var bFind = a.FirstOrDefault(x => x.Value == inpY).Key;

		if(!string.IsNullOrEmpty(aFind) && !string.IsNullOrEmpty(bFind))
		{
			Tuple<int, int> helperX = StringHelper.Instance.GetBoardCoordinates(aFind);
			Tuple<int, int> helperY = StringHelper.Instance.GetBoardCoordinates(bFind);
			ExecuteMovement(helperX, helperY);
		
		}
		else
		{
			ValidateMovement(inpX,inpY);
			//ExecuteMovement();
		}

	}



}

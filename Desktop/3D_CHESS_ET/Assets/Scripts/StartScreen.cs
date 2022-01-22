using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{

	public string GameScene;
	public string AboutScene;
	public string ScoreBoardScene;

	public GameObject ExitConfirmationDialog;
	public GameObject EndGamePopup;
	public GameObject customSettings;
	public InputField Customtime;
	public InputField CustomInc;

	public void ShowExitConfirmationDialog()
	{
		ExitConfirmationDialog.SetActive(true);
	}

	public void ExitGame()
	{

		Debug.Log("exit game was triggered");
		Application.Quit();
	}

	public void LoadGameScene()
	{
		SceneManager.LoadScene(GameScene);
		Time.timeScale = 1;
	}

	public void LoadAboutScene()
	{
		SceneManager.LoadScene(AboutScene);
	}

	public void LoadScoreboard()
	{
		SceneManager.LoadScene(ScoreBoardScene);
	}

	public void LoadStartScene()
	{
		SceneManager.LoadScene("StartScreen");
		ChessBoardManager.Instance.numberOfMoves = 0;
	}

	public void DoNotExitGame()
	{
		ExitConfirmationDialog.SetActive(false);
	}

	public void HidePopUpDialog()
	{
		EndGamePopup.SetActive(false);
		Timer.Instance.timerIsRunningWhite = false;
		Timer.Instance.timerIsRunningBlack = false;
	}

	public void StartGameOnCondition()
	{
		if(customSettings.activeInHierarchy)
		{
			if(Customtime.text == string.Empty)
			{
				Customtime.image.color = Color.red;
			}
			else
			{
				LoadGameScene();
			}
		}
		else
		{
			LoadGameScene();
		}
	}
}

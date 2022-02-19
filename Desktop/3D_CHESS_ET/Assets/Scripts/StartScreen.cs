using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Class responsible for handling pop-up windows and navigation between scenes 
 */
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

	/**
	 * Method loading on the screen Exit confirmation dialog
	 */
	public void ShowExitConfirmationDialog()
	{
		ExitConfirmationDialog.SetActive(true); // show dialog
	}

	/**
	 * Method quitting the game
	 */
	public void ExitGame()
	{
		Application.Quit(); // quit game
	}

	/**
	 * Method loading Game scene
	 */
	public void LoadGameScene()
	{
		SceneManager.LoadScene(GameScene); // load scene
		Time.timeScale = 1; // start time scale
	}
		
	/**
	 * Method loading About scene
	 */
	public void LoadAboutScene()
	{
		SceneManager.LoadScene(AboutScene); // load scene
	}

	/**
	 * Method loading Scoreboard scene
	 */
	public void LoadScoreboard()
	{
		SceneManager.LoadScene(ScoreBoardScene); // load scene
	}

	/**
	 * Method loading Start screen scene
	 */
	public void LoadStartScene()
	{
		SceneManager.LoadScene("StartScreen"); // start the game
		ChessBoardManager.Instance.numberOfMoves = 0; //reset moves counter
	}

	/**
	 * Method closing Exit confirmation dialog
	 */
	public void DoNotExitGame()
	{
		ExitConfirmationDialog.SetActive(false); // load scene
	}

	/**
	 * Method closing End game pop-up window
	 */
	public void HidePopUpDialog()
	{
		//close pop-up
		EndGamePopup.SetActive(false);
		//stop timers
		Timer.Instance.timerIsRunningWhite = false;
		Timer.Instance.timerIsRunningBlack = false;
	}

	/**
	 * Method loading the game scene on condition that custom time rules are set
	 */
	public void StartGameOnCondition()
	{
		//if custom time rules apply for the game
		if(customSettings.activeInHierarchy)
		{
			//if custom time is not filled - cannot start the game
			if(Customtime.text == string.Empty)
			{
				Customtime.image.color = Color.red;
			}
			else
			{
				LoadGameScene(); // load scene
			}
		}
		else
		{
			LoadGameScene(); // load scene
		}
	}
}

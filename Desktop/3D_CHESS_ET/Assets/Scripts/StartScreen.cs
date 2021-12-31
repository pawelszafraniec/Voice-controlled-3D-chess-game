﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{

	public string GameScene;
	public string AboutScene;
	public string ScoreBoardScene;

	public void ExitGame()
	{
		Debug.Log("exit game was triggered");
		Application.Quit();
	}

	public void LoadGameScene()
	{
		SceneManager.LoadScene(GameScene);
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
	}

}

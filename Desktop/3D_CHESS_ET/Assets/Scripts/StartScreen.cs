using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{

	public string Scene;

	public void ExitGame()
	{
		Debug.Log("exit game was triggered");
		Application.Quit();
	}

	public void StartGame()
	{
		SceneManager.LoadScene(Scene);
	}
}

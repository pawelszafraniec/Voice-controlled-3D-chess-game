using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class handling operations on scoreboard
 */
public class ScoreManager : MonoBehaviour
{
    private Data scoreData;
	
	/**
	 * AWAKE function - initialize scoreData before application starts
	 */
	void Awake()
	{
		// read and assign data about scores from PlayerPrefs
		var json = PlayerPrefs.GetString("Scores", "{}");
		scoreData = JsonUtility.FromJson<Data>(json);	
	}

	/**
	 * Method organizing Scores by data in descending order
	 */
	public IEnumerable<Score> GetScoresByData()
	{
		return scoreData.Scores.OrderByDescending(x => x.scoreTime);
	}

	/**
	 * Method adding new score to data
	 */
	public void AddScore(Score score)
	{
		scoreData.Scores.Add(score);
	}

	/**
	 * Unity method executed when application is quitted
	 */
	public void OnDestroy()
	{
		//Save scores to PlayerPrefs
		SaveScores();
	}

	/**
	 * Method saving scores to PlayerPrefs
	 */
	public void SaveScores()
	{
		var json = JsonUtility.ToJson(scoreData);
		PlayerPrefs.SetString("Scores", json);
	}

	/**
	 * Method copying result text in PGN format from selected cell to clipboard
	 */
	public void SaveToClipboard(Text result)
	{
		TextEditor editor = new TextEditor();
		editor.text = result.text;
		editor.SelectAll();
		editor.Copy();
	}
}

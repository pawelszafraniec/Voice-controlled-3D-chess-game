using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Data scoreData;

	void Awake()
	{
		var json = PlayerPrefs.GetString("Scores", "{}");
		scoreData = JsonUtility.FromJson<Data>(json);	
	}

	public IEnumerable<Score> GetScoresByData()
	{
		return scoreData.Scores.OrderByDescending(x => x.scoreTime);
	}

	public void AddScore(Score score)
	{
		scoreData.Scores.Add(score);
	}

	public void OnDestroy()
	{
		SaveScores();
	}

	public void SaveScores()
	{
		var json = JsonUtility.ToJson(scoreData);
		PlayerPrefs.SetString("Scores", json);
	}

	public void SaveToClipboard(Text result)
	{
		TextEditor editor = new TextEditor();
		editor.text = result.text;
		editor.SelectAll();
		editor.Copy();
	}
}

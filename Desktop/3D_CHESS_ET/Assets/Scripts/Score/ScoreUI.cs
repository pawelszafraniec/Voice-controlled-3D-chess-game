using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Class for the UI of scoreboard
 */
public class ScoreUI : MonoBehaviour
{
    public RowHelper row;
    public ScoreManager manager;

	/**
	 * START method - runs when script is being enabled
	 */
	void Start()
	{
		var scores = manager.GetScoresByData().ToArray();
		// organize the scores data and assign it to proper cells of the table
		for(int i = 0; i < scores.Length; i++)
		{
			var r = Instantiate(row, transform).GetComponent<RowHelper>();
			r.ID.text = (i + 1).ToString();
			r.Winner.text = scores[i].winner;
			r.Lost.text = scores[i].lost;
			r.DateGame.text = scores[i].scoreTime;
			r.numberOfMoves.text = scores[i].numberOfMoves.ToString();
			r.gameCourse.text = scores[i].gameCourse;
		}
	}
}

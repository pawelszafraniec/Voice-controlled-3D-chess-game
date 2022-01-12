using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public RowHelper row;
    public ScoreManager manager;

	void Start()
	{
		//manager.AddScore(new Score("12314", DateTime.Now.TimeOfDay));

		var scores = manager.GetScoresByData().ToArray();
		for(int i = 0; i < scores.Length; i++)
		{
			var r = Instantiate(row, transform).GetComponent<RowHelper>();
			r.ID.text = (i + 1).ToString();
			r.Winner.text = scores[i].winner;
			r.Lost.text = scores[i].lost;
			r.DateGame.text = scores[i].scoreTime;
		}
	}
}

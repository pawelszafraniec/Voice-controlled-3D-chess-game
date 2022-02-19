using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Data class for Scoreboard
 */
[Serializable]
public class Data
{
    public List<Score> Scores;

    public Data()
	{
		Scores = new List<Score>();
	}
}

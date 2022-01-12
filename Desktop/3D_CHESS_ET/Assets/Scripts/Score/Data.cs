using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public List<Score> Scores;

    public Data()
	{
		Scores = new List<Score>();
	}
}

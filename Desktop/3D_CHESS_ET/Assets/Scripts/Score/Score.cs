using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Score
{
    public string id;
    public string winner;
    public string lost;
    public string scoreTime;

    public Score(string id,string winner,string lost, string scoreTime)
	{
        this.scoreTime = scoreTime;
        this.id = id;
        this.winner = winner;
        this.lost = lost;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Score
{
    public string id;
    public string winner;
    public string lost;
    public string scoreTime;
    public int numberOfMoves;
    public string gameCourse; 

    public Score(string id,string winner,string lost, string scoreTime, int numberOfMoves, string gameCourse)
	{
        this.scoreTime = scoreTime;
        this.id = id;
        this.winner = winner;
        this.lost = lost;
        this.numberOfMoves = numberOfMoves;
        this.gameCourse = gameCourse;
	}
}

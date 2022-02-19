using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
	#region fields
	string TimerValue = GameOptionsManager.timeValue;
    string WhiteName = GameOptionsManager.playerWhiteName;
    string BlackName = GameOptionsManager.playerBlackName;

    int customTimer = GameOptionsManager.customTimerValue;
    int customIncr = GameOptionsManager.customIncrementValue; 

    public static Timer Instance { get; set; }

    public float timeRemainingWhite = 25;
	public bool timerIsRunningWhite = false;
    public Text timeTextWhite;

    public float timeRemainingBlack = 25;
    public bool timerIsRunningBlack = false;
    public Text timeTextBlack;

    public bool ifIncrement;
    public int incrementValue;

    public string result;

    public ScoreManager manager;
    public int flag = 0;

    #endregion

    /**
	 * START method - runs when script is being enabled
	 */
    private void Start()
	{
        Instance = this;
        SetTimers();
        Init();
    }

    /**
     * Method that initialize chess timers with selected values
     */
    void Init()
	{
        timerIsRunningWhite = true;
        timerIsRunningBlack = true;
        DisplayTime(timeRemainingWhite, timeTextWhite);
        DisplayTime(timeRemainingBlack - 1, timeTextBlack);
        flag = 0;
    }

    /**
     * Method that set value of chess timers from possible options or exclude them
     */
    void SetTimers()
	{
        switch (TimerValue)
        {
            case "10":
                timeRemainingBlack = 10 * 60;
                timeRemainingWhite = 10 * 60;
                break;
            case "10 + 1":
                timeRemainingBlack = 10 * 60;
                timeRemainingWhite = 10 * 60;
                ifIncrement = true;
                incrementValue = 1;
                break;
            case "3":
                timeRemainingBlack = 3 * 60;
                timeRemainingWhite = 3 * 60;
                break;
            case "3 + 1": //3 + 1s increment
                
                break;
            case "15":
                timeRemainingBlack = 15 * 60;
                timeRemainingWhite = 15 * 60;
                break;
            case "15 + 1": //15 + 1s increment
                timeRemainingBlack = 15 * 60;
                timeRemainingWhite = 15 * 60;
                ifIncrement = true;
                incrementValue = 1;
                break;
            case "No time limit":
                GetComponent<Timer>().enabled = false;
                GameObject.Find("Timers").SetActive(false);
                break;
            case "Custom":
                timeRemainingBlack = customTimer * 60;
                timeRemainingWhite = customTimer * 60;
                ifIncrement = true;
                incrementValue = customIncr;
                break;
            default:
                timeRemainingBlack = 10 * 60;
                timeRemainingWhite = 10 * 60;
                break;
        }
    }

    /**
	 * UPDATE method - runs on each frame of the game
	 */
    void Update()
	{
        DateTime now = DateTime.Now;

        // White is moving
        if (ChessBoardManager.Instance.isWhiteTurn)
		{
			timerIsRunningBlack = false;
            timerIsRunningWhite = true;
            if (timerIsRunningWhite) // timer for white is running
            {
                if (timeRemainingWhite > 0) // if player has got time
                {
                    timeRemainingWhite -= Time.deltaTime; // decrement time by 1 second
                    DisplayTime(timeRemainingWhite, timeTextWhite); // Display time
                }
                else // end of time - white lost
                {
                    result = BlackName + " won by timeout!"; // dark won
                    ChessBoardManager.Instance.EndGamePopUp(result); // end game pop-up
                    timeRemainingWhite = 0;
                    timerIsRunningWhite = false;
                    if(flag == 0)
					{
                        //add new score to scoreboard table
                        ChessBoardManager.Instance.manager.AddScore(new Score("1", BlackName, WhiteName + " (timeout)", now.ToString("MM/dd/yyyy H:mm"), ChessBoardManager.Instance.numberOfMoves, ChessBoardManager.Instance.chessNotation));
                        flag = 1;
                    }
                }
            }
        }
        else // Dark is moving
		{
            timerIsRunningBlack = true;
            timerIsRunningWhite = false;
            if (timerIsRunningBlack)
            {
                if (timeRemainingBlack > 0) // if player has got time
                {
                    timeRemainingBlack -= Time.deltaTime; // decrement time by 1 second
                    DisplayTime(timeRemainingBlack, timeTextBlack); // Display time
                }
                else
                {
                    result = WhiteName + " won by timeout!"; // white won
                    ChessBoardManager.Instance.EndGamePopUp(result); // end-game pop-up
                    timeRemainingBlack = 0;
                    timerIsRunningBlack = false;
                    
                    if(flag == 0)
					{
                        //add new score to scoreboard table
                        ChessBoardManager.Instance.manager.AddScore(new Score("1", WhiteName, BlackName + " (timeout)", now.ToString("MM/dd/yyyy H:mm"), ChessBoardManager.Instance.numberOfMoves, ChessBoardManager.Instance.chessNotation));
                        flag = 1;
					}
                }
            }
        }
    }

    // DisplayTime - Display time on chess clock (text field) in proper format
    public void DisplayTime(float timeToDisplay, Text timeText)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /**
     * Get White player name
     */
    public string GetWhiteName()
	{
        return WhiteName;
	}

    /**
     * Get Dark player name
     */
    public string GetBlackName()
	{
        return BlackName;
	}

}


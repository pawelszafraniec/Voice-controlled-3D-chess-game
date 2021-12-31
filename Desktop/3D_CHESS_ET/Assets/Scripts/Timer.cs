using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    string TimerValue = GameOptionsManager.timeValue;

    public float timeRemainingWhite = 17;
	public bool timerIsRunningWhite = false;
    public Text timeTextWhite;

    public float timeRemainingBlack = 17;
    public bool timerIsRunningBlack = false;
    public Text timeTextBlack;

    public bool ifIncrement;
    public int incrementValue;

    private void Start()
	{
        SetTimers();
        Init();
    }

    void Init()
	{
        timerIsRunningWhite = true;
        timerIsRunningBlack = true;
        DisplayTime(timeRemainingWhite, timeTextWhite);
        DisplayTime(timeRemainingBlack - 1, timeTextBlack);
    }

    void SetTimers()
	{
        //dokonczyc dla incrementu
        switch (TimerValue)
        {
            case "10":
                timeRemainingBlack = 10;
                timeRemainingWhite = 10;
                break;
            case "10 + 1":
                timeRemainingBlack = 10;
                timeRemainingWhite = 10;
                ifIncrement = true;
                incrementValue = 1;
                break;
            case "3":
                timeRemainingBlack = 3;
                timeRemainingWhite = 3;
                break;
            default:
                timeRemainingBlack = 10;
                timeRemainingWhite = 10;
                break;


        }
    }

    void Update()
	{
        if(ChessBoardManager.Instance.isWhiteTurn)
		{
            //if (ifIncrement)
            //{
            //    timeRemainingWhite += incrementValue;
            //}

            timerIsRunningBlack = false;
            timerIsRunningWhite = true;
            if (timerIsRunningWhite)
            {

                if (timeRemainingWhite > 0)
                {
                    timeRemainingWhite -= Time.deltaTime;
                    DisplayTime(timeRemainingWhite, timeTextWhite);
                }
                else
                {
                    Debug.Log("Time has run out! BLACK WON");
                    timeRemainingWhite = 0;
                    timerIsRunningWhite = false;
                }
            }
        }
        else
		{
            //if (ifIncrement)
            //{
            //    timeRemainingBlack += incrementValue;
            //}

            timerIsRunningBlack = true;
            timerIsRunningWhite = false;
            if (timerIsRunningBlack)
            {

                if (timeRemainingBlack > 0)
                {
                    timeRemainingBlack -= Time.deltaTime;
                    DisplayTime(timeRemainingBlack, timeTextBlack);
                }
                else
                {
                    Debug.Log("Time has run out! WHITE WON");
                    timeRemainingBlack = 0;
                    timerIsRunningBlack = false;
                }
            }

        }
    }

    void DisplayTime(float timeToDisplay, Text timeText)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float STARTING_TIME = 61;

    [SerializeField]
    Text timerText;

    float timeRemaining;

    bool running;

    void Start()
    {
        timerText = GetComponent<Text>();

         timeRemaining = STARTING_TIME;

         timeRemaining -= Time.deltaTime;
         float minutes = Mathf.FloorToInt(timeRemaining / 60);
         float seconds = Mathf.FloorToInt(timeRemaining % 60);
         timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

         running = true;
    }

    void Update()
    {
        if (running)
        {
            if (timeRemaining > 1)
            {
                timeRemaining -= Time.deltaTime;
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                running = false;
            }
        }

    }

    public void addToTime(float num)
    {
        timeRemaining += num;
    }

    public float getTimeLeft()
    {
        return timeRemaining;
    }

    public bool getRunning()
    {
        return running;
    }

    public void resetTime()
    {
        timeRemaining = STARTING_TIME;
        running = true;
    }

    public void pause()
    {
        running = false;
    }

    public void unpause()
    {
        running = true;
    }
}
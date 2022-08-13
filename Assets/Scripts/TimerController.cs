using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TimerController : MonoBehaviourPunCallbacks
{
    public float timeRemaining = 3;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    private void Start()
    {
        timeText.gameObject.SetActive(false);
        StartTimer();
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                timeText.gameObject.SetActive(false);
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void StartTimer()
    {
        timeText.gameObject.SetActive(true);
        timerIsRunning = true;
    }
}

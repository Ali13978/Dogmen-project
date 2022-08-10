using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MatchManager : MonoBehaviourPunCallbacks
{
    public static MatchManager Instance;
    public string WinnerName;

    private void Awake()
    {
        Instance = this;
    }
    
    List<GameObject> AllPlayers;

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowWinner()
    {
        UIController.Instance.TurnOffAllPannels();
        UIController.Instance.WinnerPannel.SetActive(true);
        UIController.Instance.WinnerText.text = "Winner is " + WinnerName;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class AppearSelectedDog : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> AllDogs;
    [SerializeField] GameObject MyCamera;


    private void Awake()
    {

    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            ShowSelectedPlayer();
        }
        else
        {
            Destroy(MyCamera);
        }
    }

    public Transform SelectedDogTransform()
    {
        return AllDogs[PlayerPrefs.GetInt("SelectedDog")].transform;
    }
    
    private void ShowSelectedPlayer()
    {
        foreach (GameObject i in AllDogs)
        {
            i.SetActive(false);
        }
        for (int i = 0; i <= AllDogs.Count; i++)
        {
            PlayerPrefs.SetInt("SelectedDog", 0);
            if (i == PlayerPrefs.GetInt("SelectedDog"))
            {
                AllDogs[i].SetActive(true);
                break;
            }
        }
    }
}


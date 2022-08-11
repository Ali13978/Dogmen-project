using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class AppearSelectedDog : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> AllDogs;
    [SerializeField] GameObject MyCamera;
    [SerializeField] int Health;
    int CurrentHealth;
    
    public PhotonView pv;

    string GameOver = "Gamme is Over";

    private void Awake()
    {
        pv = photonView;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            ShowSelectedPlayer();
            UIController.Instance.HealthSlider.maxValue = Health;
            CurrentHealth = (int)UIController.Instance.HealthSlider.maxValue;
            UIController.Instance.HealthSlider.value = CurrentHealth;
        }
        else
        {
            Destroy(MyCamera);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "HitBox" && other.gameObject != GetComponentInChildren<DogController>().HitBox)
        {
            if (photonView.IsMine)
            {
                CurrentHealth -= other.GetComponentInParent<DogController>().Damage;
                if(CurrentHealth > 0)
                {
                    UIController.Instance.HealthSlider.value = CurrentHealth;
                }
                else
                {
                    pv.RPC("SyncValues", RpcTarget.All, GameOver);
                }
            }
        }
    }
    
    [PunRPC]
    void SyncValues(string _gameOver)
    {
        Debug.Log(_gameOver);
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


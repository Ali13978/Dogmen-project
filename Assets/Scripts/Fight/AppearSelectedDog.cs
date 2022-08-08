using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AppearSelectedDog : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> AllDogs;
    int SelectedDog;
    [SerializeField] GameObject MyCamera;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject i in AllDogs)
            {
                i.SetActive(false);
            }
            for (int i = 0; i <= AllDogs.Count; i++)
            {
                if (i == PlayerPrefs.GetInt("SelectedDog"))
                {
                    SelectedDog = i;
                    AllDogs[SelectedDog].SetActive(true);
                }
            }
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
}


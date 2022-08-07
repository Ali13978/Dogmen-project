using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearSelectedDog : MonoBehaviour
{
    [SerializeField] List<GameObject> AllDogs;
    int SelectedDog;
    private void Awake()
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

    public Transform SelectedDogTransform()
    {
        return AllDogs[PlayerPrefs.GetInt("SelectedDog")].transform;
    }
}


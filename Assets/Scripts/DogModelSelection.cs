using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogModelSelection : MonoBehaviour
{
    [SerializeField] List<GameObject> AllDogs;
    [SerializeField] List<DogSO> AllDogsSO;
    private void Start()
    {
        int selectedDogIndex = PlayerPrefs.GetInt("SelectedDog", 0);
        InitDog(selectedDogIndex);
    }
    private void InitDog(int dogId)
    {
        DogSO selectedDog = AllDogsSO.Find(x => x.index == dogId);

        foreach (GameObject dog in AllDogs)
        {
            dog.SetActive(false);
        }

        AllDogs[selectedDog.index].SetActive(true);
    }


}

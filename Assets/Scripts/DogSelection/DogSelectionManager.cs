using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DogSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject SelectDogPannel;
    [SerializeField] List<TMP_Text> SelectButtonTexts;
    private void Start()
    {
        SelectDogPannel.SetActive(false);
    }

    public void ShowSelectDogPannel()
    {
        foreach(TMP_Text i in SelectButtonTexts)
        {
            i.text = "Select";
        }
        SelectButtonTexts[PlayerPrefs.GetInt("SelectedDog")].text = "Selected";
        SelectDogPannel.SetActive(true);
    }

    public void HideSelectDogPannel()
    {
        SelectDogPannel.SetActive(false);
    }

    public void SelectDogButton()
    {

    }
}

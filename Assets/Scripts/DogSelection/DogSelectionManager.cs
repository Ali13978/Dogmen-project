using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        RefreshDogPannel();
    }

    private void RefreshDogPannel()
    {
        foreach (TMP_Text i in SelectButtonTexts)
        {
            i.transform.parent.GetComponent<Image>().color = Color.white;
            i.text = "Select";
        }
        SelectDogPannel.SetActive(true);
        SelectButtonTexts[PlayerPrefs.GetInt("SelectedDog")].text = "Selected";
        SelectButtonTexts[PlayerPrefs.GetInt("SelectedDog")].GetComponentInParent<Button>().gameObject.GetComponent<Image>().color = Color.green;
    }

    public void HideSelectDogPannel()
    {
        SelectDogPannel.SetActive(false);
    }

    public void SelectDogButton()
    {
        var go = EventSystem.current.currentSelectedGameObject;
        if (go != null)
        {
            PlayerPrefs.SetInt("SelectedDog", go.gameObject.GetComponentInParent<DogSelectionHelper>().IndexInList);
            RefreshDogPannel();
        }
    }
}

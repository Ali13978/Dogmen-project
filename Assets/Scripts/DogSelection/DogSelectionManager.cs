using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using TMPro;

public class DogSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject SelectDogPannel;
    [SerializeField] List<TMP_Text> SelectButtonTexts;
    private void Start()
    {
        PlayerPrefs.SetInt("SelectedDog", 0);
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

    private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();

    //public void SelectCharacter(int index)
    //{
    //    PlayerPrefs.SetInt("SelectedDog", index);
    //    Debug.Log("Player prefs: " + PlayerPrefs.GetInt("MyCharacter"));
    //    _myCustomProperties["ID"] = index;
    //    PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
    //}

    public void SelectDogButton()
    {
        int index = 0;
        var go = EventSystem.current.currentSelectedGameObject;
        if (go != null)
        {
            index = go.gameObject.GetComponentInParent<DogSelectionHelper>().IndexInList;
            PlayerPrefs.SetInt("SelectedDog", index);
            Debug.Log("Player prefs: " + PlayerPrefs.GetInt("SelectedDog"));
            _myCustomProperties["ID"] = index;
            PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
            RefreshDogPannel();
        }
    }
}

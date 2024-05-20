using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GYMController : MonoBehaviour
{
    #region Singleton
    public static GYMController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] public DogSO selectedDogSO;
    [SerializeField] public GYMDoor gymDoor;
    [SerializeField] GameObject[] dogsCatalogueObjects;
    public void SelectDog(DogSO dog)
    {
        selectedDogSO = dog;
        SetCatalogueObjectsActive(false);
        PlayerPrefs.SetInt("SelectedDog", dog.index);
        SelectTrainingUI.Instance.EnableSelectTrainingUI();
    }
    public void SetCatalogueObjectsActive(bool isActive)
    {
        foreach (GameObject go in dogsCatalogueObjects)
        {
            go.SetActive(isActive);
        }
    }
}

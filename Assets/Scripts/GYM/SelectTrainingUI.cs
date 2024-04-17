using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTrainingUI : MonoBehaviour
{
    #region Singleton
    public static SelectTrainingUI Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [Header("SelectTrainingPanel")]
    public GameObject selectTrainingPanel;
    [SerializeField] Button selectTrainingBackBtn;
    [SerializeField] Button selectTreadmill;

    [Header("Treadmill panel")]
    [SerializeField] GameObject treadmillUIPanel;
    [SerializeField] Button treadmillPanelBackBtn;
    [SerializeField] Slider treadmillSlider;

    private void Start()
    {
        InitSelectTrainingBtns();
        InitTreadMillUIBtns();
    }

    private void InitSelectTrainingBtns()
    {
        selectTrainingBackBtn.onClick.AddListener(() =>
        {
            selectTrainingPanel.SetActive(false);
            GYMController.instance.SetCatalogueObjectsActive(true);
        });

        selectTreadmill.onClick.AddListener(() =>
        {
            selectTrainingPanel.SetActive(false);
            treadmillUIPanel.SetActive(true);
            TreadmillController.instance.EnableTreadmill(GYMController.instance.selectedDogSO);
            treadmillSlider.value = 1;
        });
    }

    private void InitTreadMillUIBtns()
    {
        treadmillPanelBackBtn.onClick.AddListener(() =>
        {
            treadmillUIPanel.SetActive(false);
            selectTrainingPanel.SetActive(true);
            TreadmillController.instance.DisableTreadmill();
        });

        treadmillSlider.value = 1;
    }

    public void AddValueToTreadmillSlider(float value)
    {
        treadmillSlider.value += value;

        if (treadmillSlider.value > 1)
            treadmillSlider.value = 1;
    }

    public void SubtractValueFromTreadmillSlider(float value)
    {
        treadmillSlider.value -= value;

        if (treadmillSlider.value < 0)
            treadmillSlider.value = 0;
    }

    public void EnableSelectTrainingUI()
    { 
        selectTrainingPanel.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] Button selectTyreRope;
    [SerializeField] Button selectFightTraining;

    [Header("Treadmill panel")]
    [SerializeField] GameObject treadmillUIPanel;
    [SerializeField] Button treadmillPanelBackBtn;
    [SerializeField] Slider treadmillSlider;
    [SerializeField] Slider treadmillSpeedSlider;

    [Header("Tyre Rope panel")]
    [SerializeField] GameObject tyreRopeUIPanel;
    [SerializeField] Button tyreRopePanelBackBtn;

    private void Start()
    {
        InitSelectTrainingBtns();
        InitTreadMillUIBtns();
        InitTyreRopeUIBtns();
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

        selectTyreRope.onClick.AddListener(() =>
        {
            selectTrainingPanel.SetActive(false);
            tyreRopeUIPanel.SetActive(true);
            TyreRopeController.instance.EnableTyreRope(GYMController.instance.selectedDogSO);
        });

        selectFightTraining.onClick.AddListener(() => 
        {
            SceneManager.LoadSceneAsync("FightTraining");
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
    
    private void InitTyreRopeUIBtns()
    {
        tyreRopePanelBackBtn.onClick.AddListener(() =>
        {
            tyreRopeUIPanel.SetActive(false);
            selectTrainingPanel.SetActive(true);
            TyreRopeController.instance.DisableTyreRope();
        });
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

        if (treadmillSlider.value <= 0)
        {
            Debug.Log("Treadmill slider elapsed");
            treadmillUIPanel.SetActive(false);
            selectTrainingPanel.SetActive(true);
            TreadmillController.instance.DisableTreadmill();
        }
    }

    public void EnableSelectTrainingUI()
    { 
        selectTrainingPanel.SetActive(true);
    }

    public void SetTreadmillSpeedSlider(float speedMultiplier)
    {
        treadmillSpeedSlider.value = speedMultiplier;
    }
}

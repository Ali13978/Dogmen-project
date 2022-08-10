using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] public Slider HealthSlider;

    [SerializeField] public GameObject WinnerPannel;
    [SerializeField] public TMP_Text WinnerText;
    // Start is called before the first frame update
    void Start()
    {
        TurnOffAllPannels();
        HealthSlider.gameObject.SetActive(true);
    }

    public void TurnOffAllPannels()
    {
        HealthSlider.gameObject.SetActive(false);
        WinnerPannel.SetActive(false);
    }
}

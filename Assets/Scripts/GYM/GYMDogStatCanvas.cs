using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GYMDogStatCanvas : MonoBehaviour
{
    [SerializeField] Button backBtn;
    [SerializeField] Button BuyBtn;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Slider speedSlider;
    [SerializeField] Slider willPowerSlider;
    [SerializeField] Slider damageSlider;
    [SerializeField] Slider staminaSlider;
    #region Singleton
    public static GYMDogStatCanvas Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public void RefreshStatsUI(DogSO dog)
    {
        nameText.text = "Dog " + (dog.index + 1);


        speedSlider.value = dog.moveSpeedPower;
        willPowerSlider.value = dog.willPower;
        damageSlider.value = dog.attackDamagePower;
        staminaSlider.value = dog.stamina;


        BuyBtn.onClick.RemoveAllListeners();

        BuyBtn.GetComponentInChildren<TMP_Text>().text = "Select";

        BuyBtn.onClick.AddListener(() =>
        {
            SelectDog(dog);
        });


        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(() => {
            GYMController.instance.gymDoor.ExitGYM();
        });
    }

    private void SelectDog(DogSO dog)
    {
        GYMController.instance.SelectDog(dog);
    }
}

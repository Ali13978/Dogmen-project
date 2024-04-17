using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class DogStatCanvas : MonoBehaviour
{
    [SerializeField] Button BuyBtn;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Slider speedSlider;
    [SerializeField] Slider willPowerSlider;
    [SerializeField] Slider damageSlider;
    [SerializeField] Slider staminaSlider;

    #region Singleton
    public static DogStatCanvas Instance;

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

        if (dog.isPurchased)
        {
            BuyBtn.GetComponentInChildren<TMP_Text>().text = "Owned";
            return;
        }
        else
        {
            BuyBtn.GetComponentInChildren<TMP_Text>().text = "Buy";
        }

        BuyBtn.onClick.AddListener(() =>
        {
            dog.isPurchased = true;
            DogsSaveLoad.instance.SaveDog(dog);
            RefreshStatsUI(dog);
        });
    }
}

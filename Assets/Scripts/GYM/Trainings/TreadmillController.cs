using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillController : MonoBehaviour
{
    #region Singleton
    public static TreadmillController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] GameObject treadmillCam;
    [SerializeField] GameObject treadmillDog;
    [SerializeField] TreadmillDog treadmillDogController;
    [SerializeField] Material treadmillBaseMaterial;
    [SerializeField] int offsetSpeed;
    [SerializeField] float treadmillFallSpeed;
    [SerializeField] float treadmillAddSpeed;
    private bool treadmillEnabled;
    private float offsetY;
    public void EnableTreadmill(DogSO dog)
    {
        treadmillDog.SetActive(true);
        treadmillCam.SetActive(true);
        treadmillDogController.InitDog(dog);
        treadmillEnabled = true;
        treadmillDogController.EnableWalkAnim();
    }

    public void DisableTreadmill()
    {
        treadmillDog.SetActive(false);
        treadmillCam.SetActive(false);
        treadmillEnabled = false;
        treadmillDogController.EnableIdleAnim();
    }

    void Update()
    {
        if (!treadmillEnabled)
            return;

        offsetY += Time.deltaTime * offsetSpeed;
        if(offsetY >= 1)
            offsetY = 0;
        treadmillBaseMaterial.mainTextureOffset = new Vector2(0, offsetY);

        SelectTrainingUI.Instance.SubtractValueFromTreadmillSlider(Mathf.Clamp(Time.deltaTime * treadmillFallSpeed, 0, .5f));

        if (Input.GetButtonDown("ActionBtn"))
            SelectTrainingUI.Instance.AddValueToTreadmillSlider(Mathf.Clamp(Time.deltaTime * treadmillAddSpeed, 0, 1));
    }

}

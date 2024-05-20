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
    private float speedMultiplier = 1.0f;
    public void EnableTreadmill(DogSO dog)
    {
        treadmillDog.SetActive(true);
        treadmillCam.SetActive(true);
        treadmillDogController.InitDog(dog);
        treadmillEnabled = true;
        treadmillDogController.EnableWalkAnim();
        speedMultiplier = 1.0f;
        StartCoroutine(TimerCoroutine());
    }

    public void DisableTreadmill()
    {
        treadmillDog.SetActive(false);
        treadmillCam.SetActive(false);
        treadmillEnabled = false;
        treadmillDogController.EnableIdleAnim();
        speedMultiplier = 1.0f;
        SelectTrainingUI.Instance.SetTreadmillSpeedSlider(speedMultiplier);
        StopCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        speedMultiplier += 0.3f;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1, 3);
        SelectTrainingUI.Instance.SetTreadmillSpeedSlider(speedMultiplier);
        StartCoroutine(TimerCoroutine());

    }

    void Update()
    {
        if (!treadmillEnabled)
            return;

        offsetY += Time.deltaTime * offsetSpeed;
        if(offsetY >= 1)
            offsetY = 0;
        treadmillBaseMaterial.mainTextureOffset = new Vector2(0, offsetY);

        SelectTrainingUI.Instance.SubtractValueFromTreadmillSlider(Mathf.Clamp(Time.deltaTime * treadmillFallSpeed * speedMultiplier, 0, .5f));

        if (Input.GetButtonDown("ActionBtn"))
            SelectTrainingUI.Instance.AddValueToTreadmillSlider(Mathf.Clamp(Time.deltaTime * treadmillAddSpeed, 0, 1));
    }

}

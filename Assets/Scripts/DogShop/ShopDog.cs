using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShopDog : MonoBehaviour
{
    [SerializeField] List<GameObject> AllDogs;
    [SerializeField] Animator dogAnim;// Animator for the assigned dog
    private float countDown = 1;
    bool Sit_b = false;

    [Header("Particle FX")]
    [SerializeField] ParticleSystem poopFX;
    [SerializeField] ParticleSystem dirtFX;
    [SerializeField] ParticleSystem peeFX;
    [SerializeField] ParticleSystem waterFX;
    [SerializeField] Transform fxTransform;
    [SerializeField] Transform fxTail;
    private int locomotionLayerIndex;

    private int currentIndex;

    private void Awake()
    {
        locomotionLayerIndex = dogAnim.GetLayerIndex("Locomotion");

        InitShopDog();
    }

    private void Update()
    {
        if(IsCurrentClipFinished())
        {
            Actions();
        }
    }

    private void InitShopDog()
    {
        currentIndex = Random.Range(0, AllDogs.Count);
        foreach (GameObject dog in AllDogs)
        {
            dog.SetActive(false);
        }
        AllDogs[currentIndex].SetActive(true);
    }

    #region Actions
    private void Actions()
    {
        int randomAction = Random.Range(1, 14);

        if (randomAction == 1)
        {
            StartCoroutine(DogActions(1));
        }
        else if (randomAction == 2)
        {
            StartCoroutine(DogActions(2));
        }
        else if (randomAction == 3)
        {
            StartCoroutine(DogActions(3));
        }
        else if (randomAction == 4)
        {
            StartCoroutine(DogActions(4));
            if (!Sit_b)
            {
                dirtFX.transform.localPosition = new Vector3(dirtFX.transform.localPosition.x, dirtFX.transform.localPosition.y, dirtFX.transform.localPosition.z + 0.3f);
                dirtFX.Play();
            }
        }
        else if (randomAction == 5)
        {
            StartCoroutine(DogActions(5));
        }
        else if (randomAction == 6)
        {
            StartCoroutine(DogActions(6));
        }
        else if (randomAction == 7)
        {
            StartCoroutine(DogActions(7));
        }
        else if (randomAction == 8)
        {
            StartCoroutine(DogActions(8));
            if (!Sit_b)
            {
                peeFX.transform.position = new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f);
                peeFX.transform.SetParent(fxTransform);
                peeFX.transform.localPosition = new Vector3(peeFX.transform.localPosition.x, peeFX.transform.localPosition.y, peeFX.transform.localPosition.z - 0.2f);
                peeFX.transform.localRotation = Quaternion.Euler(0, -45, 0);

                peeFX.Play();
            }
        }
        else if (randomAction == 9)
        {
            StartCoroutine(DogActions(9));
            if (!Sit_b)
            {
                poopFX.transform.position = new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f);
                poopFX.transform.SetParent(fxTransform);
                poopFX.transform.localPosition = new Vector3(poopFX.transform.localPosition.x, poopFX.transform.localPosition.y, poopFX.transform.localPosition.z - 0.35f);
                poopFX.Play();
            }
        }
        else if (randomAction == 10)
        {
            StartCoroutine(DogActions(10));
            if (!Sit_b)
            {
                waterFX.transform.localPosition = new Vector3(waterFX.transform.localPosition.x, waterFX.transform.localPosition.y - 0.0f, waterFX.transform.localPosition.z);
                waterFX.gameObject.transform.GetChild(0).transform.position = new Vector3(fxTail.transform.position.x, fxTail.transform.position.y, fxTail.transform.position.z);
                waterFX.Play();
            }
        }
        else if (randomAction == 11)
        {
            StartCoroutine(DogActions(11));
        }
        else if (randomAction == 12)
        {
            StartCoroutine(DogActions(12));
        }
        else if (randomAction == 13)
        {
            StartCoroutine(DogActions(13));
        }
    }


    IEnumerator DogActions(int actionType) // Dog action coroutine
    {
        dogAnim.SetInteger("ActionType_int", actionType); // Enable Animation

        string currentAnimation = dogAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        float animationLength = dogAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        countDown = animationLength;
        yield return new WaitForSeconds(countDown); // Countdown
        dogAnim.SetInteger("ActionType_int", 0); // Disable animation
    }
    bool IsCurrentClipFinished()
    {
        AnimatorStateInfo currentState = dogAnim.GetCurrentAnimatorStateInfo(locomotionLayerIndex);
        return (!currentState.loop && currentState.normalizedTime >= 1f) ||
               (currentState.loop && currentState.normalizedTime >= 1f && Mathf.FloorToInt(currentState.normalizedTime) > 0);
    }

    #endregion

    public void NextDog()
    {
        foreach (GameObject dog in AllDogs)
        {
            dog.SetActive(false);
        }

        currentIndex++;
        if (currentIndex >= AllDogs.Count)
            currentIndex = 0;

        AllDogs[currentIndex].SetActive(true);
    }

    public void PreviousDog()
    {
        foreach (GameObject dog in AllDogs)
        {
            dog.SetActive(false);
        }

        currentIndex--;
        if(currentIndex < 0)
            currentIndex = AllDogs.Count - 1;

        AllDogs[currentIndex].SetActive(true);
    }
}

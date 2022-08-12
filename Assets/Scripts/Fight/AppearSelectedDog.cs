using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class AppearSelectedDog : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> AllDogs;
    [SerializeField] List<Material> DogsMaterials;
    [SerializeField] GameObject Skin;
    [SerializeField] GameObject MyCamera;
    [SerializeField] int Health;
    int CurrentHealth;

    [HideInInspector] public bool IsHitted = false;


    private void Awake()
    {

    }



    private void Start()
    {
        if (photonView.IsMine)
        {
            UIController.Instance.HealthSlider.maxValue = Health;
            CurrentHealth = (int)UIController.Instance.HealthSlider.maxValue;
            UIController.Instance.HealthSlider.value = CurrentHealth;
            ShowSelectedPlayer();
        }
        else
        {
            Destroy(MyCamera);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "HitBox" && other.gameObject != GetComponentInChildren<DogController>().HitBox)
        {
            if (photonView.IsMine)
            {
                if (!IsHitted)
                {
                    CurrentHealth -= other.GetComponentInParent<DogController>().Damage;
                }

                if (other.gameObject.transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Arm_Dog_Attack_L") || other.gameObject.transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Arm_Dog_Attack_R"))
                {
                    IsHitted = true;
                    GetComponentInChildren<DogController>().MyAnimator.SetBool("LightHit", true);
                }

                else if (other.gameObject.transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Arm_Dog_Attack_J"))
                {
                    IsHitted = true;
                    GetComponentInChildren<DogController>().MyAnimator.SetBool("HeavyHit", true);
                }
                if(CurrentHealth > 0)
                {
                    UIController.Instance.HealthSlider.value = CurrentHealth;
                }
                else
                {
                    GetComponentInChildren<DogController>().MyAnimator.SetBool("Died", true);
                }
            }
        }
    }
    

    public Transform SelectedDogTransform()
    {
        return AllDogs[PlayerPrefs.GetInt("SelectedDog")].transform;
    }
    
    private void ShowSelectedPlayer()
    {
        for (int i = 0; i <= DogsMaterials.Count; i++)
        {
            if (i == PlayerPrefs.GetInt("SelectedDog"))
            {
                Skin.GetComponent<Renderer>().material = DogsMaterials[i];
                break;
            }
        }
    }
}


using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GYMDoor : MonoBehaviour
{
    private bool playerInsideTrigger = false;
    private float timer = 0f;
    [SerializeField] Transform playerTransform;
    float timeRequired = 0.5f;
    [SerializeField] Transform enterPos;
    [SerializeField] Transform exitPos;
    [SerializeField] static bool InGYM = false;

    [Header("DogSelection")]
    private bool playerInTrigger = true;
    [SerializeField] ThirdPersonController player;
    [SerializeField] GameObject[] dogsCatalogueObjects;
    [SerializeField] GYMDogSelection gymDogSelection;

    private void Update()
    {
        if (playerInsideTrigger)
        {
            timer += Time.deltaTime;

            if (timer >= timeRequired)
            {
                Debug.Log("Player stayed inside the trigger for 2 seconds.");

                if (InGYM)
                {
                    ExitGYM();
                }
                else
                {
                    EnterGYM();
                }
                Debug.Log(InGYM);

                timer = 0f;
                playerInsideTrigger = false;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            timer = 0f;
        }
    }
    private void InitDogSelection()
    {
        player.cinemachineCamera.SetActive(false);
        SetCatalogueObjectsActive(isActive: true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        player.canMove = false;

        gymDogSelection.InitGYMDog();
    }

    private void DeInitDogSelection()
    {
        SetCatalogueObjectsActive(isActive: false);
        player.cinemachineCamera.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.canMove = true;

    }

    //public void DeInit()
    //{
    //    player.cinemachineCamera.SetActive(true);
    //    SetCatalogueObjectsActive(isActive: false);

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

    //    player.canMove = true;
    //}

    private void SetCatalogueObjectsActive(bool isActive)
    {
        foreach (GameObject go in dogsCatalogueObjects)
        {
            go.SetActive(isActive);
        }
    }

    private void EnterGYM()
    {
        InGYM = true;
        playerTransform.position = enterPos.position;
        playerTransform.rotation = enterPos.rotation;
        GYMController.instance.gymDoor = this;
        InitDogSelection();
    }

    public void ExitGYM()
    {
        InGYM = false;
        playerTransform.position = exitPos.position;
        playerTransform.rotation = exitPos.rotation;
        DeInitDogSelection();
    }
}

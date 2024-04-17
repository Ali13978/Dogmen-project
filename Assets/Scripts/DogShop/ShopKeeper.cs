using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    private bool playerInTrigger = false;
    private ThirdPersonController player;
    [SerializeField] GameObject[] dogsCatalogueObjects;
    [SerializeField] GameObject[] shopKeeperObjects;

    #region UnityEvents
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            player = other.GetComponent<ThirdPersonController>();
            SetShopkeeperObjectsActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            player = null;
            SetShopkeeperObjectsActive(false);
        }
    }
    #endregion

    private void Update()
    {
        if (!playerInTrigger)
            return;

        if(Input.GetButtonDown("ActionBtn"))
        {
            Init();
        }
    }


    private void Init()
    {
        player.cinemachineCamera.SetActive(false);
        SetCatalogueObjectsActive(isActive: true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        player.canMove = false;
    }

    public void DeInit()
    {
        player.cinemachineCamera.SetActive(true);
        SetCatalogueObjectsActive(isActive: false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.canMove = true;
    }

    private  void  SetCatalogueObjectsActive(bool isActive)
    {
        foreach (GameObject go in dogsCatalogueObjects)
        {
            go.SetActive(isActive);
        }
    }
    private  void  SetShopkeeperObjectsActive(bool isActive)
    {
        foreach (GameObject go in shopKeeperObjects)
        {
            go.SetActive(isActive);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;
    [SerializeField]
    private GameObject[] m_Characters;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(GetCharacterPrefab(PhotonNetwork.LocalPlayer).name, spawnPoint.position, spawnPoint.rotation);
    }


    private GameObject GetCharacterPrefab(Player newPlayer)
    {

        if (newPlayer.CustomProperties.ContainsKey("ID"))
        {
            int result = (int)newPlayer.CustomProperties["ID"];
            Debug.Log("Character ID : " + result);
            return m_Characters[result];
        }
        else
        {
            return m_Characters[0];
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

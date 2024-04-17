using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerDogSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerDogPrefab;
    [SerializeField] List<Transform> dogSpawnPoints;
    [SerializeField] Transform dogsHolder;
    [SerializeField] List<GameObject> spawnedPlayerDogs;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LoadAllPlayerDogs();
        }
    }

    public void LoadAllPlayerDogs()
    {
        for (int i = 0; i < spawnedPlayerDogs.Count; i++)
        {
            Destroy(spawnedPlayerDogs[i].gameObject);
        }

        spawnedPlayerDogs.Clear();

        List<DogSO> data = DogsSaveLoad.instance.FetchAllDogs();

        foreach(DogSO d in data)
        {
            int index = Random.Range(0, dogSpawnPoints.Count);

            GameObject homeDog = Instantiate(playerDogPrefab, dogSpawnPoints[index].position, Quaternion.identity, dogsHolder);
            spawnedPlayerDogs.Add(homeDog);
            HomeDog _dog = homeDog.GetComponent<HomeDog>();
            _dog.InitShopDog(d.index);
        }
    }
}

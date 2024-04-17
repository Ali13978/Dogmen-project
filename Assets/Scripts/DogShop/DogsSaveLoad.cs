using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DogsSaveLoad : MonoBehaviour
{
    private const string fileName = "Dogs.json";
    [SerializeField] List<DogSO> dogs = new List<DogSO>();
    #region Singleton
    public static DogsSaveLoad instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        LoadAllDogs();
    }

    public void LoadAllDogs()
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Dog file does not exist.");
            return;
        }

        string jsonData = File.ReadAllText(filePath);
        DogListWrapper dogListWrapper = DogListWrapper.FromJson(jsonData);

        dogs = dogListWrapper.wrapperDogs;
    }

    public List<DogSO> FetchAllDogs()
    {
        return dogs;
    }

    public DogSO GetDogById(int _index)
    {
        foreach (DogSO dogObj in dogs)
        {
            if (dogObj.index == _index)
            {
                return dogObj;
            }
        }
        return null;
    }
    public void SaveDog(DogSO dog)
    {
        dogs.Add(dog);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        DogListWrapper dogListWrapper = new DogListWrapper { wrapperDogs = dogs };
        string dogJson = dogListWrapper.ToJson();
        Debug.Log(dogJson);
        File.WriteAllText(filePath, dogJson);
    }
}

[System.Serializable]
public class DogListWrapper
{
    public List<DogSO> wrapperDogs;

    public string ToJson()
    {
        string json = "[";
        foreach (var dogObj in wrapperDogs)
        {
            json += dogObj.ToJson();
            json += ",";
        }
        json = json.TrimEnd(',') + "]";
        return json;
    }
    public static DogListWrapper FromJson(string json)
    {
        DogListWrapper wrapper = new DogListWrapper();
        wrapper.wrapperDogs = new List<DogSO>();

        List<Dictionary<string, object>> dogDicts = JsonHelper.FromJson(json);
        foreach (var dogDict in dogDicts)
        {
            DogSO dog = new DogSO();
            foreach (var kvp in dogDict)
            {
                //Debug.Log("\"index");
                string key = kvp.Key;
                object value = kvp.Value;
                
                int index;
                switch (key)
                {
                    case "{\"index":
                        if (int.TryParse(value.ToString(), out index))
                            dog.index = index;
                        else
                            Debug.LogWarning("Failed to parse index value: " + value);
                        break;
                    case "index":
                        if (int.TryParse(value.ToString(), out index))
                            dog.index = index;
                        else
                            Debug.LogWarning("Failed to parse index value: " + value);
                        break;
                    case "isPurchased":
                        dog.isPurchased = (bool)value;
                        break;
                    case "willPower":
                        dog.willPower = (float)value;
                        break;
                    case "attackDamagePower":
                        dog.attackDamagePower = (float)value;
                        break;
                    case "moveSpeedPower":
                        dog.moveSpeedPower = (float)value;
                        break;
                    case "stamina":
                        dog.stamina = (float)value;
                        break;
                    default:
                        // Handle unknown key or value
                        break;
                }
            }
            wrapper.wrapperDogs.Add(dog);
            Debug.Log(dog.index + "\n" + dog.isPurchased + "\n" + dog.attackDamagePower + "\n" + dog.moveSpeedPower);

        }

        return wrapper;
    }




}

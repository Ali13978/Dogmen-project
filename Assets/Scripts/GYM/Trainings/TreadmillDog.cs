using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillDog : MonoBehaviour
{
    [SerializeField] List<GameObject> AllDogs;
    [SerializeField] Animator dogAnim;

    public void InitDog(DogSO _dog)
    {
        foreach (GameObject dog in AllDogs)
        {
            dog.SetActive(false);
        }
        AllDogs[_dog.index].SetActive(true);
    }



    public void EnableWalkAnim()
    {
        dogAnim.SetFloat("Movement_f", 0.7f);
    }

    public void EnableIdleAnim()
    {
        dogAnim.SetFloat("Movement_f", 0f);
    }
}

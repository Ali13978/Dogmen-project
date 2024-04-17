using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dog", menuName = "ScriptableObjects/DogSO", order = 1)]
public class DogSO : ScriptableObject
{
    public int index;
    public bool isPurchased;
    //Add Dog Stats

    [Range(0f, 1f)]
    public float willPower;

    [Range(0f, 1f)]
    public float attackDamagePower;

    [Range(0f, 1f)]
    public float moveSpeedPower;

    [Range(0f, 1f)] 
    public float stamina;
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

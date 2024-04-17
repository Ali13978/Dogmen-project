
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DogList", menuName = "ScriptableObjects/DogListSO", order = 1)]
public class DogList : ScriptableObject
{
    public List<DogSO> dogs = new List<DogSO>();
}
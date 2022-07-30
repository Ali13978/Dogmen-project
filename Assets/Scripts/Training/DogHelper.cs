using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogHelper : MonoBehaviour
{
    public void EndSlipped()
    {
        FindObjectOfType<TrainingManager>().TurnOnTrainingModeSelection();
    }
    public void StartHorizontalMovement()
    {
        transform.parent.parent.GetComponent<TrainDog>().AttachedWithTire = true;
    }
    public void TurnOffHorizontalMovement()
    {
        transform.parent.parent.GetComponent<TrainDog>().AttachedWithTire = false;
    }
}

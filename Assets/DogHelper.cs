using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogHelper : MonoBehaviour
{
    public void EndSlipped()
    {
        FindObjectOfType<TrainingManager>().TurnOnTrainingModeSelection();
    }
}

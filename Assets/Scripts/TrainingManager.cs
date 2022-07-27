using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] GameObject Tyre;
    [SerializeField] GameObject TreadMill;
    [SerializeField] GameObject TrainingModeMenu;
    [SerializeField] GameObject TrainingModeButt;
    // Start is called before the first frame update
    void Start()
    {
        TurnOnTrainingModeSelection();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TurnOffAllMachines()
    {
        Tyre.SetActive(false);
        TreadMill.SetActive(false);
    }

    public void TurnOnTrainingModeSelection()
    {
        TurnOffAllMachines();
        Tyre.GetComponentInChildren<Animator>().SetBool("IsRunning", false);
        TrainingModeButt.SetActive(false);
        TrainingModeMenu.SetActive(true);
    }

    public void TyreSelected()
    {
        TurnOffAllMachines();
        TrainingModeMenu.SetActive(false);
        TrainingModeButt.SetActive(true);
        Tyre.GetComponentInChildren<Animator>().SetBool("IsSlipped", false);
        Tyre.GetComponentInChildren<Animator>().SetBool("IsRunning", false);
        Tyre.SetActive(true);
    }

    public void TreadMillSelected()
    {
        TurnOffAllMachines();
        TrainingModeButt.SetActive(true);
        TreadMill.SetActive(true);
        TreadMill.GetComponent<TrainDog>().CurrentValue = TreadMill.GetComponent<TrainDog>().SliderMaxValue;
        TreadMill.GetComponentInChildren<Animator>().SetBool("IsSlipped", false);
        TreadMill.GetComponentInChildren<Animator>().SetBool("IsRunning", true);
        TrainingModeMenu.SetActive(false);
    }
}

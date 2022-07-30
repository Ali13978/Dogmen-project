using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] GameObject Tyre;
    [SerializeField] GameObject TreadMill;
    [SerializeField] GameObject TrainingModePannel;
    [SerializeField] GameObject PausePannel;
    // Start is called before the first frame update
    void Start()
    {
        PausePannel.SetActive(false);
        TurnOnTrainingModeSelection();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !PausePannel.activeInHierarchy && !TrainingModePannel.activeInHierarchy)
        {
            Time.timeScale = 0f;
            PausePannel.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && PausePannel.activeInHierarchy && !TrainingModePannel.activeInHierarchy)
        {
            ResumeGame();
        }
    }

    void TurnOffAllMachines()
    {
        Tyre.SetActive(false);
        TreadMill.SetActive(false);
    }

    public void TurnOnTrainingModeSelection()
    {
        var objects = FindObjectsOfType<TrainDog>();
        foreach (var i in objects)
        {
            i.GetComponent<TrainDog>().ResetWheelTransform();
        }
        TurnOffAllMachines();
        Tyre.GetComponentInChildren<Animator>().SetBool("IsRunning", false);
        PausePannel.SetActive(false);
        Time.timeScale = 1f;
        TrainingModePannel.SetActive(true);
    }

    public void TyreSelected()
    {
        TurnOffAllMachines();
        TrainingModePannel.SetActive(false);
        Tyre.GetComponentInChildren<Animator>().SetBool("IsSlipped", false);
        Tyre.GetComponentInChildren<Animator>().SetBool("IsRunning", false);
        Tyre.SetActive(true);
        var objects = FindObjectsOfType<TrainDog>();
        foreach(var i in objects)
        {
            i.GetComponent<TrainDog>().TurnOnTyreOn();
        }
    }

    public void TreadMillSelected()
    {
        TurnOffAllMachines();
        TreadMill.SetActive(true);
        TreadMill.GetComponent<TrainDog>().CurrentValue = TreadMill.GetComponent<TrainDog>().SliderMaxValue;
        TreadMill.GetComponentInChildren<Animator>().SetBool("IsSlipped", false);
        TreadMill.GetComponentInChildren<Animator>().SetBool("IsRunning", true);
        TrainingModePannel.SetActive(false);
    }

    public void ResumeGame()
    {
        PausePannel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LoadMainManu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

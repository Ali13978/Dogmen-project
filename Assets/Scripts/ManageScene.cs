using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    public void LoadTrainingScene()
    {
        MainMenuController.instance.CloseMenus();
        MainMenuController.instance.loadingScreen.SetActive(true);
        MainMenuController.instance.loadingText.text = "Loading training...";
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadFightScene()
    {
        SceneManager.LoadScene(2);
    }
}

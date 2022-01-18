using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;

    private void Start()
    {
        if(menuPanel!=null)
            menuPanel.SetActive(false);
    }

    public void NewGame()
    {
        LevelManager.instance.NewGame();
    }

    public void MainMenu()
    {
        LevelManager.instance.MainMenu();
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
        #endif
    }

    public void Resume()
    {
        LevelManager.instance.ResumeGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && LevelManager.instance.canBePaused)
        {
            if (LevelManager.instance.isPaused)
            {
                menuPanel.SetActive(false);
                LevelManager.instance.ResumeGame();
            }
            else
            {
                menuPanel.SetActive(true);
                LevelManager.instance.PauseGame();
            }
        }
    }
}

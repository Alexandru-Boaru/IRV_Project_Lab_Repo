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
        Time.timeScale = 1;
        LevelManager.instance.NewGame();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
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
        menuPanel.SetActive(false);
        LevelManager.instance.ResumeGame();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && LevelManager.instance.canBePaused && !PlayerStats.instance.dead)
        {
            if (LevelManager.instance.isPaused)
            {
                Resume();
            }
            else
            {
                menuPanel.SetActive(true);
                LevelManager.instance.PauseGame();
                Time.timeScale = 0;
            }
        }
    }
}

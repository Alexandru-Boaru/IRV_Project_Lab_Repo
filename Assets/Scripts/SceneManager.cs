using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    public bool isPaused;
    public bool canBePaused;

    // Start is called before the first frame update
    void Start()
    {
        //launch menu UI

    }

    public void PauseGame()
    {
        if (!canBePaused)
            return;
        isPaused = true;
        //freeze all rigibodies
        //put up menu
    }

    public void ResumeGame()
    {
        isPaused = false;
        //unfreeze all rigidbodies
        //put down menu
    }
    public void LoadLevel()
    {
        //Spawn player in train
        //Run train
        //Randomly spawn flying enemies
        //Generate maze
        //Add obstacles prefabs
        //Add enemies spawn points
        //Add collectibles

    }

    public void StartLevel()
    {
        //animate transition to maze
        //activate player
        //spawn enemies
    }

    public void EndLevel()
    {
        //freeze all rigibodies
        //animate transition to train
    }

    public void RestartGame()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

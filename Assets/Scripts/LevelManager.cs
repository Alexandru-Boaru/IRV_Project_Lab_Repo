using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public bool isPaused;
    public bool canBePaused;
    public int currentLevel = 0;

    public static LevelManager instance;

    public int mainMenuID = 0;
    public int mazeSceneID = 1;
    public int railSceneID = 2;

    public GameObject loadingPanel;
    public Animator anim;

    public GameObject player;

    void Awake()
    {
        //LevelManager[] objs = FindObjectsOfType<LevelManager>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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
        Cursor.lockState = CursorLockMode.None;
        //freeze all rigibodies
        //put up menu
    }

    public void ResumeGame()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        //unfreeze all rigidbodies
        //put down menu
    }

    public void NextLevel()
    {
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                LoadLevel(mazeSceneID, currentLevel);
                break;
            case 1:
                LoadLevel(railSceneID, currentLevel);
                break;
            case 2:
                LoadLevel(mazeSceneID, ++currentLevel);
                break;
            default:
                break;
        }
    }

    public void LoadLevel(int id, int level)
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(id);
        player.transform.position = Vector3.zero;
        if (SceneManager.GetActiveScene().buildIndex > 0)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
        if(SceneManager.GetActiveScene().buildIndex > 0)
        {
            StartLevel();
        }
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
        GameplayManager.instance.ResetCards();
        player.SetActive(true);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //animate transition to maze
        //activate player
        //spawn enemies
    }

    public void EndLevel()
    {
        //freeze all rigibodies
        //animate transition to train
    }

    public void NewGame()
    {
        GameplayManager.instance.ResetCards();
        GameplayManager.instance.ResetScore();
        currentLevel = 0;
        LoadLevel(mazeSceneID, currentLevel);
    }

    public void MainMenu()
    {
        GameplayManager.instance.ResetCards();
        GameplayManager.instance.ResetScore();
        currentLevel = 0;
        LoadLevel(0, currentLevel);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            PlayerStats ps = FindObjectOfType<PlayerStats>();
            if(ps != null)
                player = ps.gameObject;
        }
        if (player == null)
            return;
        if(SceneManager.GetActiveScene().buildIndex == 0 && player.activeInHierarchy)
        {
            player.SetActive(false);
        }
        //if(SceneManager.GetActiveScene().buildIndex == 1 && GameplayManager.instance.cards == 4)
        //{

        //}
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex > 0)
        {
            canBePaused = true;
        }
        else
        {
            canBePaused = false;
        }
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

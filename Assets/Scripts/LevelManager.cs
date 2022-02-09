using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public bool isPaused;
    public bool canBePaused;
    public int currentLevel = 1;

    public static LevelManager instance;

    public int mainMenuID = 0;
    public int mazeSceneID = 1;
    public int railSceneID = 2;

    public GameObject loadingPanel;
    public Animator anim;

    public GameObject player;
    public GameObject playerPref;

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
        
        //Spawn player in train
        //Run train
        //Randomly spawn flying enemies
        //Generate maze
        //Add obstacles prefabs
        //Add enemies spawn points
        //Add collectibles
    }

    public IEnumerator StartLevel()
    {
        GameplayManager.instance.ResetCards();
        while(player==null)
        {
            yield return null;
        }
        //player.SetActive(true);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Debug.Log(SceneManager.GetActiveScene().buildIndex.ToString() + " cretin");
        //StartCoroutine(StartPlayer());
        GenerateLevel gl = null;
        Debug.Log("RIIII");
        while (!(gl != null && gl.checkLoadCompletion()))
        {
            gl = FindObjectOfType<GenerateLevel>();
            Debug.Log("bumbum");
            yield return null;
        }

        player.transform.position = gl.GetStartingPosition();
        Debug.Log("Hehehe " + player.transform.position);
        player.SetActive(true);
        PlayerStats.instance.ShowQuestUI(SceneManager.GetActiveScene().buildIndex == 1);
        GameplayManager.instance.SetCardsToCollect(currentLevel);
        ResumeGame();
        //animate transition to maze
        //activate player
        //spawn enemies
    }

    public void EndLevel()
    {

        //freeze all rigibodies
        //animate transition to train
        Time.timeScale = 0;
        NextLevel();
    }

    public void NewGame()
    {
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("heiihiehei");
        }
        GameplayManager.instance.ResetCards();
        GameplayManager.instance.ResetScore();
        currentLevel = 1;
        LoadLevel(mazeSceneID, currentLevel);
    }

    public void MainMenu()
    {
        GameplayManager.instance.ResetCards();
        GameplayManager.instance.ResetScore();
        currentLevel = 1;
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
        if (SceneManager.GetActiveScene().buildIndex == 1 && !player.activeInHierarchy)
        {
            //GenerateLevel gl = FindObjectOfType<GenerateLevel>();
            //if (gl != null && gl.checkLoadCompletion())
            //{
            //    player.transform.position = gl.GetStartingPosition();
            //    player.SetActive(true);
            //}
        }

        if (PlayerStats.instance == null)
            Instantiate(playerPref);
        //if(SceneManager.GetActiveScene().buildIndex == 1 && GameplayManager.instance.cards == 4)
        //{

        //}
    }

    //public IEnumerator StartPlayer()
    //{
        //GenerateLevel gl = null;
        //Debug.Log("RIIII");
        //while (!(gl != null && gl.checkLoadCompletion()))
        //{
        //    gl = FindObjectOfType<GenerateLevel>();
        //    Debug.Log("bumbum");
        //    yield return null;
        //}
        
        //player.transform.position = gl.GetStartingPosition();
        //Debug.Log("Hehehe " + player.transform.position);
        //player.SetActive(true);
    //}

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
        Debug.Log(scene.buildIndex.ToString() + " cretin");

        //player.transform.position = Vector3.zero;
        if (scene.buildIndex > 0)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
        if (scene.buildIndex > 0)
        {
            StartCoroutine(StartLevel());
        }
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

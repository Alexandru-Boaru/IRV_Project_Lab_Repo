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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void PauseGame()
    {
        if (!canBePaused)
            return;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void ResumeGame()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;

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
       
    }

    public IEnumerator StartLevel()
    {
        GameplayManager.instance.ResetCards();
        while(player==null)
        {
            yield return null;
        }
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GenerateLevel gl = null;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            while (!(gl != null && gl.checkLoadCompletion()))
            {
                gl = FindObjectOfType<GenerateLevel>();
                yield return null;
            }
            player.transform.position = gl.GetStartingPosition();
        }

        
        player.SetActive(true);
        PlayerStats.instance.ShowQuestUI(SceneManager.GetActiveScene().buildIndex == 1);
        GameplayManager.instance.SetCardsToCollect(currentLevel);
        ResumeGame();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            player.GetComponent<PlayerMotion>().Freeze(false);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            player.GetComponent<PlayerMotion>().Freeze(true);
        }

    }

    public void EndLevel()
    {
        Time.timeScale = 0;
        NextLevel();
    }

    public void NewGame()
    {
        if (player != null)
        {
            player.SetActive(false);
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
            //
        }

        if (PlayerStats.instance == null)
            Instantiate(playerPref);
        
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
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

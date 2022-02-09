using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public int score;
    public int cards;
    public int cardsToCollect;

    public static GameplayManager instance;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetCards()
    {
        cards = 0;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void GetCard()
    {
        cards++;
        if (cards == cardsToCollect)
            LevelManager.instance.EndLevel();
    }

    public void UpdateScore(int s)
    {
        score += s;
    }

    public void SetCardsToCollect(int level)
    {
        cardsToCollect = 3 + Mathf.FloorToInt(Mathf.Log(level, 2));
    }
}

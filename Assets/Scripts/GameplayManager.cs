using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public int score;
    public int cards;

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
    }

    public void UpdateScore(int s)
    {
        score += s;
    }
}

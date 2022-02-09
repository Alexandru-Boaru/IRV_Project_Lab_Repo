using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : EntityStats
{
    public Image fillHpBar;
    public bool invincible = false;
    public float invincibilityTimer;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI questText;
    public static PlayerStats instance;
    public GameObject gameOverPanel;

    private void Awake()
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

    public void ResetPlayer()
    {
        invincible = false;
        hitPoints = maxHitPoints;
        fillHpBar.fillAmount = ((float)hitPoints) / maxHitPoints;
        GetComponentInChildren<PlayerShooter>().ResetPlayerGun();
    }

    public override void TakeDamage(int hp)
    {
        if (invincible)
            return;
        base.TakeDamage(hp);
        StartCoroutine(Invincibility());
        fillHpBar.fillAmount = ((float)hitPoints) / maxHitPoints;
    }

    public void Heal(int hp)
    {
        hitPoints = Mathf.Clamp(hitPoints + hp, 0, maxHitPoints);
        fillHpBar.fillAmount = ((float)hitPoints) / maxHitPoints;
    }

    protected override void Die()
    {
        dead = true;
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Final Score: {GameplayManager.instance.score}";
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        //gameOver.Invoke();
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilityTimer);
        invincible = false;
    }

    private void Update()
    {
        scoreText.text = ("Score: " + (GameplayManager.instance==null?"0":GameplayManager.instance.score.ToString()));
        questText.text = $"Collect blue boxes ({GameplayManager.instance.cards}/{GameplayManager.instance.cardsToCollect})";
    }

    public void ShowQuestUI(bool status)
    {
        questText.gameObject.SetActive(status);
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        gameOverPanel.SetActive(false);
        base.Die();
        LevelManager.instance.NewGame();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        gameOverPanel.SetActive(false);
        base.Die();
        LevelManager.instance.MainMenu();
    }

}

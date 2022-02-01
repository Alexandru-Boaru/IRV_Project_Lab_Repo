using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : EntityStats
{
    public Image fillHpBar;
    public bool invincible = false;
    public float invincibilityTimer;

    public static PlayerStats instance;

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
        //gameOver.Invoke();
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilityTimer);
        invincible = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    public int hitPoints;
    public int maxHitPoints;
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void TakeDamage(int hp)
    {
        if (dead)
            return;
        hitPoints -= hp;
        if (hitPoints < 0)
        {
            //dies
            dead = true;
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

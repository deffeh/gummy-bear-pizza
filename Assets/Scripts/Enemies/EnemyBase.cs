using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    public int MaxHp;
    public int CurHp;
    // Start is called before the first frame update
    protected void Start()
    {
        CurHp = MaxHp;
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        CurHp -= damage;
        if (CurHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}

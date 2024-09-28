using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    public int MaxHp;
    public int CurHp;

    public SpriteRenderer Sprite;
    public GameObject DeathEffect;

    private Material mat;
    // Start is called before the first frame update
    protected void Start()
    {
        CurHp = MaxHp;
        if (!Sprite)
        {
            Sprite = GetComponent<SpriteRenderer>();
        }

    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (Sprite)
        {
            StartCoroutine(DamageAnim());
        }
        
        CurHp -= damage;
        if (CurHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageAnim()
    {
        float t = .6f;
        while (t > 0)
        {
            t -= Time.deltaTime;
            Sprite.color = Color.Lerp(Color.white, Color.red, t/.6f);
            yield return null;
        }
        Sprite.color = Color.white;
    }
    
    public void Die()
    {
        if (DeathEffect)
            Instantiate(DeathEffect, transform.position, quaternion.identity, null);
        Destroy(gameObject);
    }
}

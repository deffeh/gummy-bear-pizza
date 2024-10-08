using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    public int MaxHp;
    public int CurHp;

    public SpriteRenderer Sprite;
    public GameObject DeathEffect;
    public AudioSource DamageTakenSFX;
    public GameObject CurrentRoot;

    public GameObject Sandwich;
    public GameObject DogTreat;

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
        if(DamageTakenSFX)
            DamageTakenSFX.Play();
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
    
    public virtual void Die()
    {
        if (DeathEffect)
        {
            Instantiate(DeathEffect, transform.position, Quaternion.identity, null);
            CameraShake.Shake(1);
        }

        if (DogTreat && Sandwich)
        {
            if (Random.Range(0, 10) == 0)
                Instantiate(Random.Range(0,2) == 0 ? Sandwich : DogTreat, transform.position, Quaternion.identity, null);
        }
      
        if (!CurrentRoot)
            Destroy(transform.root.gameObject);
        else
            Destroy(CurrentRoot);
    }
}

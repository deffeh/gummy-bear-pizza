using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPigeon : EnemyBase
{
    public PigeonState CurState;
    public AnimationCurve FlightCurve;
    public float FlightStrength = 100;
    public float FlightRate = .6f;
    public float FireRate = 2f;
    public GameObject PigeonProjectile;
    private Rigidbody rb;
    public AudioSource audiosrc;
    private float t = 0;
    
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
        CurState = PigeonState.Idle;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(FireProjectile());
        t += Random.Range(-100f, 100f);
    }


    // Update is called once per frame
    void Update()
    {
        t = (t + (Time.deltaTime * FlightRate)) % 1f;
        rb.AddForce(Vector3.up * FlightCurve.Evaluate(t) * FlightStrength * Time.deltaTime * rb.mass);
        switch (CurState)
        {
            case PigeonState.Attack:
                Attack();
                break;
            case PigeonState.Idle:
                break;
        }
    }

    private IEnumerator FireProjectile()
    {
        yield return new WaitForSeconds(Random.Range(0f, 5f));

        float t = 0;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= FireRate)
            {
                t = 0;
                audiosrc.Play();
                Instantiate(PigeonProjectile, transform.position, quaternion.identity, null);
            }
            yield return null;
        }
    }
    
    private void Attack()
    {
        
    }
}

public enum PigeonState
{
    Attack,
    Idle
}
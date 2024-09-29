using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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
    
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
        CurState = PigeonState.Idle;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(FireProjectile());
    }


    // Update is called once per frame
    void Update()
    {
        float t = (Time.time * FlightRate) % 1f;
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
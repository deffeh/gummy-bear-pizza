using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPigeon : EnemyBase
{
    public PigeonState CurState;
    public AnimationCurve FlightSpeed;
    private Rigidbody rb;
    
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
        CurState = PigeonState.Idle;
        rb = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        float t = (Time.time * .5f) % 1f;
        print(t);
        rb.AddForce(Vector3.up * FlightSpeed.Evaluate(t));
        switch (CurState)
        {
            case PigeonState.Attack:
                Attack();
                break;
            case PigeonState.Idle:
                break;
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum SkunkState {
    Idle,
    Active,
    Farting,
    FartRecovery
}

public class Skunk : EnemyBase
{
    [Header("Components")]
    public Rigidbody player; //TODO test
    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    [Header("Stats")]
    public float Speed = 10f;
    public float Damage = 25f;
    [Header("State Stuff")]
    public SkunkState curState;
    public float FartTriggerDist = 30f;
    public float FartDuration = 5f;
    public float FartRecoveryDuration = 3f;
    public float FartDist = 5f;
    public float FartWalkSpeed = 2f;
    public float FartRadius = 5f;
    private float lerpVal;
    

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        NavAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        NavAgent.speed = Speed;
        // curState = SkunkState.Idle;
        SetToActive();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (curState) {
            case SkunkState.Idle:
            break;

            case SkunkState.Active:
            ActiveState();
            break;

            case SkunkState.Farting:
            FartingState();
            break;

            case SkunkState.FartRecovery:
            FartRecoveryState();
            break;
        }
    }

    public void SetToActive() {
        UpdateState(SkunkState.Active);
    }

    private void ActiveState() {
        float distToPlayer = Vector3.Distance(rb.position, player.position);
        if (distToPlayer > FartTriggerDist)
        {
            rb.rotation.SetLookRotation(player.position);
            NavAgent.destination = player.position;
        } 
        else 
        {
            UpdateState(SkunkState.Farting);
        }
    }

    private void FartingState() {
        lerpVal += Time.deltaTime / FartDuration;
        if (lerpVal >= 1) {
            UpdateState(SkunkState.FartRecovery);
        }
        NavAgent.destination = rb.position;
    }

    private void FartRecoveryState() {
        lerpVal += Time.deltaTime / FartRecoveryDuration;
        if (lerpVal >= 1) {
            UpdateState(SkunkState.Active);
        }
    }

    private void UpdateState(SkunkState state) {
        switch (state) {
            case SkunkState.Farting:
                lerpVal = 0;
                rb.freezeRotation = true;
                SprayGas();
            break;
            case SkunkState.FartRecovery:
                lerpVal = 0;
                rb.freezeRotation = false;
            break;
        }
        curState = state;
    }

    private void SprayGas() {
        var startPos = rb.position + transform.forward * FartDist;
        Physics.SphereCast(origin: startPos, FartRadius, Vector3.forward, out RaycastHit hit);
    }

    private void OnDrawGizmos() {
        if (curState == SkunkState.Farting) {
            var startPos = rb.position + transform.forward * FartDist;
            Gizmos.DrawSphere(startPos, FartRadius);
        }
    }
}

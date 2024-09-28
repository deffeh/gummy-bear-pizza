using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum ChihuahuaState {
    Idle,
    Active,
    Biting,
    BitingRecovery
}

public class Chihuahua : EnemyBase
{
    [Header("Components")]
    public Rigidbody player; //TODO test
    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    [Header("Stats")]
    public float Speed = 10f;
    public float Damage = 25f;
    [Header("State Stuff")]
    public ChihuahuaState curState;

    public float BiteTriggerDist = 1f;
    public float BiteDuration = 5f;
    public float BiteRecoveryDuration = 3f;
    public float BiteDist = 5f;
    public float BiteWalkSpeed = 2f;
    public float BiteRadius = 5f;
    private float lerpVal;
    

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        NavAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        NavAgent.speed = Speed;
        //curState = ChihuahuaState.Idle;
        SetToActive();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (curState) {
            case ChihuahuaState.Idle:
            break;

            case ChihuahuaState.Active:
            ActiveState();
            break;

            case ChihuahuaState.Biting:
            BitingState();
            break;

            case ChihuahuaState.BitingRecovery:
            BitingRecoveryState();
            break;
        }
    }

    public void SetToActive() {
        UpdateState(ChihuahuaState.Active);
    }

    private void ActiveState() {
        float distToPlayer = Vector3.Distance(rb.position, player.position);

        if (distToPlayer > BiteTriggerDist)
        {
            rb.rotation.SetLookRotation(player.position);
            NavAgent.destination = player.position;
        } 
        else 
        {
            UpdateState(ChihuahuaState.Biting);
        }
    }

    private void BitingState() {
        lerpVal += Time.deltaTime / BiteDuration;
        if (lerpVal >= 1) {
            UpdateState(ChihuahuaState.BitingRecovery);
        }
        NavAgent.destination = rb.position;
    }

    private void BitingRecoveryState() {
        lerpVal += Time.deltaTime / BiteRecoveryDuration;
        if (lerpVal >= 1) {
            UpdateState(ChihuahuaState.Active);
        }
    }

    private void UpdateState(ChihuahuaState state) {
        switch (state) {
            case ChihuahuaState.Biting:
                lerpVal = 0;
                rb.freezeRotation = true;
                BiteThings();
            break;
            case ChihuahuaState.BitingRecovery:
                lerpVal = 0;
                rb.freezeRotation = false;
            break;
        }
        curState = state;
    }

    private void BiteThings() {
        var startPos = rb.position + transform.forward * BiteDist;
        Physics.SphereCast(origin: startPos, BiteRadius, Vector3.forward, out RaycastHit hit);
    }
}

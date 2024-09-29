using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
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
    [HideInInspector] public Rigidbody player;
    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    [Header("Stats")]
    public float Speed = 10f;
    public int Damage = 10;
    [Header("State Stuff")]
    public ChihuahuaState curState;

    public float BiteTriggerDist = 1f;
    public float BiteRecoveryDuration = 0.5f;
    public float BiteDist = 5f;
    public float BiteRadius = 5f;
    public LayerMask mask;
    

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        player = Player.Instance.GetComponent<Rigidbody>();
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
            break;

            case ChihuahuaState.BitingRecovery:
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

    private void BitingRecoveryState() {
        var seq = DOTween.Sequence();
        seq.AppendInterval(BiteRecoveryDuration);
        seq.OnComplete(() => {
            UpdateState(ChihuahuaState.Active);
        });
        seq.Play();
    }

    private void UpdateState(ChihuahuaState state) {
        switch (state) {
            case ChihuahuaState.Biting:
                rb.freezeRotation = true;
                BiteThings();
            break;
            case ChihuahuaState.BitingRecovery:
                rb.freezeRotation = false;
                BitingRecoveryState();
            break;
        }
        curState = state;
    }

    private void BiteThings() {
        var startPos = transform.position + transform.forward * BiteDist;
        var colls = Physics.OverlapSphere(startPos, BiteRadius, mask);
        foreach (var col in colls) {
            if (col.GetComponent<Player>()) {
                col.GetComponent<Player>().OnHit(Damage);
            } else if (col.GetComponent<Human>()) {
                col.GetComponent<Human>().OnHit(Damage);
            }
        }
        UpdateState(ChihuahuaState.BitingRecovery);
    }
}

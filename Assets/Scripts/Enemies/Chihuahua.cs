using System;
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
    [HideInInspector] public Rigidbody human;

    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    [Header("Stats")]
    public float Speed = 10f;
    public int Damage = 10;
    [Header("State Stuff")]
    public ChihuahuaState curState;
    public float PlayerTriggerDist = 10f;
    public float BiteTriggerDist = 1f;
    public float BiteRecoveryDuration = 0.5f;
    public float BiteDist = 5f;
    public float BiteRadius = 5f;
    public LayerMask mask;
    public AudioSource barksrc;
    public Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        if (PersistData.Instance) {
            Difficulty currDiff = PersistData.Instance.CurrDifficulty;
            if (currDiff == Difficulty.Easy) {
                MaxHp = 1;
                Damage /= 2;
            } else if (currDiff == Difficulty.Helldogger) {
                MaxHp *= 2;
                Damage = (int) ((float)Damage * 1.5f); //raw dog
            }
        }
        base.Start();
        player = Player.Instance.GetComponent<Rigidbody>();
        human = Human.Instance.GetComponent<Rigidbody>();
        NavAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        NavAgent.speed = Speed;
        curState = ChihuahuaState.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (curState) {
            case ChihuahuaState.Idle:
            SearchForPlayer();
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

     private void SearchForPlayer() {
        animator.Play("ChihuahuaIdle");
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distToHuman = Vector3.Distance(transform.position, human.position);
        
        if (distToPlayer < PlayerTriggerDist || distToHuman < PlayerTriggerDist) {
            SetToActive();
        }
    }

    public void SetToActive() {
        UpdateState(ChihuahuaState.Active);
    }

      private void ActiveState() {
        animator.Play("ChihuahuaRun");
        float distToPlayer = Vector3.Distance(rb.position, player.position);
        float distToHuman = Vector3.Distance(rb.position, human.position);
        float dist = Math.Min(distToPlayer, distToHuman);
        Rigidbody tgt = distToPlayer < distToHuman ? player : human;
        if (dist > BiteTriggerDist)
        {   
            rb.rotation.SetLookRotation(tgt.position);
            NavAgent.destination = tgt.position;
        } 
        else 
        {
            UpdateState(ChihuahuaState.Biting);
        }
    }

    private void BitingRecoveryState() {
        animator.Play("ChihuahuaIdle");
        var seq = DOTween.Sequence();
        seq.AppendInterval(BiteRecoveryDuration);
        seq.OnComplete(() => {
            UpdateState(ChihuahuaState.Idle);
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
        animator.Play("ChihuahuaIdle");
        barksrc.Play();
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

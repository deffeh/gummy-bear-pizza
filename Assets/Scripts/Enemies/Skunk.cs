using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum SkunkState {
    Idle,
    Active,
    FartCharge,
    Farting,
    FartRecovery
}

public class Skunk : EnemyBase
{
    [Header("Components")]
    [HideInInspector] public Rigidbody player;
    [HideInInspector] public Rigidbody human;
    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    [Header("Stats")]
    public float Speed = 10f;
    public int Damage = 20;
    [Header("State Stuff")]
    public SkunkState curState;
    public float PlayerTriggerDist = 10f;
    public float FartTriggerDist = 30f;
    public float FartChargeDuration = 3f;
    public float FartRecoveryDuration = 3f;
    public float FartDist = 5f;
    public float FartWalkSpeed = 2f;
    public float FartRadius = 5f;
    public Animator animator;
    public ParticleSystem fartCloud;
    public LayerMask mask;
    private float seqLerpVal;
    public List<AudioClip> farts; //peak humor
    

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
        curState = SkunkState.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (curState) {
            case SkunkState.Idle:
            SearchForPlayer();
            break;

            case SkunkState.Active:
            ActiveState();
            break;

            case SkunkState.FartCharge:
            break;

            case SkunkState.Farting:
            break;

            case SkunkState.FartRecovery:
            FartRecoveryState();
            break;
        }
    }

    private void SearchForPlayer() {
        animator.Play("SkunkIdle");
        float distToPlayer = Vector3.Distance(rb.position, player.position);
        float distToHuman = Vector3.Distance(rb.position, human.position);
        if (distToPlayer < PlayerTriggerDist || distToHuman < PlayerTriggerDist) {
            SetToActive();
        }
    }

    public void SetToActive() {
        UpdateState(SkunkState.Active);
    }

    private void ActiveState() {
        float distToPlayer = Vector3.Distance(rb.position, player.position);
        float distToHuman = Vector3.Distance(rb.position, human.position);
        float dist = Math.Min(distToPlayer, distToHuman);
        Rigidbody tgt = distToPlayer < distToHuman ? player : human;
        if (dist > FartTriggerDist)
        {   
            rb.rotation.SetLookRotation(tgt.position);
            NavAgent.destination = tgt.position;
        } 
        else 
        {
            UpdateState(SkunkState.FartCharge);
        }
    }

    private void FartCharge() {
        animator.Play("SkunkBlast");
        NavAgent.destination = rb.position;
        seqLerpVal = 1f;
        var seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => seqLerpVal, x => seqLerpVal = x, 100f, FartChargeDuration));
        seq.OnUpdate(() => {
            Sprite.material.SetFloat("_Brightness", seqLerpVal);
        });
        seq.OnComplete( () => {
            Sprite.material.SetFloat("_Brightness", 0.2f);
            UpdateState(SkunkState.Farting);
        });
        seq.Play();
    }

    private void FartRecoveryState() {
        animator.Play("SkunkBlast");
        var seq = DOTween.Sequence();
        seq.AppendInterval(FartRecoveryDuration);
        seq.AppendCallback(() => {
            Sprite.material.SetFloat("_Brightness", 1f);
            UpdateState(SkunkState.Idle);
        });
        seq.Play();
    }

    private void UpdateState(SkunkState state) {
        switch (state) {
            case SkunkState.Active:
                animator.Play("SkunkRun");
                Sprite.material.SetFloat("_Brightness", 1f);
            break;

            case SkunkState.FartCharge:
                FartCharge();
            break;

            case SkunkState.Farting:
                rb.freezeRotation = true;
                SprayGas();
            break;

            case SkunkState.FartRecovery:
                rb.freezeRotation = false;
                FartRecoveryState();
            break;
        }
        curState = state;
    }

    private void SprayGas() {
        var startPos = transform.position + (Vector3.up * 0.5f);
        var colls = Physics.OverlapSphere(startPos, FartRadius, mask);
        foreach (var col in colls) {
            if (col.GetComponent<Player>()) {
                col.GetComponent<Player>().OnHit(Damage);
            } else if (col.GetComponent<Human>()) {
                col.GetComponent<Human>().OnHit(Damage);
            }
        }

        GameObject cloud = Instantiate(fartCloud.gameObject, startPos, Quaternion.identity);
        var clip = farts[UnityEngine.Random.Range(0, farts.Count)];
        cloud.GetComponent<AudioSource>().clip = clip;
        cloud.GetComponent<AudioSource>().Play();
        UpdateState(SkunkState.FartRecovery);
    }

    private void OnDrawGizmos() {
        if (curState == SkunkState.Farting) {
            var startPos = transform.position + (Vector3.up * 1f);
            Gizmos.DrawSphere(startPos, FartRadius);
        }
    }
}

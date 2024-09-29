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
    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    [Header("Stats")]
    public float Speed = 10f;
    public int Damage = 20;
    [Header("State Stuff")]
    public SkunkState curState;
    public float FartTriggerDist = 30f;
    public float FartChargeDuration = 3f;
    public float FartRecoveryDuration = 3f;
    public float FartDist = 5f;
    public float FartWalkSpeed = 2f;
    public float FartRadius = 5f;
    public ParticleSystem fartCloud;
    public LayerMask mask;
    private float seqLerpVal;
    

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        player = Player.Instance.GetComponent<Rigidbody>();
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

            case SkunkState.FartCharge:
            break;

            case SkunkState.Farting:
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
            UpdateState(SkunkState.FartCharge);
        }
    }

    private void FartCharge() {
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
        var seq = DOTween.Sequence();
        seq.AppendInterval(FartRecoveryDuration);
        seq.AppendCallback(() => {
            Debug.Log("Recovery");
            Sprite.material.SetFloat("_Brightness", 1f);
            UpdateState(SkunkState.Active);
        });
        seq.Play();
    }

    private void UpdateState(SkunkState state) {
        switch (state) {
            case SkunkState.Active:
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
        var startPos = transform.position + (Vector3.up * 1f);
        var colls = Physics.OverlapSphere(startPos, FartRadius, ~mask);
        foreach (var col in colls) {
            if (col.GetComponent<Player>()) {
                col.GetComponent<Player>().OnHit(Damage);
            } else if (col.GetComponent<Human>()) {
                col.GetComponent<Human>().OnHit(Damage);
            }
        }
        Instantiate(fartCloud, startPos, Quaternion.identity);
        UpdateState(SkunkState.FartRecovery);
    }

    private void OnDrawGizmos() {
        if (curState == SkunkState.Farting) {
            var startPos = transform.position + (Vector3.up * 1f);
            Gizmos.DrawSphere(startPos, FartRadius);
        }
    }
}

using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum SquirrelState {
    Idle,
    Searching,
    Attacking,
    AttackRecovery
}

public class Squirrel : EnemyBase
{
    SquirrelState curState;
    public GameObject projectilePrefab;
    public NavMeshAgent NavAgent;
    public Rigidbody player;
    public Rigidbody human;
    public Rigidbody rb;
    public int Damage = 5;
    public float Speed = 5f;
    public float projectileSpeed = 8f;
    public float projectileSpawnDistFromPlayer = 0.5f;
    public float AttackCooldownLength = 2f;
    public float MaxAttackRange = 15f;
    public float AggressionRange = 20f;
    public LayerMask layerMask;
    private Rigidbody curTarget;
    private float attackDelayLength;
    private bool HasLineOfSight = false;
    private bool CanAttack = true;
    private Animator animator;
    public AudioSource audiosrc;


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
        NavAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        player = Player.Instance.GetComponent<Rigidbody>();
        human = Human.Instance.GetComponent<Rigidbody>();
        curTarget = FindClosestTarget();
        attackDelayLength = UnityEngine.Random.Range(0, 4) * 0.5f;
        NavAgent.speed = Speed;
        NavAgent.stoppingDistance = MaxAttackRange;
        
        UpdateState(SquirrelState.Idle);
    }

    void FixedUpdate()
    {
        switch (curState) 
        {
            case SquirrelState.Idle:
            IdleState();
            break;

            case SquirrelState.Searching:
            SearchingState();
            break;

            case SquirrelState.Attacking:
            AttackingState();
            break;

            case SquirrelState.AttackRecovery:
            break;
        }
    }

    void Update()
    {
        base.Update();
        HasLineOfSight = CheckLineOfSight();
        curTarget = FindClosestTarget();
    }

    Rigidbody FindClosestTarget()
    {
         
        float distFromPlayer = (player.position - transform.position).magnitude;
        float distFromHuman = (human.position - transform.position).magnitude;
        return (distFromPlayer < distFromHuman) ? player : human;
    }
    
    bool CheckLineOfSight()
    {
        Vector3 diff = curTarget.position - transform.position;
        if (Physics.Raycast(transform.position, diff.normalized, out RaycastHit hit, 1000f, layerMask))
        {
            Player collidingPlayer = hit.collider.GetComponent<Player>();
            Human collidingHuman = hit.collider.GetComponent<Human>();

            return !collidingPlayer.IsUnityNull() || !collidingHuman.IsUnityNull();
        }

        Debug.Log("hit not detected");

        return false;
    }

    void UpdateState(SquirrelState newState)
    {
        curState = newState;
    }

    void IdleState() {
        NavAgent.isStopped = true;
        animator.Play("Squirrel_Idle");

        float distToTarget = (curTarget.position - transform.position).magnitude;
        if (distToTarget < AggressionRange && HasLineOfSight)
        {
            NavAgent.isStopped = false;
            UpdateState(SquirrelState.Searching);
        }
    }

    private void SearchingState()
    {
        animator.Play("Squirrel_Run");

        Vector3 direction = curTarget.position - transform.position;
        float targetDist = direction.magnitude;

        if (targetDist > MaxAttackRange || !HasLineOfSight)
        {
            NavAgent.destination = curTarget.position;
            NavAgent.stoppingDistance = 0;
        }
        else
        {
            StartCoroutine(RandomDelayedSwitchToAttacking());
        }
    }

    IEnumerator RandomDelayedSwitchToAttacking()
    {
        yield return new WaitForSeconds(attackDelayLength);
        UpdateState(SquirrelState.Attacking);
    }

    private void AttackingState()
    {
        animator.Play("Squirrel_Idle");
        float targetDist = Vector3.Distance(rb.position, curTarget.position);

        if (targetDist <= MaxAttackRange && HasLineOfSight)
        {
            NavAgent.stoppingDistance = MaxAttackRange;
            if (CanAttack && projectilePrefab)
            {
                ThrowAcorn();
                StartCoroutine(StartCooldown());
            }
        }
        else
        {
            UpdateState(SquirrelState.Searching);
        }
    }

    IEnumerator StartCooldown()
    {
        CanAttack = false;
        yield return new WaitForSeconds(AttackCooldownLength);
        CanAttack = true;
    }

    private void ThrowAcorn()
    {
        audiosrc.Play();
        Vector3 shootDir = (curTarget.position - new Vector3(0f, 0.3f, 0f) - transform.position).normalized;
        Vector3 spawnPos = rb.position + new Vector3(0f, 0.5f, 0f);
        
        Instantiate(projectilePrefab, spawnPos, quaternion.identity)
            .GetComponent<AcornProjectile>().Init(projectileSpeed, shootDir, Damage);
    }
}
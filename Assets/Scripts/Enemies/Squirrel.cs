using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    public Rigidbody rb;
    public int Damage = 5;
    public float Speed = 5f;
    public float projectileSpeed = 8f;
    public float projectileSpawnDistFromPlayer = 0.5f;
    public float AttackCooldownLength = 2f;
    public float MaxAttackRange = 15f;
    public float AggressionRange = 20f;
    public LayerMask layerMask;
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
        attackDelayLength = UnityEngine.Random.Range(0, 4) * 0.5f;
        NavAgent.speed = Speed;
        NavAgent.stoppingDistance = MaxAttackRange;
        
        UpdateState(SquirrelState.Idle);
    }

    void FixedUpdate()
    {
        Debug.Log("Current State: " + curState.ToString());

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
        // Debug.Log("Has Line of Sight: " + HasLineOfSight);
    }
    
    bool CheckLineOfSight()
    {
        Vector3 diff = player.position - rb.position;
        Debug.Log("Casting ray");
        if (Physics.Raycast(transform.position, diff.normalized, out RaycastHit hit, 1000f, layerMask))
        {
            // Debug.Log("hit detected");
            // Debug.Log("hitter name: " + hit.collider.name + ", hit pos: " + hit.transform.position + ", player pos: " + player.position);
            // Debug.Log("collider name: " + hit.collider.name);

            Player collidingPlayer = hit.collider.GetComponent<Player>();  
            return !collidingPlayer.IsUnityNull();
        }

        // Debug.Log("hit not detected");

        return false;
    }

    void UpdateState(SquirrelState newState)
    {
        curState = newState;
    }

    void IdleState() {
        NavAgent.isStopped = true;
        animator.Play("Squirrel_Idle");

        float distToPlayer = (player.position - transform.position).magnitude;
        if (distToPlayer < AggressionRange && HasLineOfSight)
        {
            NavAgent.isStopped = false;
            UpdateState(SquirrelState.Searching);
        }
    }

    private void SearchingState()
    {
        animator.Play("Squirrel_Run");

        Vector3 direction = player.position - rb.position;
        float targetDist = direction.magnitude;

        if (targetDist > MaxAttackRange || !HasLineOfSight)
        {
            NavAgent.destination = player.position;
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
        float targetDist = Vector3.Distance(rb.position, player.position);

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
        Vector3 shootDir = (player.position - new Vector3(0f, 0.3f, 0f) - rb.position).normalized;
        Vector3 spawnPos = rb.position + new Vector3(0f, 0.5f, 0f);
        
        Instantiate(projectilePrefab, spawnPos, quaternion.identity)
            .GetComponent<AcornProjectile>().Init(projectileSpeed, shootDir, Damage);
    }

    // public void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Ray ray = new Ray(rb.position, (player.position - rb.position).normalized);
    //     Gizmos.DrawLine(rb.position, rb.position + (player.position - rb.position).normalized * 1000);
    // }
}
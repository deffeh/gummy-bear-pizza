using System;
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
    public float Speed = 5f;
    public float projectileSpeed = 8f;
    public float projectileSpawnDistFromPlayer = 0.5f;
    public float AttackCooldown = 2f;
    public float MaxAttackRange = 15f;
    public float AggressionRange = 20f;
    public LayerMask layerMask;
    private bool HasLineOfSight = false;
    private float CurCooldown = 0f;


    void Start()
    {
        base.Start();
        NavAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        player = Player.Instance.GetComponent<Rigidbody>();
        NavAgent.speed = Speed;
        NavAgent.stoppingDistance = MaxAttackRange;
        
        UpdateState(SquirrelState.Idle);
        // SetToSearching();
    }

    void FixedUpdate()
    {
        CurCooldown = Math.Max(0f, CurCooldown - Time.deltaTime);
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
        float distToPlayer = (player.position - transform.position).magnitude;
        
        if (distToPlayer < AggressionRange && HasLineOfSight)
        {
            NavAgent.isStopped = false;
            UpdateState(SquirrelState.Searching);
        }
    }

    public void SetToSearching()
    {
        UpdateState(SquirrelState.Searching);
    }

    private void SearchingState()
    {
        Vector3 direction = player.position - rb.position;
        float targetDist = direction.magnitude;

        if (targetDist > MaxAttackRange || !HasLineOfSight)
        {
            NavAgent.destination = player.position;
            NavAgent.stoppingDistance = 0;
        }
        else
        {
            UpdateState(SquirrelState.Attacking);
        }
    }

    private void AttackingState()
    {
        float targetDist = Vector3.Distance(rb.position, player.position);
        if (targetDist <= MaxAttackRange && HasLineOfSight)
        {
            NavAgent.stoppingDistance = MaxAttackRange;
            if (CurCooldown <= 0 && projectilePrefab)
            {
                ThrowAcorn();
                CurCooldown = AttackCooldown;
            }
        }
        else
        {
            UpdateState(SquirrelState.Searching);
        }
    }

    private void ThrowAcorn()
    {
        Vector3 shootDir = (player.position - rb.position).normalized;
        Vector3 spawnPos = rb.position;
        
        Instantiate(projectilePrefab, spawnPos, quaternion.identity)
            .GetComponent<AcornProjectile>().Init(projectileSpeed, shootDir);
    }

    // public void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Ray ray = new Ray(rb.position, (player.position - rb.position).normalized);
    //     Gizmos.DrawLine(rb.position, rb.position + (player.position - rb.position).normalized * 1000);
    // }
}
using System;
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
    // public AcornProjectile projectile;
    public NavMeshAgent NavAgent;
    public Rigidbody player;
    public Rigidbody rb;
    public int Health = 50;
    public float Speed = 5f;
    public float AttackCooldown = 1f;
    public float MaxAttackRange = 10f;
    private float CurCooldown = 0f;
    private Vector3 cameraDir;


    void Start()
    {
        base.Start();
        NavAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        NavAgent.speed = Speed;
        NavAgent.stoppingDistance = MaxAttackRange;
        SetToSearching();
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation((player.position - rb.position).normalized);
        
        CurCooldown = Math.Max(0f, CurCooldown - Time.deltaTime);
        Debug.Log("Current State: " + curState.ToString());

        switch (curState) 
        {
            case SquirrelState.Idle:
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

    void UpdateState(SquirrelState newState)
    {
        curState = newState;
    }

    public void SetToSearching()
    {
        UpdateState(SquirrelState.Searching);
    }

    private void SearchingState()
    {
        Vector3 direction = player.position - rb.position;
        float targetDist = direction.magnitude;

        bool hasLOS = CheckForLineOfSight(rb.transform, player.transform);

        

        if (targetDist > MaxAttackRange)
        {
            NavAgent.destination = player.position;
        }
        else
        {
            UpdateState(SquirrelState.Attacking);
        }
    }

    private bool CheckForLineOfSight(Transform t1, Transform t2)
    {
        t1.position += new Vector3(0, 1, 0);
        Vector3 direction = t1.position - t2.position;
        if (Physics.Raycast(t1.position, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            
            Debug.Log("hit pos: " + hit.transform.position + ", hit length: " + hit.distance + ", target pos: " + t2.position);
            Debug.Log("hit collider name: " + hit.collider.name);  
        }
        else
        {
            Debug.Log("no hit collison");
        }

        return true;
    }

    private void AttackingState()
    {
        float targetDist = Vector3.Distance(rb.position, player.position);
        if (targetDist <= MaxAttackRange)
        {
            if (CurCooldown <= 0)
            {
                // ThrowProjectile
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
        
    }
}
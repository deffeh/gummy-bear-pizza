using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public enum HumanState
{
    Idle,
    ReloadingNav,
    Reloading
}

public class Human : MonoBehaviour
{
    public static Human Instance;
    public int CurrentHealth;
    public int MaxHealth;

    private Player player;
    public float maxDistFromPlayer = 35f;
    public HumanState CurrentState;

    public Rigidbody HumanRigidBody;
    public NavMeshAgent HumanNavMeshAgent;
    public event Action<int> OnTakeDamage;
    public event Action<int> OnHealed;
    public Animator anim;
    public LineRenderer lr;
    public float lineSize = 1f;
    private bool catchingUpToPlayer;


    // real shit?
    void Awake() 
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else 
        {
            Instance = this;
        }

        catchingUpToPlayer = false;
        CurrentHealth = MaxHealth;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentState = HumanState.Idle;
        player = Player.Instance;
        lr.startWidth = lineSize;
        lr.endWidth = lineSize;
        lr.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (HumanNavMeshAgent.remainingDistance <= HumanNavMeshAgent.stoppingDistance) {
            anim.Play("HumanIdle");
            catchingUpToPlayer = false;
            lr.positionCount = 0;
        } else {
            Vector3[] pathCorners = HumanNavMeshAgent.path.corners;
            lr.positionCount = pathCorners.Length;
            lr.SetPositions(pathCorners);
        }

        float distFromPlayer = (player.transform.position - transform.position).magnitude;
        if (distFromPlayer > maxDistFromPlayer && !catchingUpToPlayer)
        {
            catchingUpToPlayer = true;
            MoveTo(player.transform.position);
        }
        
    }

    void FixedUpdate()
    {
        switch(CurrentState)
        {
            case HumanState.ReloadingNav:
            MoveTo(player.transform.position);
            break;
        }
    }

    public void MoveTo(Vector3 target)
    {
        anim.Play("HumanWalk");
        HumanRigidBody.rotation.SetLookRotation(target);
        HumanNavMeshAgent.SetDestination(target);
    }

    public void BeginReload()
    {
        CurrentState = HumanState.ReloadingNav;
    }

    public void Reload()
    {
        CurrentState = HumanState.Reloading;
        player.Reload();
    }

    public void OnHeal(int heal) {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + heal);
        OnHealed?.Invoke(CurrentHealth);
    }

    public void OnHit(int damage) {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        if (CurrentHealth == 0) {
            PauseMenu.Instance.Die();
            return;
        }
        OnTakeDamage?.Invoke(CurrentHealth);
    }
}

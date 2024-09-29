using System;
using System.Collections;
using System.Collections.Generic;
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

    private Player Player;
    public HumanState CurrentState;

    public Rigidbody HumanRigidBody;
    public NavMeshAgent HumanNavMeshAgent;
    public event Action<int> OnTakeDamage;
    public event Action<int> OnHealed;
    public Animator anim;
    public LineRenderer lr;
    public float lineSize = 1f;


    // real shit?
    void Awake() 
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else{
            Instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentState = HumanState.Idle;
        Player = Player.Instance;
        lr.startWidth = lineSize;
        lr.endWidth = lineSize;
        lr.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (HumanNavMeshAgent.remainingDistance <= HumanNavMeshAgent.stoppingDistance) {
            anim.Play("HumanIdle");
            lr.positionCount = 0;
        } else {
            Vector3[] pathCorners = HumanNavMeshAgent.path.corners;
            lr.positionCount = pathCorners.Length;
            lr.SetPositions(pathCorners);
        }
    }

    void FixedUpdate()
    {
        switch(CurrentState)
        {
            case HumanState.ReloadingNav:
            MoveTo(Player.transform.position);
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
        Player.Reload();
    }

    public void OnHeal(int heal) {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + heal);
        OnHealed?.Invoke(CurrentHealth);
    }

    public void OnHit(int damage) {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        if (CurrentHealth == 0) {
            //unborn yourself
        } else {
        }
        OnTakeDamage?.Invoke(CurrentHealth);
    }
}

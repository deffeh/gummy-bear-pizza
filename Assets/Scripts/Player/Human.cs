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
    }

    // Update is called once per frame
    void Update()
    {
        
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

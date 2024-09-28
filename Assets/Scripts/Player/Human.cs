using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    public static Human Instance;

    public Rigidbody HumanRigidBody;
    public NavMeshAgent HumanNavMeshAgent;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 target)
    {
        HumanRigidBody.rotation.SetLookRotation(target);
        HumanNavMeshAgent.SetDestination(target);
    }
}

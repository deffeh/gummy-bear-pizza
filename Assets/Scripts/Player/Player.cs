using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance;
    public Rigidbody PlayerRigidBody;

    public float MovementSpeed;
    public float AccelerationSpeed;
    public float JumpSpeed;
    public bool CanJump;
    private Vector3 StrafeVelocity;

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
        float fallSpeed = PlayerRigidBody.velocity.y;
        Vector3 targetVelocity = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
        {
            targetVelocity += Vector3.forward;
        }
        if(Input.GetKey(KeyCode.S))
        {
            targetVelocity += Vector3.back;
        }
        if(Input.GetKey(KeyCode.A))
        {
            targetVelocity += Vector3.left;
        }
        if(Input.GetKey(KeyCode.D))
        {
            targetVelocity += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Space) && CanJump)
        {
            fallSpeed = JumpSpeed;
            CanJump = false;
        }

        targetVelocity.Normalize();
        StrafeVelocity = Vector3.Lerp(StrafeVelocity, targetVelocity * MovementSpeed, AccelerationSpeed * Time.deltaTime);
        PlayerRigidBody.velocity = new Vector3(StrafeVelocity.x, fallSpeed, StrafeVelocity.z);
    }
}

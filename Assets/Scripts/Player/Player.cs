using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance;
    public Rigidbody PlayerRigidBody;

    // Movement
    public float MovementSpeed;
    public float AccelerationSpeed;
    public float JumpSpeed;
    public bool CanJump;
    private Vector3 StrafeVelocity;

    // Bark Cooldown
    public float BarkCooldown;
    private float BarkCooldownRemaining;
    private bool CanBark;

    // Bite Cooldown
    public float BiteCooldown;
    private float BiteCooldownRemaining;
    private bool CanBite;

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
        UpdateMovement();
        UpdateCooldown();

        if (Input.GetKeyDown(KeyCode.Mouse0) && CanBark)
            Bark();
        if (Input.GetKeyDown(KeyCode.Mouse1))
            Bite();

    }

    // Called every frame to update player movement
    void UpdateMovement()
    {
        float fallSpeed = PlayerRigidBody.velocity.y;
        Vector3 targetVelocity = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
            targetVelocity += Vector3.forward;
        if(Input.GetKey(KeyCode.S))
            targetVelocity += Vector3.back;
        if(Input.GetKey(KeyCode.A))
            targetVelocity += Vector3.left;
        if(Input.GetKey(KeyCode.D))
            targetVelocity += Vector3.right;

        if (Input.GetKeyDown(KeyCode.Space) && CanJump)
        {
            fallSpeed = JumpSpeed;
            CanJump = false;
        }

        targetVelocity.Normalize();
        StrafeVelocity = Vector3.Lerp(StrafeVelocity, targetVelocity * MovementSpeed, AccelerationSpeed * Time.deltaTime);
        PlayerRigidBody.velocity = new Vector3(StrafeVelocity.x, fallSpeed, StrafeVelocity.z); 
    }

    // Called everyframe to update action cooldown
    void UpdateCooldown()
    {
        if (!CanBark)
        {
            BarkCooldownRemaining -= Time.deltaTime;
            if (BarkCooldownRemaining <= 0)
            {
                CanBark = true;
                BarkCooldownRemaining = 0;
            }
        }

        if (!CanBite)
        {
            BiteCooldownRemaining -= Time.deltaTime;
            if (BiteCooldownRemaining <= 0)
            {
                CanBite = true;
                BiteCooldownRemaining = 0;
            }
        }
    }

    // Called to bark
    void Bark()
    {
        // Do Bark
        CanBark = true;
        BarkCooldownRemaining = BarkCooldown;
    }

    // Called to bite
    void Bite()
    {
        // Do Bite
        CanBite = true;
        BiteCooldownRemaining = BiteCooldown;
    }
}

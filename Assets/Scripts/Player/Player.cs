using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance;
    public Rigidbody PlayerRigidBody;
    public Camera PlayerCamera;

    // Look
    public float MouseSensitivity;
    private float XRotation;

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
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLook();
        UpdateMovement();
        UpdateCooldown();

        if (Input.GetKeyDown(KeyCode.Mouse0) && CanBark)
            Bark();
        if (Input.GetKeyDown(KeyCode.Mouse1))
            Bite();

    }

    // Called every frame to update player look
    void UpdateLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90, 90);
        PlayerCamera.transform.localRotation = Quaternion.Euler(XRotation, 0, 0);
    }

    // Called every frame to update player movement
    void UpdateMovement()
    {
        float fallSpeed = PlayerRigidBody.velocity.y;
        Vector3 targetVelocity = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
            targetVelocity += transform.forward;
        if(Input.GetKey(KeyCode.S))
            targetVelocity -= transform.forward;
        if(Input.GetKey(KeyCode.A))
            targetVelocity -= transform.right;
        if(Input.GetKey(KeyCode.D))
            targetVelocity += transform.right;

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

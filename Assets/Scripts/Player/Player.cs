using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance;
    public Rigidbody PlayerRigidBody;
    public Camera PlayerCamera;
    public Transform MouthPosition;
    public int CurrentHealth;
    public int MaxHealth;
    public Transform Model;

    // Look
    public float MouseSensitivity;
    private float XRotation;

    // Movement
    public float MovementSpeed;
    public float AccelerationSpeed;
    public float JumpSpeed;
    public bool CanJump;
    public float DashSpeed;
    public float DashCoolDown;
    private float DashCooldownRemaining;
    private bool CanDash;
    private Vector3 StrafeVelocity;

    // Bark
    public int BarkDamage;
    public float BarkRange;
    public float BarkRadius;
    public float BarkCooldown;
    private float BarkCooldownRemaining;
    private bool CanBark;

    // Bite
    public int BiteDamage;
    public float BiteRange;
    public float BiteRadius;
    public float BiteCooldown;
    private float BiteCooldownRemaining;
    private bool CanBite;
    
    // Human Interaction
    public bool CanInteract;

    // Human Command
    public float CommandRange;
    public LayerMask CommandLayerMask;
    private Human Human;

    private Vector3 targetVelocity;
    private float fallSpeed;
    private bool isJumping = false;
    private bool isDashing = false;
    
    public event Action OnBark;
    public event Action OnBite;
    public event Action<int> OnHealed;
    public event Action<int> OnTakeDamage;

    //sfx
    public AudioSource walkSrc;
    public AudioSource BarkSrc;
    public AudioSource BiteSrc;
    public AudioSource JumpSrc;
    public AudioSource DashSrc;
    public AudioSource HurtSrc;
    public AudioSource HealSrc;
    public List<AudioClip> barks;

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
        CurrentHealth = MaxHealth;
        Human = Human.Instance;
        CanInteract = false;
        if (PauseMenu.Instance) {
            SetDogVision(PauseMenu.Instance.Settings.IsDogVisionOn());
            MouseSensitivity = PauseMenu.Instance.Settings.GetSensitivity() * 200f;
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLook();
        UpdateCooldown();
        MovementControls();
        if (Input.GetKeyDown(KeyCode.Mouse0) && CanBark)
            Bark();
        if (Input.GetKeyDown(KeyCode.Mouse1) && CanBite)
            Bite();
        if (Input.GetKeyDown(KeyCode.Mouse2))
            CommandHuman();
        if (Input.GetKeyDown(KeyCode.F) && CanInteract)
            Interact();
        Model.position = Vector3.Lerp(Model.position, transform.position, 30 * Time.deltaTime);
        Model.rotation = transform.rotation;
    }

    void MovementControls()
    {
        fallSpeed = PlayerRigidBody.velocity.y;
        targetVelocity = Vector3.zero;
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
            isJumping = true;
            CanJump = false;
            JumpSrc.Play();
        }
        targetVelocity.Normalize();
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanDash)
        {
            CanDash = false;
            isDashing = true;
            DashSrc.Play();
        }
        if (Vector2.Distance(targetVelocity, Vector2.zero) < 0.001) {
            if (walkSrc.isPlaying) {
                walkSrc.Stop();
            }
        } else if (!walkSrc.isPlaying) {
            walkSrc.Play();
        }
    }
    
    // Called every frame to update player look
    void UpdateLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity / 150f;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity / 150f;
        transform.Rotate(Vector3.up * mouseX);
        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90, 90);
        PlayerCamera.transform.localRotation = Quaternion.Euler(XRotation, 0, 0);
    }

    // Called every frame to update player movement
    void UpdateMovement()
    {
        fallSpeed = PlayerRigidBody.velocity.y;
        if (isJumping)
        {
            fallSpeed = JumpSpeed;
            isJumping = false;
        }

        if (isDashing)
        {
            DashCooldownRemaining = DashCoolDown;
            StrafeVelocity += targetVelocity * DashSpeed;
            isDashing = false;
        }
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
        if (!CanDash)
        {
            DashCooldownRemaining -= Time.deltaTime;
            if (DashCooldownRemaining <= 0)
            {
                CanDash = true;
                DashCooldownRemaining = 0;
            }
        }
    }

    // Called to bark
    void Bark()
    {
        RaycastHit[] hits = Physics.SphereCastAll(MouthPosition.position, BarkRadius, MouthPosition.forward, BarkRange);
        foreach (RaycastHit hit in hits)
        {
            EnemyBase enemy = hit.transform.GetComponent<EnemyBase>();
            if (enemy)
                enemy.TakeDamage(BarkDamage);
        }
        OnBark?.Invoke();
        int randomId = UnityEngine.Random.Range(0, barks.Count);
        BarkSrc.clip = barks[randomId];
        BarkSrc.Play();
        CanBark = true; 
        BarkCooldownRemaining = BarkCooldown;
    }

    // Called to bite
    void Bite()
    {
        Debug.Log("Bite");
        RaycastHit hit;
        if (Physics.SphereCast(MouthPosition.position, BiteRadius, MouthPosition.forward, out hit, BiteRange))
        {
            EnemyBase enemy = hit.transform.GetComponent<EnemyBase>();
            if (enemy)
                enemy.TakeDamage(BiteDamage);
        }
        OnBite?.Invoke();
        BiteSrc.Play();
        CanBite = true;
        BiteCooldownRemaining = BiteCooldown;
    }

    public void OnHit(int damage) {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        if (CurrentHealth == 0) {
            //unborn yourself
        } else {

        }
        HurtSrc.Play();
        OnTakeDamage?.Invoke(CurrentHealth);

    }
    
    public void OnHit(int damage, Vector3 HitPos)
    {
        OnHit(damage);

        StrafeVelocity += (transform.position - HitPos).normalized * 30;
    }

    void CommandHuman()
    {
        Debug.Log("Command");
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, CommandRange, ~CommandLayerMask))
        {
            Human.MoveTo(hit.point);
        }
    }

    void Interact()
    {
        Human.Interact();
    }

    public void AddHealth(int HealAmount)
    {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + HealAmount);
        HealSrc.Play();
        OnHealed?.Invoke(CurrentHealth);
    }

    public void SetDogVision(bool isOn) {
        if (PlayerCamera.GetComponent<DogVisionPostProcess>()) {
            PlayerCamera.GetComponent<DogVisionPostProcess>().enabled = isOn;
        }
    }
}

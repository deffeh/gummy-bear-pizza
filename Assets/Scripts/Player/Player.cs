using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Reloading
}

public class Player : MonoBehaviour
{

    public static Player Instance;
    public Rigidbody PlayerRigidBody;
    public Camera PlayerCamera;
    public Transform MouthPosition;
    public int CurrentHealth;
    public int MaxHealth;
    public Transform Model;
    public PlayerState CurrentState;
    public ParticleSystem SpeedLines;
    public ParticleSystem Hearts;
    
    // Look
    public float MouseSensitivity;
    private float XRotation;
    private float YRotation;

    // Movement
    public float MovementSpeed;
    public float AccelerationSpeed;
    public float MinAccelerationSpeed;
    public float AccelerateT = 1;
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
    public int MaxBarkAmmo;
    [HideInInspector] public int BarkAmmo;
    public float ReloadTimeoutTime;

    // Bite
    public int BiteDamage;
    public float BiteRange;
    public float BiteRadius;
    public float BiteCooldown;
    private float BiteCooldownRemaining;
    private bool CanBite;
    
    // Human Reload
    public bool CanReload;

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
    public event Action OnReload;
    public event Action<int> OnHealed;
    public event Action<int> OnTakeDamage;
    public event Action<int> OnAmmoChanged;

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
        CurrentState = PlayerState.Normal;
        CurrentHealth = MaxHealth;
        BarkAmmo = MaxBarkAmmo;
        Human = Human.Instance;
        if (PauseMenu.Instance) {
            SetDogVision(PauseMenu.Instance.Settings.IsDogVisionOn());
            MouseSensitivity = PauseMenu.Instance.Settings.GetSensitivity() * 200f;
        }

        XRotation = transform.rotation.y;
        YRotation = 0;
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
        AccelerateT += Time.deltaTime * .5f;
        AccelerateT = Mathf.Clamp01(AccelerateT);
        if (CurrentState != PlayerState.Reloading)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && CanBark)
                Bark();
            if (Input.GetKeyDown(KeyCode.Mouse1) && CanBite)
                Bite();
            if (Input.GetKeyDown(KeyCode.Mouse2))
                CommandHuman();
            if (Input.GetKeyDown(KeyCode.R) && CanReload)
                BeginReload();
        }
        Model.position = Vector3.Lerp(Model.position, transform.position, 15 * Time.deltaTime);
        Model.rotation = transform.rotation;
    }

    void MovementControls()
    {
        fallSpeed = PlayerRigidBody.velocity.y;
        targetVelocity = Vector3.zero;
        if (CurrentState == PlayerState.Reloading) return;
        
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
            DashCooldownRemaining = DashCoolDown;
            CanDash = false;
            isDashing = true;
            DashSrc.Play();

            if (SpeedLines)
            {
                SpeedLines.transform.rotation = Quaternion.LookRotation( Quaternion.AngleAxis(90, Vector3.up) * targetVelocity.normalized) ;
                SpeedLines.Play();
            }
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
        YRotation += mouseX;
        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(0, YRotation, 0);
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
            StrafeVelocity += targetVelocity * DashSpeed;
            isDashing = false;
        }

        StrafeVelocity = Vector3.Lerp(StrafeVelocity, targetVelocity * MovementSpeed,
            Mathf.Lerp(MinAccelerationSpeed, AccelerationSpeed, AccelerateT) * Time.deltaTime);
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
        if (BarkAmmo <= 0)
            return;
        --BarkAmmo;
        OnAmmoChanged?.Invoke(BarkAmmo);
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
        CanBark = false; 
        BarkCooldownRemaining = BarkCooldown;
    }

    // Called to bite
    void Bite()
    {
        RaycastHit hit;
        if (Physics.SphereCast(MouthPosition.position, BiteRadius, MouthPosition.forward, out hit, BiteRange))
        {
            EnemyBase enemy = hit.transform.GetComponent<EnemyBase>();
            if (enemy)
                enemy.TakeDamage(BiteDamage);
        }
        OnBite?.Invoke();
        BiteSrc.Play();
        CanBite = false;
        BiteCooldownRemaining = BiteCooldown;
    }

    public void OnHit(int damage) {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        HurtSrc.Play();
        OnTakeDamage?.Invoke(CurrentHealth);
        if (CurrentHealth == 0) {
            //unborn yourself
            PauseMenu.Instance.Die();
            return;
        }
    }
    
    public void OnHit(int damage, Vector3 HitPos)
    {
        OnHit(damage);
        AccelerateT = 0;

        StrafeVelocity += (transform.position - HitPos).normalized * 10;
    }

    void CommandHuman()
    {
        if (Human.CurrentState == HumanState.ReloadingNav || Human.CurrentState == HumanState.Reloading)
        return;
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, CommandRange, ~CommandLayerMask))
        {
            Human.MoveTo(hit.point);
        }
    }

    void BeginReload()
    {
        if (BarkAmmo >= MaxBarkAmmo)
            return;
        Human.BeginReload();
    }

    public void Reload()
    {
        Hearts.Play();
        OnReload?.Invoke();
        BarkAmmo = MaxBarkAmmo;
        OnAmmoChanged?.Invoke(BarkAmmo);
        Human.CurrentState = HumanState.Idle;
        CurrentState = PlayerState.Reloading;
        StartCoroutine(ReloadTimeout());
    }

    private IEnumerator ReloadTimeout()
    {
        yield return new WaitForSeconds(ReloadTimeoutTime);
        CurrentState = PlayerState.Normal;
    }

    public void AddHealth(int HealAmount)
    {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + HealAmount);
        HealSrc.Play();
        OnHealed?.Invoke(CurrentHealth);
    }

    public void SetDogVision(bool isOn) {
        if (PlayerCamera && PlayerCamera.GetComponent<DogVisionPostProcess>()) {
            PlayerCamera.GetComponent<DogVisionPostProcess>().enabled = isOn;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(MouthPosition.position, BarkRadius);
        //Gizmos.DrawSphere(MouthPosition.position + (MouthPosition.forward * BarkRange), BarkRadius);
        // Gizmos.DrawSphere(MouthPosition.position, BiteRadius);
        // Gizmos.DrawSphere(MouthPosition.position + (MouthPosition.forward * BiteRange), BiteRadius);
    }
}

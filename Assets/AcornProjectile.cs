using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AcornProjectile : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SphereCollider triggerVolume;
    public float lifeSpan = 5f;
    private Rigidbody rb;

    public void Init(float initSpeed, Vector3 initDirection)
    {
        speed = initSpeed;
        direction = initDirection.normalized;
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }
    void Start()
    {
        Invoke(nameof(SelfDestruct), lifeSpan);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelfDestruct()
    {
        if (this != null)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(SelfDestruct));
        Debug.Log("Destroying projectile");
    }

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            // player take damage
        }
        
        Human human = other.GetComponent<Human>();
        if (human)
        {
            // human takes damange
            
        }

        Squirrel self = other.GetComponent<Squirrel>();
        if (!self)
        {
            Destroy(gameObject);
        }
    }
}

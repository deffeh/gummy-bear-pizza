using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AcornProjectile : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SphereCollider triggerVolume;
    public GameObject AcornEffectPrefab;
    public int Damage;
    public float lifeSpan = 5f;
    private Rigidbody rb;

    public void Init(float initSpeed, Vector3 initDirection, int Damage)
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
        Instantiate(AcornEffectPrefab, transform.position, quaternion.identity, null);
        Debug.Log("Destroying projectile");
    }

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            player.OnHit(Damage);
        }

        Human human = other.GetComponent<Human>();
        if (human)
        {
            human.OnHit(Damage);
        }

        Squirrel self = other.GetComponent<Squirrel>();
        if (!self)
        {
            Destroy(gameObject);
        }
    }
}

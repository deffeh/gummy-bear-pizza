using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    public float KnockbackForce = 100;
    void Start()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, transform.lossyScale.x/2);
        HashSet<GameObject> hitObjects = new HashSet<GameObject>();
        foreach (Collider hit in hits)
        {
            Rigidbody otherRb = hit.gameObject.GetComponent<Rigidbody>();
            if (otherRb)
            {
                if (!hitObjects.Contains(hit.gameObject))
                {
                    hitObjects.Add(hit.gameObject);
                    otherRb.AddForce((otherRb.position - transform.position).normalized * KnockbackForce);
                }
            }
        }

        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

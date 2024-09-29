using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarrel : EnemyBase
{
    public int Damage = 20;
    // Start is called before the first frame update
    void Start()
    {
        MaxHp = 1;

        base.Start();
    }

    override public void Die()
    {

        base.Die();
        
        Collider[] hits = Physics.OverlapSphere(transform.position, 3);
        HashSet<GameObject> hitObjects = new HashSet<GameObject>();
        foreach (Collider hit in hits)
        {
            Rigidbody otherRb = hit.gameObject.GetComponent<Rigidbody>();
            if (otherRb)
            {
                if (!hitObjects.Contains(hit.gameObject))
                {
                    hitObjects.Add(hit.gameObject);
                    otherRb.AddForce((otherRb.position - transform.position).normalized * 1000);
                    if (hit.GetComponent<Player>())
                    {
                        hit.GetComponent<Player>().OnHit(Damage, transform.position);
                    }

                    if (hit.GetComponent<Human>())
                    {
                        hit.GetComponent<Human>().OnHit(Damage);
                    }
                }
            }
        }
    }
    
}

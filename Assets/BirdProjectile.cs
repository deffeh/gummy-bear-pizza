using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BirdProjectile : MonoBehaviour
{
    public GameObject PigeonExplosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<EnemyPigeon>())
        {
            Instantiate(PigeonExplosion, transform.position, quaternion.identity, null);
            Destroy(gameObject);
        }
    }
}

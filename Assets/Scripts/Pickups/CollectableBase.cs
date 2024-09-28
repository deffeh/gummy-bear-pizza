using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableBase : MonoBehaviour
{

    private SphereCollider triggerVolume;
    private AudioSource collectSound;

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        triggerVolume = GetComponent<SphereCollider>();
        triggerVolume.isTrigger = true;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pickup collected");
        Collect(other);
    }

    public abstract void Collect(Collider other);
}

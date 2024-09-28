using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInteractionTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Player.Instance.CanInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Player.Instance.CanInteract = false;
    }
}

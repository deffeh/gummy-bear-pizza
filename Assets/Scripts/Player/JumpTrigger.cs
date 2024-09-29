using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private bool isGrounded;
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
        
        
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            print(other.name);
            Player.Instance.CanJump = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Player.Instance.CanJump = false;
    }
}

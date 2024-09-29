using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
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
        if (!other.CompareTag("Player"))
        {
            print(other.name);
            Player.Instance.CanJump = true;
        }
        
        
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            Player.Instance.CanJump = false;
    }
}

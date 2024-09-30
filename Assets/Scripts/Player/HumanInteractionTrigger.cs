using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInteractionTrigger : MonoBehaviour
{
    private Human Human;

    // Start is called before the first frame update
    void Start()
    {
        Human = Human.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (Human.CurrentState == HumanState.ReloadingNav))
            Human.Reload();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && (Human.CurrentState == HumanState.ReloadingNav))
            Human.Reload();
    }
}

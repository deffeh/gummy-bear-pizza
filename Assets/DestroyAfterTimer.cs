using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTimer : MonoBehaviour
{
    private float DestroyTimer = 5;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestroyTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnim : MonoBehaviour
{

    float startY;
    public float amp = 1;
    float freq = 1;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {   
        float oscillate = (Mathf.Sin(Time.time * freq) * amp);
        Debug.Log(oscillate);
        transform.position = new Vector3(transform.position.x,startY + oscillate, transform.position.z);
        transform.Rotate(0,0.1f,0);
    }
}

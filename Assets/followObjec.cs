using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObject : MonoBehaviour
{
    public GameObject ObjectToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, ObjectToFollow.transform.position, 30 * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform ParentObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 UpVector = transform.root.up;
        if (ParentObject)
        {
            UpVector = ParentObject.up;
        }
        if (Camera.main) { 
            transform.rotation = Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized, UpVector);
        }
    }
}

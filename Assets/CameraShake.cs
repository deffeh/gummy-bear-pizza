using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;

    private Vector3 startPos;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shake(1);
        }   
    }

    public static void Shake(float power)
    {
        instance.InstanceShake(power);
    }

    private void InstanceShake(float power)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeAnim(power));
    }

    private IEnumerator ShakeAnim(float power)
    {
        float startDuration = power / 10;
        float duration = startDuration;
        float startOffset = power / 3;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            float offset = startOffset * duration / startDuration;
            transform.localPosition = startPos + new Vector3(Random.Range(-offset, offset),Random.Range(-offset, offset),Random.Range(-offset, offset) );
            yield return null;
        }

        transform.localPosition = startPos;
    }
}

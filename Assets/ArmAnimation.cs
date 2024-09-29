using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmAnimation : MonoBehaviour
{
    private static ArmAnimation Instance;

    public AnimationCurve xPos;
    public AnimationCurve yPos;

    public Vector3 StartPos;
    public Vector3 EndPos;

    private RectTransform rect;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance)
        {
            return;
            Destroy(this);
        }
        Instance = this;
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        rect.anchoredPosition = StartPos;
    }

    public static void PlayAnimation()
    {
        if(!Instance) return;

        Instance.Animation();
    }

    private void Update()
    {
    }

    private void Animation()
    {
        StopAllCoroutines();
        StartCoroutine(PlayAnim());
    }
    
    private IEnumerator PlayAnim()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(StartPos.x, EndPos.x, xPos.Evaluate(t));
            float y = Mathf.Lerp(StartPos.y, EndPos.y, yPos.Evaluate(t));
            
            rect.anchoredPosition = new Vector2(x,y);
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSnout : MonoBehaviour
{
    public Animator animator; 
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Player.Instance.OnBark += DoBarkAnim;
        Player.Instance.OnBite += DoBiteAnim;
    }

    void OnDestroy()
    {
        Player.Instance.OnBark -= DoBarkAnim;
        Player.Instance.OnBite -= DoBiteAnim;
    }

    public void DoBarkAnim() {
        animator.Play("DogSnoutBark");
    }

    public void DoBiteAnim() {
        animator.Play("DogSnoutBite");
    }
}

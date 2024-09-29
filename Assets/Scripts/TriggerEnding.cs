using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnding : MonoBehaviour
{
    public IntroSequence intro;

    private bool entered = false;
    void OnTriggerEnter(Collider other) {
        if (entered) {
            return;
        }
        entered = true;
        intro.EndSequence();
    }
}

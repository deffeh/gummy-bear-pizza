using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnding : MonoBehaviour
{
    private bool entered = false;
    void OnTriggerEnter(Collider other) {
        if (entered) {
            return;
        }
        if (other.GetComponent<Player>()) {
            entered = true;
            LoadingScreen.Instance.LoadNewScene("EndingScene");
        }
    }
}

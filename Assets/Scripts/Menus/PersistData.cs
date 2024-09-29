using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistData : MonoBehaviour
{
    public static PersistData Instance;
    [HideInInspector] public Difficulty CurrDifficulty;
    [HideInInspector] public string DogName = "Dog";

    void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
    
}

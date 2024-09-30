using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistData : MonoBehaviour
{
    public static PersistData Instance;
    [HideInInspector] public Difficulty CurrDifficulty;
    [HideInInspector] public string DogName = "Dog";
    public AudioClip easySFX;
    public AudioClip medSFX;
    public AudioClip hardSFX;
    private AudioSource audiosrc;


    void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }

        audiosrc = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound() {
        var diff = CurrDifficulty;
        if (diff == Difficulty.Easy) {
            audiosrc.clip = easySFX;
        } else if (diff == Difficulty.Standard) {
            audiosrc.clip = medSFX;
        } else {
            audiosrc.clip = hardSFX;
        }
        audiosrc.Play();
    }
    
}

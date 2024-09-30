using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip HoverClip;
    public AudioClip OnClickClip;
    private AudioSource Audio;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Audio.clip = HoverClip;
        Audio.volume = 0.5f;
        Audio.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioSource>();
        if (OnClickClip != null) {
            GetComponent<Button>().onClick.AddListener(() => {
                Audio.clip = OnClickClip;
                Audio.volume = 1f;
                Audio.Play();
            });
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneScript : MonoBehaviour
{
    public Image SlideImage;
    public Image AnimationLayer;
    public List<Sprite> slides;
    public TMP_Text ObjCompText;
    public GameObject FartText;
    public CanvasGroup fadeGroup;
    public AudioSource clack;
    public AudioSource fart;
    // Start is called before the first frame update
    void Start()
    {
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        for (int i = 1; i < slides.Count; i++) {
            int temp = i;
            seq.AppendCallback(() => {
                AnimationLayer.sprite = slides[temp - 1];
                AnimationLayer.GetComponent<CanvasGroup>().alpha = 1f;
                SlideImage.sprite = slides[temp];
            });
            seq.AppendInterval(2f);
            seq.Append(AnimationLayer.GetComponent<CanvasGroup>().DOFade(0, 1f));
        }
        seq.AppendInterval(4f);
        seq.OnComplete(() => {
            TypeIt();
        });
        seq.Play();
    }

    void TypeIt() {
        var seq = DOTween.Sequence();
        float initialDelay = 1f;
        ObjCompText.text = "";
        seq.Append(fadeGroup.DOFade(1, initialDelay));
        string obj = "OBJECTIVE COMPLETE";
        string curString = "";
        float delay = initialDelay;
        for (int i = 0; i < obj.Length; i++) {
            curString += obj[i];
            string temp = curString;
            seq.InsertCallback(delay, () => {
                ObjCompText.text = temp;
                clack.Play();
            });
            delay += 0.1f;
        }

        seq.InsertCallback(initialDelay + (0.1f * obj.Length) + 2f, () => {
            FartText.SetActive(true);
            fart.Play();
        });
        
        seq.Insert(initialDelay + (0.1f * obj.Length) + 5f, GetComponent<AudioSource>().DOFade(0, 3f));

        seq.InsertCallback(initialDelay + (0.1f * obj.Length) + 8f, () => {
            LoadingScreen.Instance.LoadNewScene("TitleScreen");
        });
        seq.Play();
    }

}

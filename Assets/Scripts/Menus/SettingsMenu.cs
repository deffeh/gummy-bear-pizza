using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{

    public Slider SensSlider;
    public TMP_InputField SensInputField;
    public int MaxSens = 10;
    public float FADE_ANIM_DURATION = 1f;
    public Button CloseButton;

    // Start is called before the first frame update
    void Awake()
    {
        SensSlider.onValueChanged.AddListener(UpdateSensValText);
        SensInputField.onValueChanged.AddListener(UpdateSensSlider);
        CloseButton.onClick.AddListener(() => {
            Hide();
        });
    }

    public void Show() {
        var canvasGrp = GetComponent<CanvasGroup>();
        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.AppendCallback(() => gameObject.SetActive(true));
        seq.Append(canvasGrp.DOFade(1, FADE_ANIM_DURATION));
        seq.Join(transform.DOScale(1, FADE_ANIM_DURATION));
        seq.Play();
    }

    public void Hide() {
        var canvasGrp = GetComponent<CanvasGroup>();
        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(canvasGrp.DOFade(0, FADE_ANIM_DURATION));
        seq.Join(transform.DOScale(0, FADE_ANIM_DURATION));
        seq.AppendCallback(() => gameObject.SetActive(false));
        seq.Play();
    }

    public void InstantHide() {
        var canvasGrp = GetComponent<CanvasGroup>();
        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(canvasGrp.DOFade(0, 0));
        seq.Join(transform.DOScale(0, 0));
        seq.AppendCallback(() => gameObject.SetActive(false));
        seq.Play();
    }

    private void UpdateSensSlider(string val) {
        
        if (decimal.TryParse(val, out decimal value)) {
            if (value > MaxSens) {
                value = MaxSens;
            } else if (value < 0) {
                value = 0;
            }
            SensSlider.value = (float)value; 
        }
    }

    private void UpdateSensValText(float val) {
         if (val > MaxSens) {
            val = MaxSens;
        } else if (val < 0) {
            val = 0;
        }
        SensInputField.text = val.ToString("F2");
    }



}

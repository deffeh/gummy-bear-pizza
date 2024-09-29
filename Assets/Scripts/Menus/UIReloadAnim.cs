using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReloadAnim : MonoBehaviour
{
    public TMP_Text reloadText;
    public Image barFill;
    public void Show()
    {
        ArmAnimation.PlayAnimation();
        barFill.fillAmount = 0;
        reloadText.text = "Reloading";
        gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        float fillVal = 0;
        seq.Append(DOTween.To(() => fillVal, x => fillVal = x, 1f, 1f));
        seq.AppendCallback(() => {
            reloadText.text = "Reloaded!";
        });
        seq.OnUpdate(() => {
            barFill.fillAmount = fillVal;
        });
        seq.AppendInterval(0.5f);
        seq.OnComplete(() => {
            gameObject.SetActive(false);
        });
        seq.Play();

    }
}

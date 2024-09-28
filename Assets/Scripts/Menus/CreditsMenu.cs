using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    public CanvasGroup creditsCon;
    public Transform startPos;
    public float ANIM_DURATION = 1f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Hide);
    }

    public void Show() {
        creditsCon.transform.position = startPos.transform.position;
        creditsCon.alpha = 0;
        gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(creditsCon.DOFade(1, ANIM_DURATION));
        seq.Join(creditsCon.transform.DOLocalMoveY(0, ANIM_DURATION));
        seq.Play();
    }

    public void Hide() {
        gameObject.SetActive(false);
        creditsCon.alpha = 0;
        creditsCon.gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Difficulty {
    Easy,
    Standard,
    Helldogger
}


public class DifficultyColumn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Difficulty difficulty;
    public Transform SelectorRing;
    private const float notHoverOpacity = 0.9f;
    private const float ANIM_DURATION = 0.5f;
    // private Sequence seq;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().DOFade(0, ANIM_DURATION);
        SelectorRing.position = transform.position;
        SelectorRing.SetParent(transform.parent);
        SelectorRing.SetSiblingIndex(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().DOFade(notHoverOpacity, ANIM_DURATION);
    }

    void Awake()
    {
        // seq = DOTween.Sequence();
        // seq.SetEase(Ease.InOutQuad);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        GetComponent<Button>().onClick.AddListener(() => {

        });
    }

    

}

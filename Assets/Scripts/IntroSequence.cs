using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    public Transform StartTrans;
    public Transform CutTrans1;
    public Transform CutTrans2;
    public Camera IntroCam;
    public TMP_Text ObjectiveText;
    public AudioSource typeWriter;
    public AudioSource BARK;
    public TMP_Text SubtitleText;
    public Canvas GameCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameCanvas.gameObject.SetActive(false);
        PauseMenu.Instance.AllowPause = false;
        Player.Instance.PlayerCamera.gameObject.SetActive(false);
        ObjectiveText.text = "";
        SubtitleText.gameObject.SetActive(false);
        IntroCam.transform.position = StartTrans.position;
        IntroCam.transform.rotation = StartTrans.rotation;
        var seq = DOTween.Sequence();
        float initialDelay = 1.5f;
        seq.AppendInterval(initialDelay);
        string obj = "OBJECTIVE";
        string curString = "";
        float delay = initialDelay;
        for (int i = 0; i < obj.Length; i++) {
            curString += obj[i];
            string temp = curString;
            seq.InsertCallback(delay, () => {
                ObjectiveText.text = temp;
                typeWriter.Play();
            });
            delay += 0.2f;
        }

        seq.InsertCallback(initialDelay + 2f, () => {
            SubtitleText.gameObject.SetActive(true);
            BARK.Play();
            IntroCam.transform.position = CutTrans1.position;;
            IntroCam.transform.rotation = CutTrans1.rotation;
        });
        seq.Append(IntroCam.transform.DOMove(CutTrans2.position, 5f));
        seq.OnComplete(() => {
            IntroCam.gameObject.SetActive(false);
            Player.Instance.PlayerCamera.gameObject.SetActive(true);
            PauseMenu.Instance.AllowPause = true;
            gameObject.SetActive(false);
            GameCanvas.gameObject.SetActive(true);
        });
        seq.Play();
    }

   
}

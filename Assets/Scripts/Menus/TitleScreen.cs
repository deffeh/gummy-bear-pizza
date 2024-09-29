using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject Title;
    public Transform TitleAnimStartPos;
    public Transform ButtonAnimStartPos;
    public Button PlayButton;
    public Button SettingsButton;
    public Button CreditsButton;
    public Button ExitButton;
    public CreditsMenu Credits;
    public GameObject Vacumn;

    // Start is called before the first frame update
    void Start()
    {
        if (PauseMenu.Instance) {
            PauseMenu.Instance.AllowPause = false;
            PauseMenu.Instance.amDying = true;
        }

        PlayButton.onClick.AddListener(PlayGame);
        SettingsButton.onClick.AddListener(() => {
            PauseMenu.Instance.OpenSettings();
        });
        CreditsButton.onClick.AddListener(() => {
            Credits.Show();
        });
        ExitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        StartAnim();
    }

    private void StartAnim() {
        var ogTitlePos = Title.transform.position;
        float ogButtonX = PlayButton.transform.position.x;

        Title.transform.position = TitleAnimStartPos.position;
        PlayButton.transform.position = new Vector2(ButtonAnimStartPos.position.x, PlayButton.transform.position.y);
        SettingsButton.transform.position = new Vector2(ButtonAnimStartPos.position.x, SettingsButton.transform.position.y);
        CreditsButton.transform.position = new Vector2(ButtonAnimStartPos.position.x, CreditsButton.transform.position.y);
        ExitButton.transform.position = new Vector2(ButtonAnimStartPos.position.x, ExitButton.transform.position.y);

        Title.GetComponent<CanvasGroup>().alpha = 0;
        PlayButton.GetComponent<CanvasGroup>().alpha = 0;
        SettingsButton.GetComponent<CanvasGroup>().alpha = 0;
        CreditsButton.GetComponent<CanvasGroup>().alpha = 0;
        ExitButton.GetComponent<CanvasGroup>().alpha = 0;

        var seq = DOTween.Sequence();
        seq.Append(Title.transform.DOMoveY(ogTitlePos.y, 3f));
        seq.Join(Title.GetComponent<CanvasGroup>().DOFade(1, 3f));
        seq.Insert(1f, PlayButton.GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Insert(1f, PlayButton.transform.DOMoveX(ogButtonX, 1f));
        seq.Insert(1.5f, SettingsButton.GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Insert(1.5f, SettingsButton.transform.DOMoveX(ogButtonX, 1f));
        seq.Insert(2f, CreditsButton.GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Insert(2f, CreditsButton.transform.DOMoveX(ogButtonX, 1f));
        seq.Insert(2.5f, ExitButton.GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Insert(2.5f, ExitButton.transform.DOMoveX(ogButtonX, 1f));
        seq.Play();
    }

    private void PlayGame() {
        Vacumn.SetActive(true);
        LoadingScreen.Instance.LoadNewScene("DifficultySelection");
    }




}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public GameObject PauseContainer;
    public Button SettingsButton;
    public Button GoToTitleScreenButton;
    public SettingsMenu Settings;
    public bool isPaused = false;
    public bool AllowPause = true;
    public GameObject dieCon;
    public bool amDying = false;

    void Awake() 
    {
        if (Instance) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        SettingsButton.onClick.AddListener(OpenSettings);
        GoToTitleScreenButton.onClick.AddListener(GoToTitleScreen);
    }

    public void OpenSettings() {
        Settings.Show();
    }

    private void GoToTitleScreen() {
        Cursor.lockState = CursorLockMode.None;
        LoadingScreen.Instance.LoadNewScene("TitleScreen");
    }

    public void Die() {
        if (amDying) {return;}
        amDying = true;
        AllowPause = false;
        dieCon.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(dieCon.GetComponent<CanvasGroup>().DOFade(1, 3f));
        seq.Append(dieCon.GetComponent<CanvasGroup>().DOFade(0, 1f));
        seq.AppendInterval(0.5f);
        seq.OnComplete(() => {
            dieCon.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            LoadingScreen.Instance.LoadNewScene("TitleScreen");
        });
        seq.Play();
    }

    void Update()
    {
        // if (!AllowPause) {return;}
        // if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
            
        //     isPaused = !isPaused;
        //     if (isPaused) {
        //         Cursor.lockState = CursorLockMode.None;
        //     } else {
        //         Cursor.lockState = CursorLockMode.Locked;
        //     }
        //     PauseContainer.SetActive(isPaused);
        //     if (!isPaused) {
        //         Settings.InstantHide();
        //     }
        // }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public Button PlayButton;
    public Button SettingsButton;
    public Button CreditsButton;
    public Button ExitButton;
    public CreditsMenu Credits;

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.Instance.AllowPause = false;

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
    }

    private void PlayGame() {

    }




}
using System.Collections;
using System.Collections.Generic;
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

    private void OpenSettings() {
        Settings.Show();
    }

    private void GoToTitleScreen() {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
            isPaused = !isPaused;
            PauseContainer.SetActive(isPaused);
            if (!isPaused) {
                Settings.InstantHide();
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    private CanvasGroup _group;
    private const float DURATION = 1f;
    
    void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        _group = GetComponent<CanvasGroup>();
        SceneManager.sceneLoaded += HideScreen;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= HideScreen;
    }
    
    public void LoadNewScene(string scene) {
        var seq = DOTween.Sequence();
        seq.Append(_group.DOFade(1, DURATION));
        seq.AppendCallback(() =>SceneManager.LoadSceneAsync(scene));
    }

    private void HideScreen(Scene s, LoadSceneMode m) {
        _group.DOFade(0, DURATION);
    }
}

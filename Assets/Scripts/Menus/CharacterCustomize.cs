using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomize : MonoBehaviour
{
    public TMP_InputField dogName;
    public Material dogSnout;
    public Slider hueSlider;
    public Slider contrastSlider;
    public Slider brightnessSlider;
    public GameObject EasyDiff;
    public GameObject StandardDiff;
    public GameObject HardDiff;
    public Button PlayButton;

    // Start is called before the first frame update
    void Start()
    {
        float hue = dogSnout.GetFloat("_Hue");
        float contrast = dogSnout.GetFloat("_Contrast");
        float brightness = dogSnout.GetFloat("_Brightness");
        hueSlider.value = hue;
        contrastSlider.value = contrast;
        brightnessSlider.value = brightness;

        hueSlider.onValueChanged.AddListener((float val) => {
            Debug.Log("owo");
            dogSnout.SetFloat("_Hue", val);
        });

        contrastSlider.onValueChanged.AddListener((float val) => {
            dogSnout.SetFloat("_Contrast", val);
        });

        brightnessSlider.onValueChanged.AddListener((float val) => {
            dogSnout.SetFloat("_Brightness", val);
        });

        if (PersistData.Instance) {
            Difficulty curDiff = PersistData.Instance.CurrDifficulty;
            switch (curDiff) {
                case Difficulty.Easy:
                    EasyDiff.SetActive(true);
                    break;
                case Difficulty.Standard:
                    StandardDiff.SetActive(true);
                    break;
                case Difficulty.Helldogger:
                    HardDiff.SetActive(true);
                    break;
            }
            
            if (PersistData.Instance.DogName != "Dog") {
                dogName.text = PersistData.Instance.DogName;
            }
        }

        PlayButton.onClick.AddListener(StartGame);
    }

    public void StartGame() {
        if (PersistData.Instance) {
            if (dogName.text.Length <= 0) {
                PersistData.Instance.DogName = "Dog";
            } else {
                PersistData.Instance.DogName = dogName.text;
            }
        }

        //load game
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public TMP_Text DogHealthText;
    public TMP_Text HumanHealthText;
    public TMP_Text NumTreatsText;
    public TMP_Text NumSandwichesText;
    public Image DogPic;
    public Image HumanPic; 
    public TMP_Text Dog_NameText;
    // Start is called before the first frame update
    void Start()
    {
        Player play = Player.Instance;
        Human hum = Human.Instance;
        if (play) {
            play.OnHealed += DogHealed;
            play.OnTakeDamage += DogDamaged;
            DogHealthText.text = play.CurrentHealth.ToString();
        }
        if (hum) {
            hum.OnHealed += HumanHealed;
            hum.OnTakeDamage += HumanDamaged;
            HumanHealthText.text = hum.CurrentHealth.ToString();
        }
        if (PersistData.Instance) {
            Dog_NameText.text = PersistData.Instance.DogName + " Energy";
        }

    }

    void OnDestroy() {
        Player play = Player.Instance;
        Human hum = Human.Instance;
        if (play) {
            play.OnHealed -= DogHealed;
            play.OnTakeDamage -= DogDamaged;
        }
        if (hum) {
            hum.OnHealed -= HumanHealed;
            hum.OnTakeDamage -= HumanDamaged;
        }
    }

    private void DogHealed(int hp) {
        DogHealthText.text = hp.ToString();
    }

    private void DogDamaged(int hp) {
        DogHealthText.text = hp.ToString();
    }

    private void HumanHealed(int hp) {
        HumanHealthText.text = hp.ToString();
    }

    private void HumanDamaged(int hp) {
        HumanHealthText.text = hp.ToString();
    }


}

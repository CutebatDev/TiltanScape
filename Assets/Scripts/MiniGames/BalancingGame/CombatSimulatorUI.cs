using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatSimulatorUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider hpSlider;
    public Slider strengthSlider;
    public Slider speedSlider;
    public Slider intelligenceSlider;

    [Header("UI")]
    public TMP_Text feedbackText;
    public Button simulateButton;

    void Start()
    {
        simulateButton.onClick.AddListener(SimulateCombat);
    }

    void SimulateCombat()
    {
        float hp = hpSlider.value;
        float strength = strengthSlider.value;
        float speed = speedSlider.value;
        float intelligence = intelligenceSlider.value;

        float playerPower = hp * 0.4f + strength * 1.2f + speed * 0.8f + intelligence * 0.6f;

        float monsterPower = Random.Range(5f, 15f);

        if (playerPower > monsterPower)
        {
            feedbackText.text = "Victory!";
        }
        else
        {
            feedbackText.text = "Defeat...";
        }
    }
}
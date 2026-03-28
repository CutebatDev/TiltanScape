using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI row for one balancing parameter (HP, Damage, Speed, Intelligence).
/// Supports slider, +/- buttons, displayed value, and lock state.
/// </summary>
public class BalanceParameterRowUI : MonoBehaviour
{
    [Header("Parameter Info")]
    [SerializeField] private MonsterParameterType parameterType;

    [Header("UI References")]
    [SerializeField] private TMP_Text parameterNameText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Slider valueSlider;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private GameObject lockVisual;
    [SerializeField] private Image iconImage;

    private bool isLocked;
    private int currentValue;
    private System.Action<MonsterParameterType, int> onValueChanged;

    public MonsterParameterType ParameterType => parameterType;
    public int CurrentValue => currentValue;

    private void Awake()
    {
        if (valueSlider != null)
        {
            valueSlider.wholeNumbers = true;
            valueSlider.minValue = 1;
            valueSlider.maxValue = 10;
            valueSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreaseValue);

        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreaseValue);
    }

    /// <summary>
    /// Initializes this parameter row.
    /// </summary>
    public void Setup(
        string displayName,
        Sprite icon,
        int startValue,
        bool locked,
        System.Action<MonsterParameterType, int> onChanged)
    {
        onValueChanged = onChanged;
        isLocked = locked;
        currentValue = Mathf.Clamp(startValue, 1, 10);

        if (parameterNameText != null)
            parameterNameText.text = displayName;

        if (iconImage != null)
            iconImage.sprite = icon;

        if (valueSlider != null)
        {
            valueSlider.SetValueWithoutNotify(currentValue);
            valueSlider.interactable = !isLocked;
        }

        if (decreaseButton != null)
            decreaseButton.interactable = !isLocked;

        if (increaseButton != null)
            increaseButton.interactable = !isLocked;

        if (lockVisual != null)
            lockVisual.SetActive(isLocked);

        RefreshValueText();
    }

    private void OnSliderChanged(float newValue)
    {
        if (isLocked)
            return;

        currentValue = Mathf.RoundToInt(newValue);
        RefreshValueText();
        onValueChanged?.Invoke(parameterType, currentValue);
    }

    private void DecreaseValue()
    {
        if (isLocked)
            return;

        SetValue(currentValue - 1);
    }

    private void IncreaseValue()
    {
        if (isLocked)
            return;

        SetValue(currentValue + 1);
    }

    public void SetValue(int newValue)
    {
        currentValue = Mathf.Clamp(newValue, 1, 10);

        if (valueSlider != null)
            valueSlider.SetValueWithoutNotify(currentValue);

        RefreshValueText();
        onValueChanged?.Invoke(parameterType, currentValue);
    }

    private void RefreshValueText()
    {
        if (valueText != null)
            valueText.text = currentValue.ToString();
    }
}

public enum MonsterParameterType
{
    HP,
    Damage,
    Speed,
    Intelligence
}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI button used by the memory minigame.
/// It displays one sprite option and notifies the minigame UI when clicked.
/// </summary>
public class MemoryChoiceButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private int spriteIndex;
    private MemoryGameMinigame owner;

    /// <summary>
    /// Configures the button with its sprite and owner.
    /// </summary>
    public void Setup(int index, Sprite sprite, MemoryGameMinigame minigameOwner)
    {
        spriteIndex = index;
        owner = minigameOwner;

        if (iconImage != null)
        {
            iconImage.sprite = sprite;
            iconImage.preserveAspect = true;
        }

        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }
    }

    /// <summary>
    /// Sends the selected sprite index back to the minigame UI.
    /// </summary>
    private void OnClicked()
    {
        if (owner != null)
        {
            owner.RegisterPlayerChoice(spriteIndex);
        }
    }
}
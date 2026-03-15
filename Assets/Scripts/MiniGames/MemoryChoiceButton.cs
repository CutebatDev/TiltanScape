using UnityEngine;
using UnityEngine.UI;

public class MemoryChoiceButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private int spriteIndex;
    private MemoryGameMinigame owner;

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

    private void OnClicked()
    {
        if (owner != null)
        {
            owner.RegisterPlayerChoice(spriteIndex);
        }
    }
}
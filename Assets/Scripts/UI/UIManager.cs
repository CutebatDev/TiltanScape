using System;
using System.Collections.Generic;
using Events;
using Player.Movement;
using SceneChange;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIEntry> uiPanels;
    private UIEntry _previusPanel;
    private bool _isGamePaused;

    private void Start()
    {
        EventsManager.Instance.OnTogglePauseMenu += TogglePauseMenu;
        _isGamePaused = false;
    }

    public void TogglePauseMenu()
    {
        if (_isGamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void OpenUI(UIType uiToOpen)
    {
        _previusPanel = uiPanels.Find(panel => panel.uiObject.activeSelf);
        CloseAllUI();

        foreach (var uiPanel in uiPanels)
        {
            if (uiPanel.type == uiToOpen)
            {
                uiPanel.uiObject.SetActive(true);
            }
        }
    }

    private void CloseAllUI()
    {
        foreach (var menu in uiPanels)
        {
            if (menu.uiObject)
                menu.uiObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        OpenUI(UIType.PlayerUI);
        Time.timeScale = 1f;
        MovementController.playerInput.SwitchCurrentActionMap("Gameplay");
        _isGamePaused = false;
    }

    private void PauseGame()
    {
        OpenUI(UIType.PauseMenu);
        Time.timeScale = 0f;
        MovementController.playerInput.SwitchCurrentActionMap("UI");
        _isGamePaused = true;
    }

    public void MainMenu()
    {
        print("Main Menu");
    }

    public void SettingsMenu()
    {
        OpenUI(UIType.SettingsMenu);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BackButton()
    {
        CloseAllUI();
        OpenUI(_previusPanel.type);
    }

    public void TestNextScene()
    {
        BackButton();
        ResumeGame();
        EventsManager.Instance.OnNextScene?.Invoke();
    }
}


[System.Serializable]
public struct UIEntry
{
    public UIType type;
    public GameObject uiObject;
}
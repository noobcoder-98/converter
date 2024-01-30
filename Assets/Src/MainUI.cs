using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance;
    public Button runBtn;
    public TextMeshProUGUI btnName;
    public TextMeshProUGUI message;
    private MainHandler _mainHandler;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _mainHandler = gameObject.AddComponent<MainHandler>();
        message.text = "Do you want to load playtest data?";
        runBtn.interactable = true;
        runBtn.onClick.AddListener(OnRunBtnClick);
    }

    public void OnRunBtnClick()
    {
        runBtn.interactable = false;
        _mainHandler.BackupAndRenameFilesAsync();
    }
}
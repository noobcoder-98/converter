using System;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance;
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

        _mainHandler = new MainHandler();
    }

    public void OnRunBtnClick()
    {
        _mainHandler.BackupAndRenameFiles();
    }

    public void  OnUndoBtnClick()
    {
        
    }
}
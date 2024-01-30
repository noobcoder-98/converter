﻿using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class MainHandler : MonoBehaviour
{
    public static readonly string PrefixBackupName = "_old_";
    private string rootPath;

    public MainHandler()
    {
        var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        rootPath = Path.Combine(userPath, "AppData", "LocalLow", "TripleBricks", "LegendaryHoplite");
    }

    public async void BackupAndRenameFilesAsync()
    {
        if (MainUI.Instance == null) return;
        MainUI.Instance.btnName.text = "Processing...";
        if (!CheckPlaytestData())
        {
            MainUI.Instance.btnName.text = "Fail";
            MainUI.Instance.message.text = "You have no playtest data!";
            return;
        }
        string backupFolderName = PrefixBackupName + DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupFolderPath = Path.Combine(rootPath, backupFolderName);

        Directory.CreateDirectory(backupFolderPath);

        await Task.Run(() => CopyDirectory(rootPath, backupFolderPath, PrefixBackupName));
        string[] targetFileNames = { "gamedataBuild", "itemdataBuild", "all_SettingProloge", "___Profile_", "___RecordInfo", "___backup" };
        string[] newFileNames = { "gamedataFinal", "itemdataFinal", "all_SettingFinal", "_ProfileFinal", "_RecordFinal", "_BackupFinal" };
        await Task.Run(() => DeleteFilesAndFolders(rootPath, newFileNames));
        await Task.Run(() => RenameFilesAndFolders(rootPath, targetFileNames, newFileNames));
        MainUI.Instance.btnName.text = "Done";
    }

    private bool CheckPlaytestData()
    {
        string[] allPaths = Directory.GetFileSystemEntries(rootPath);
        foreach (var path in allPaths)
        {
            string filename = Path.GetFileName(path);
            if (string.Equals(filename, "___Profile_"))
            {
                return true;
            }
        }

        return false;
    }
    
    private void DeleteFilesAndFolders(string folderPath, string[] fileAndFolderNames)
    {
        string[] allPaths = Directory.GetFileSystemEntries(folderPath);

        foreach (string path in allPaths)
        {
            string name = Path.GetFileName(path);
            if (name.StartsWith(PrefixBackupName)) continue;
            if (Directory.Exists(path))
            {
                DeleteFilesAndFolders(path, fileAndFolderNames);
                if (!Directory.EnumerateFileSystemEntries(path).Any() || Array.Exists(fileAndFolderNames, element => element.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    Directory.Delete(path);
                }
            }
            else if (File.Exists(path))
            {
                if (Array.Exists(fileAndFolderNames,
                        element => element.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                }
            }
        }
    }

    private void CopyDirectory(string sourceDir, string destDir, string excludeFolderName)
    {
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }
        string[] files = Directory.GetFiles(sourceDir);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destPath = Path.Combine(destDir, fileName);
            File.Copy(file, destPath);
        }
        string[] subDirs = Directory.GetDirectories(sourceDir);
        foreach (string subdir in subDirs)
        {
            string dirName = Path.GetFileName(subdir);
            if (!dirName.StartsWith(excludeFolderName))
            {
                string destPath = Path.Combine(destDir, dirName);
                CopyDirectory(subdir, destPath, excludeFolderName);
            }
        }
    }

    private void RenameFilesAndFolders(string directoryPath, string[] targetNames, string[] newNames)
    {
        foreach (var filePath in Directory.GetFiles(directoryPath))
        {
            string fileName = Path.GetFileName(filePath);
            RenameIfMatch(filePath, fileName, targetNames, newNames);
        }
        foreach (var subDirectoryPath in Directory.GetDirectories(directoryPath))
        {
            string directoryName = Path.GetFileName(subDirectoryPath);
            if (directoryName.StartsWith(PrefixBackupName)) continue;
            RenameFilesAndFolders(subDirectoryPath, targetNames, newNames);
            RenameIfMatch(subDirectoryPath, directoryName, targetNames, newNames);
        }
    }

    private void RenameIfMatch(string path, string name, string[] targetNames, string[] newNames)
    {
        int index = Array.IndexOf(targetNames, name);
        if (index != -1)
        {
            string newName = newNames[index];
            string newPath = Path.Combine(Path.GetDirectoryName(path), newName);

            if (File.Exists(path))
            {
                File.Move(path, newPath);
            }
            else if (Directory.Exists(path))
            {
                Directory.Move(path, newPath);
            }
        }
    }
}

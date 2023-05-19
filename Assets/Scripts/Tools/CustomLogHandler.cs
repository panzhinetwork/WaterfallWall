using System;
using System.IO;
using UnityEngine;

public class CustomLogHandler
{
    private static CustomLogHandler _instance;

    private FileStream _fileStream;
    private StreamWriter _streamWriter;
    private ILogHandler _defaultLogHandler = Debug.unityLogger.logHandler;

    public static void Init()
    {
        if (_instance == null)
        {
            _instance = new CustomLogHandler();
        }
    }

    public static void Release()
    {
        _instance.OnDestroy();
    }

    public CustomLogHandler()
    {
        string filePath = Application.streamingAssetsPath + "/output_log.txt";
        if (File.Exists(filePath))
        {
            string destFilePath = filePath.Replace("output_log.txt", "output_log_prev.txt");
            if (File.Exists(destFilePath))
            {
                File.Delete(destFilePath);
            }
            File.Move(filePath, destFilePath);
        }

        _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _streamWriter = new StreamWriter(_fileStream);
        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (_streamWriter == null)
        {
            return;
        }

        _streamWriter.WriteLine(logString);
        _streamWriter.WriteLine(stackTrace);
        _streamWriter.Flush();
    }

    private void OnDestroy()
    {
        if (_streamWriter != null)
        {
            _streamWriter.Close();
            _streamWriter.Dispose();
            _streamWriter = null;
        }
    }
}

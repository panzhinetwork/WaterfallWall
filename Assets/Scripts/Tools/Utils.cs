using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Utils
{
    public static void CopyDirectory(string srcPath, string destPath)
    {
        try
        {
            if (destPath[destPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                destPath += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            string[] fileList = Directory.GetFileSystemEntries(srcPath);

            foreach (string file in fileList)
            {
                if (Directory.Exists(file))
                {
                    CopyDirectory(file, destPath + Path.GetFileName(file));
                }
                else
                {
                    File.Copy(file, destPath + Path.GetFileName(file), true);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }

    public static string CreateMD5Hash(string input)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    public static string CreateMD5Hash(string input, Encoding encoding)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = encoding.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    public static string GetMachineID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public static byte[] ConvertHexsToByteArray(string str)
    {
        int length = str.Length / 2;
        byte[] data = new byte[length];
        for (int i = 0; i < str.Length; i += 2)
        {
            string tmp = str[i] + "" + str[i + 1];
            data[i / 2] = Convert.ToByte(tmp, 16);
        }

        return data;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class ConfigFiles
{
    public static Configs configs;

	public static void ReadJsonConfigs()
	{
		string text = File.ReadAllText(Application.streamingAssetsPath + "/Configs.json");
		configs = JsonUtility.FromJson<Configs>(text);
	}

    public static void SaveJsonConfigs(Configs configs)
    {
        string text = JsonUtility.ToJson(configs, true);
        File.WriteAllText(Application.streamingAssetsPath + "/Configs.json", text);
    }

    public static void ReadXmlConfigs()
    {
        using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/Configs.xml", FileMode.Open))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configs));
            configs = (Configs)xmlSerializer.Deserialize(fileStream);
        }
    }

    public static void SaveXmlConfigs(Configs configs)
    {
        using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/Configs.xml", FileMode.Create))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configs));
            xmlSerializer.Serialize(fileStream, configs);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// XML数据管理类
/// </summary>
public class XMLManager
{
    // Custom MonoBehaviour Script  __SQW__
    public static XMLManager _instance = new XMLManager();
    public static XMLManager Instance => _instance;
    private XMLManager() { }

    /// <summary>
    /// 存储数据
    /// </summary>
    /// <param name="dataObj"></param>
    /// <param name="fileName"></param>
    /// <typeparam name="T"></typeparam>
    public void SaveData<T>(T dataObj, string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        using (StreamWriter writer = new StreamWriter(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            s.Serialize(writer, dataObj);
        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="fileName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadData<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" + fileName + ".xml";
            if (!File.Exists(path))
            {
                //默认文件夹里面也没有的话，就返回一个空的默认值的对象回去.
                return Activator.CreateInstance<T>();
            }
        }
        T obj = default(T);
        using (StreamReader reader = new StreamReader(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            obj = (T)s.Deserialize(reader);
        }
        return obj;
    }

}

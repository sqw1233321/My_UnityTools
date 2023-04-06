using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using System;

[Serializable]
public class Test2
{
    public int a;
    public SerializableDictionary<int, string> customDic;
}

public class IXML : MonoBehaviour
{

    public Test2 test2;

    public Test2 test3;
    // Custom MonoBehaviour Script  __SQW__
    void Start()
    {
        #region IXmlSeializebale
        //c# 的XmlSerializer 提供了可拓展内容
        //可以让一些不能被序列化和反序列化的特殊类能被处理
        //让特殊类继承IXmlSerializable接口 实现其中的方法即可
        #endregion

        #region 如何让Dictionary支持序列化和反序列化
        //还是用XmlSerializer接口
        //所以我们自定义一个类继承Dictionary，然后继承XmlSerianlizer接口
        //实现接口的自定义读写方法即可
        #endregion
        string path = Application.persistentDataPath + "/TestSerDic.xml";
        print(path);
        test2 = new Test2();
        test2.a = 1;
        test2.customDic = new SerializableDictionary<int, string>();
        test2.customDic.Add(1, "123");
        //因为泛型与参数相同类型，所以不用显示写泛型了
        test2.customDic.Add(2, "234");
        XMLManager.Instance.SaveData(test2, "testXmlDataMgr");
        // using (StreamWriter writer = new StreamWriter(path))
        // {
        //     XmlSerializer s = new XmlSerializer(typeof(Test2));
        //     s.Serialize(writer, test2);
        // }
        // test3 = new Test2();
        // test3.customDic = new SerializableDictionary<int, string>();
        test3 = XMLManager.Instance.LoadData<Test2>("testXmlDataMgr");
        // using (StreamReader reader = new StreamReader(path))
        // {
        //     XmlSerializer s = new XmlSerializer(typeof(Test2));
        //     test3 = s.Deserialize(reader) as Test2;
        // }
    }
}

/// <summary>
/// 可序列化字典
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        XmlSerializer keySer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSer = new XmlSerializer(typeof(TValue));
        //跳过根节点
        reader.Read();
        // while (reader != null)
        // {
        //     Debug.Log(reader.Name);
        //     reader.Read();
        // }
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            //解析是一行一行的解析，相当于前后两个标签加上innerXML。
            TKey key = (TKey)keySer.Deserialize(reader);
            TValue value = (TValue)valueSer.Deserialize(reader);
            this.Add(key, value);
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        //1.这种方式可以自定义字典的键值对的标签名,因为没有特性用来为字典的标签重命名

        foreach (KeyValuePair<TKey, TValue> kv in this)
        {
            //用属性写
            // writer.WriteStartElement("item1");
            // writer.WriteAttributeString("key", kv.Key.ToString());
            // writer.WriteAttributeString("value", kv.Value.ToString());
            // writer.WriteEndElement();

            //用元素写
            // writer.WriteStartElement("key");
            // writer.WriteValue(kv.Key);
            // writer.WriteEndElement();

            // writer.WriteStartElement("value");
            // writer.WriteValue(kv.Value);
            // writer.WriteEndElement();

        }

        //2.这种方式，字段没有变量名的话就直接用类型来充当标签
        XmlSerializer keySer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSer = new XmlSerializer(typeof(TValue));
        foreach (KeyValuePair<TKey, TValue> kv in this)
        {
            keySer.Serialize(writer, kv.Key);
            valueSer.Serialize(writer, kv.Value);
        }
    }
}
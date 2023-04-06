using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class PlayerPrefsDataMgr
{
    private static PlayerPrefsDataMgr _instance;

    public static PlayerPrefsDataMgr Instance
    {
        get
        {
            if (_instance == null) _instance = new PlayerPrefsDataMgr();
            return _instance;
        }
    }

    /// <summary>
    /// 存储数据
    /// </summary>
    /// <param name="obj">要存储数据的对象</param>
    /// <param name="keyName">关键字</param>
    public void SaveData(object obj, string keyName)
    {
        Type type = obj.GetType();
        FieldInfo[] fields = type.GetFields();
        //keyName_数据类型_字段类型_字段名
        string saveKeyName = "";
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo f = fields[i];
            saveKeyName = keyName + "_" + type.Name + "_" + f.FieldType.Name + "_" + f.Name;
            SaveValue(f.GetValue(obj), saveKeyName);
        }
    }


    //区分类型，一个字段可能是int，float，string，bool，List<T>,Dictionary<T>,自定义类型
    private void SaveValue(object value, string keyName)
    {
        Type type = value.GetType();
        //switch只能用作常量间的判断，变量间没法
        if (type == typeof(int))
        {
            PlayerPrefs.SetInt(keyName, (int)value);
        }
        else if (type == typeof(float))
        {
            PlayerPrefs.SetFloat(keyName, (float)value);
        }
        else if (type == typeof(string))
        {
            PlayerPrefs.SetString(keyName, value.ToString());
        }
        else if (type == typeof(bool))
        {
            //其它类型就自己定规则
            PlayerPrefs.SetInt(keyName, ((bool)value == true) ? 0 : 1);
        }
        else if (typeof(IList).IsAssignableFrom(value.GetType()))
        {
            IList list = value as IList;
            PlayerPrefs.SetInt(keyName, list.Count);
            int index = 0;
            foreach (object obj in list)
            {
                SaveValue(obj, keyName + index);
                ++index;
            }
        }
        else if (typeof(IDictionary).IsAssignableFrom(value.GetType()))
        {
            IDictionary dic = value as IDictionary;
            int index = 0;
            PlayerPrefs.SetInt(keyName, dic.Count);
            foreach (object obj in dic.Keys)
            {
                SaveValue(obj, keyName + "_key_" + index);
                SaveValue(dic[obj], keyName + "_value_" + index);
                ++index;
            }
        }
        //是自定义类型的话
        else
        {
            SaveData(value, keyName);
        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="type">要读取数据对象的类名</param>
    /// <param name="keyName">关键字</param>
    /// <returns></returns>
    public object LoadData(Type type, string keyName)
    {
        //确保type类型的对象有一个无参构造
        object obj = Activator.CreateInstance(type);
        FieldInfo[] fields = type.GetFields();
        string saveKeyName = "";
        foreach (var f in fields)
        {
            saveKeyName = keyName + "_" + type.Name + "_" + f.FieldType.Name + "_" + f.Name;
            f.SetValue(obj, LoadValue(f.FieldType, saveKeyName));
        }
        return obj;
    }

    private object LoadValue(Type fieldType, string keyName)
    {
        if (fieldType == typeof(int))
        {
            return PlayerPrefs.GetInt(keyName);
        }
        else if (fieldType == typeof(float))
        {
            return PlayerPrefs.GetFloat(keyName);
        }
        else if (fieldType == typeof(string))
        {
            return PlayerPrefs.GetString(keyName);
        }
        else if (fieldType == typeof(bool))
        {
            return PlayerPrefs.GetInt(keyName) == 0 ? true : false;
        }
        else if (typeof(IList).IsAssignableFrom(fieldType))
        {
            int length = PlayerPrefs.GetInt(keyName);
            IList list = Activator.CreateInstance(fieldType) as IList;
            for (int i = 0; i < length; i++)
            {
                list.Add(LoadValue(list.GetType().GetGenericArguments()[0], keyName + i));
            }
            return list;
        }
        else if (typeof(IDictionary).IsAssignableFrom(fieldType))
        {
            int length = PlayerPrefs.GetInt(keyName);
            IDictionary dic = Activator.CreateInstance(fieldType) as IDictionary;
            object key = null;
            object value = null;
            for (int i = 0; i < length; i++)
            {
                key = LoadValue(dic.GetType().GetGenericArguments()[0], keyName + "_key_" + i);
                value = LoadValue(dic.GetType().GetGenericArguments()[1], keyName + "_value_" + i);
                dic.Add(key, value);
            }
            return dic;
        }
        //是自定义类型的话
        else
        {
            return LoadData(fieldType, keyName);
        }
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using System;

public enum E_Axis_Name
{
    Horizontal,
    Vertical,
    Fire1,
    Fire2,
    Fire3,
    Jump,
    MouseX,
    MouseY,
    MouseScrollWheel
}
/// <summary>
/// 通过枚举名来获取虚拟轴的名字
/// </summary>
public static class GetAxisNameByEnum
{
    public static List<string> strList = new List<string>();
    //获取字串开始下标
    public static int subStartIndex = 8;

    /// <summary>
    /// 改变InputMannger中的值时同时要更新List
    /// </summary>
    public static void AxisUpdate()
    {
        string path = @"D:\unity\UnityTeach1\ProjectSettings\InputManager.asset";
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamReader sr = new StreamReader(fs, Encoding.Default);
        string content = string.Empty;
        strList.Clear();
        while ((content = sr.ReadLine()) != null)
        {
            content = content.Trim().ToString();
            if (content.StartsWith("m_Name"))
            {
                strList.Add(content.Substring(8));
            }
        }

    }
    /// <summary>
    /// 获取轴的名字
    /// </summary>
    /// <param name="name">枚举名</param>
    /// <returns></returns>
    public static string GetName(E_Axis_Name name)
    {
        return strList[Convert.ToInt32(name)];
    }
}

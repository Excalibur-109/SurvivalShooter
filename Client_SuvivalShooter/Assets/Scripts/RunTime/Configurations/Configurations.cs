using System.Collections.Generic;
using System;
using UnityEngine;
using Excalibur;

///summary 测试 /// summary
public static class TestCfg
{
    public static Dictionary<int, Test> Config = new Dictionary<int, Test> ();

    public class Test
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 字段1 ///summary
        public int field1;
        ///summary 字段2 ///summary
        public float field2;
        ///summary 字段3 ///summary
        public int[] field3;
        ///summary 用来看的 ///summary
        public float[] field_float;
        ///summary 字段4 ///summary
        public int[][] field4;
        ///summary 字段5 ///summary
        public float[][] field5;
        ///summary 字符串 ///summary
        public string icon;
    }

    public static string GetName () => typeof (Test).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<Test> (GetName ());

    public static Test TryGetValue (int id)
    {
        Test value = default (Test);
        try
        {
            value = Config[id];
        }
        catch (Exception e)
        {
            Debug.LogError ($"{GetName ()}配置表不存在id为 ({id})的数据");
        }
        return value;
    }
}

///summary 测试 /// summary
public static class Test2Cfg
{
    public static Dictionary<int, Test2> Config = new Dictionary<int, Test2> ();

    public class Test2
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 字段1 ///summary
        public int field1;
        ///summary 字段2 ///summary
        public float field2;
        ///summary 字段3 ///summary
        public int[] field3;
        ///summary 字段4 ///summary
        public int[][] field4;
        ///summary 字段5 ///summary
        public int[][] field5;
    }

    public static string GetName () => typeof (Test2).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<Test2> (GetName ());

    public static Test2 TryGetValue (int id)
    {
        Test2 value = default (Test2);
        try
        {
            value = Config[id];
        }
        catch (Exception e)
        {
            Debug.LogError ($"{GetName ()}配置表不存在id为 ({id})的数据");
        }
        return value;
    }
}


using System.Collections.Generic;
using System;
using UnityEngine;
using Excalibur;

///summary 角色表 /// summary
public static class CharacterCfg
{
    public static Dictionary<int, Character> Config = new Dictionary<int, Character> ();

    public class Character
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 类型 ///summary
        public int type;
        ///summary 预制体 ///summary
        public string prefab;
    }

    public static string GetName () => typeof (Character).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<Character> (GetName ());

    public static Character TryGetValue (int id)
    {
        Character value = default (Character);
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

///summary 武器 /// summary
public static class WeaponCfg
{
    public static Dictionary<int, Weapon> Config = new Dictionary<int, Weapon> ();

    public class Weapon
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 武器类型 ///summary
        public int weaponType;
        ///summary 对应Assets ///summary
        public string prefabName;
        ///summary 生成位置 ///summary
        public float[] spawnPos;
    }

    public static string GetName () => typeof (Weapon).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<Weapon> (GetName ());

    public static Weapon TryGetValue (int id)
    {
        Weapon value = default (Weapon);
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


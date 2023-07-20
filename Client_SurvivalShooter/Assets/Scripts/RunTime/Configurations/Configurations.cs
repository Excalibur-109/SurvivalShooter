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
        ///summary 标记 ///summary
        public int flag;
        ///summary 等级 ///summary
        public int level;
        ///summary 武器 ///summary
        public int weaponId;
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

///summary 道具 /// summary
public static class ItemCfg
{
    public static Dictionary<int, Item> Config = new Dictionary<int, Item> ();

    public class Item
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 类型 ///summary
        public int type;
        ///summary 预制体 ///summary
        public string prefab;
        ///summary 图片 ///summary
        public string icon;
    }

    public static string GetName () => typeof (Item).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<Item> (GetName ());

    public static Item TryGetValue (int id)
    {
        Item value = default (Item);
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

///summary 按键输入 /// summary
public static class PlayerInputCfg
{
    public static Dictionary<int, PlayerInput> Config = new Dictionary<int, PlayerInput> ();

    public class PlayerInput
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 组 ///summary
        public int group;
        ///summary 输入类型 ///summary
        public int inputType;
        ///summary 具体按键 ///summary
        public int key;
        ///summary 输入事件 ///summary
        public int inputAction;
    }

    public static string GetName () => typeof (PlayerInput).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<PlayerInput> (GetName ());

    public static PlayerInput TryGetValue (int id)
    {
        PlayerInput value = default (PlayerInput);
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

///summary 生成Entity /// summary
public static class SpawnCfg
{
    public static Dictionary<int, Spawn> Config = new Dictionary<int, Spawn> ();

    public class Spawn
    {
        ///summary 主键 ///summary
        public int id;
        ///summary 类型 ///summary
        public int spawnType;
        ///summary 生成中心点 ///summary
        public float position;
        ///summary 半径 ///summary
        public float radius;
        ///summary 生成对象 ///summary
        public int[] targets;
    }

    public static string GetName () => typeof (Spawn).Name;

    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<Spawn> (GetName ());

    public static Spawn TryGetValue (int id)
    {
        Spawn value = default (Spawn);
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


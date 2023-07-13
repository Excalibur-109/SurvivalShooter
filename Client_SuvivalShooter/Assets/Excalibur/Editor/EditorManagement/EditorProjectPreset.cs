using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace Excalibur
{
    public enum EditorPreset
    {
        AssetBundleBuild = 1,
        Configuration = 2,
        SpriteAtlasGenerator = 3,
    }

    public class EditorProjectPreset : Singleton<EditorProjectPreset>, IPersistant, IPersistantBehaviour
    {
        public const string EDITOR_GAME_ASSETS_FILE = "editorOptions.op";
        public const string EDITOR_GAME_ASSETS = "GameAssets";

        private readonly Dictionary<EditorPreset, IWindowDrawer> presets = new Dictionary<EditorPreset, IWindowDrawer>();

        private static string _gameAssetsPath;
        private static string _gameOptionFilePath;
        public static string GameAssetsPath => _GetGameAssetsPath();
        public static string GameOptionFilePath => _GetGameOptionFilePath();

        private static string _GetGameAssetsPath()
        {
            if (!string.IsNullOrEmpty(_gameAssetsPath)) { return _gameAssetsPath; }
            string directory = Path.GetFullPath(".");
            for (int i = directory.Length - 1; i >= 0; --i)
            {
                char value = directory[i];
                if (value == '\\')
                {
                    _gameAssetsPath = directory.Remove(i + 1) + EDITOR_GAME_ASSETS;
                    break;
                }
            }

            if (!Directory.Exists(_gameAssetsPath))
            {
                Directory.CreateDirectory(_gameAssetsPath);
            }
            return _gameAssetsPath;
        }

        private static string _GetGameOptionFilePath()
        {
            if (!string.IsNullOrEmpty(_gameOptionFilePath)) { return _gameOptionFilePath; }
            _gameOptionFilePath = Path.Combine(GameAssetsPath, EDITOR_GAME_ASSETS_FILE);
            if (!File.Exists(_gameOptionFilePath))
            {
                using (StreamWriter writer = new StreamWriter(File.Open(_gameOptionFilePath, FileMode.Create)))
                {
                    writer.WriteLine(0);
                }
            }
            return _gameOptionFilePath;
        }

        public void Save(IDataWriter writer)
        {
            writer.Write(presets.Count);
            foreach (EditorPreset option in presets.Keys)
            {
                writer.Write((int)option);
                presets[option].Save(writer);
            }
        }

        public void Load(IDataReader reader)
        {
            presets.Clear();
            int count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                EditorPreset option = (EditorPreset)reader.ReadInt();
                IWindowDrawer preset = _CreatePreset(option);
                preset.Load(reader);
                presets.Add(option, preset);
            }
        }

        public void Save()
        {
            PersistentStorage.Instance.StreamSave(GameOptionFilePath, this);
        }

        public void Load()
        {
            PersistentStorage.Instance.StreamLoad(GameOptionFilePath, this);
        }

        private static IWindowDrawer _CreatePreset (EditorPreset option)
        {
            IWindowDrawer preset = default;
            switch (option)
            {
                case EditorPreset.AssetBundleBuild:
                    preset = new AssetBundleBuildPreset();
                    break;
                case EditorPreset.Configuration:
                    preset = new ConfigurationPreset();
                    break;
                case EditorPreset.SpriteAtlasGenerator:
                    preset = new SpriteAtlasGenerator();
                    break;
            }
            return preset;
        }

        public IWindowDrawer GetPreset(EditorPreset option)
        {
            presets.TryGetValue(option, out IWindowDrawer preset);
            if (preset == default) 
            {
                preset = _CreatePreset(option);
                presets.Add(option, preset);
            }
            return preset;
        }
    }
}
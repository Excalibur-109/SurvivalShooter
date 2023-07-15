using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;
using System;

namespace Excalibur
{
    public partial class SpriteAtlasGenerator : IWindowDrawer
    {
        public const float RETRACT = 20f;

        private string _spritesPath;
        private string _outputPath;
        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;

        private GenSpriteAtlasItem _overrallSettings = new GenSpriteAtlasItem();
        private List<GenSpriteAtlasItem> _genSpriteAtlasItemsCache = new List<GenSpriteAtlasItem>();
        private List<GenSpriteAtlasItem> _genSpriteAtlasItems = new List<GenSpriteAtlasItem>();

        private TextureImporterPlatformSettings _platformSettings = new TextureImporterPlatformSettings();

        private string outputPath => _outputPath;
        private BuildTarget buildTarget => _buildTarget;
        private TextureImporterPlatformSettings platformSettings => _platformSettings;

        private static string[] Max_Tex_Size_Arr = new string[]
        {
            "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192"
        };

        private static string[] Padding = new string[] { "2", "4", "8" };

        public string title => "Generate SpriteAltas";

        Vector2 scrollPos;
        BuildTarget currentPlatform;
        bool showPlatformSettings = true;
        int selectedMaxSizeIndex;
        TextureImporterFormat importerFormat;
        TextureImporterCompression compression;

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(_spritesPath))
            {
                _UpdateItems();
            }
        }

        public void OnEditorGUI()
        {
            GUILayout.Space(5f);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AtlasTex 源文件夹", GUILayout.Width(120f));
            EditorGUILayout.TextField(_spritesPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref _spritesPath, "选择AtlasTexture源目录",
                    () => 
                    {
                        _genSpriteAtlasItems.Clear();
                        _UpdateItems();
                    });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AtlasTex 输出文件夹", GUILayout.Width(120f));
            EditorGUILayout.TextField(_outputPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref _outputPath, "选择AtlasTexture输出文件夹",() => 
                {
                    _genSpriteAtlasItems.ForEach(e => e.overrallOutput = _outputPath);
                    string path = IOAssistant.CombinePath(Application.dataPath, _outputPath);
                });
            }
            EditorGUILayout.EndHorizontal();
            currentPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Platform", _buildTarget);
            if (currentPlatform != _buildTarget)
            {
                _buildTarget = currentPlatform;
            }
            Color origin = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate All"))
            {
                for (int i = 0; i < _genSpriteAtlasItems.Count; ++i)
                {
                    _genSpriteAtlasItems[i].GenerateSpriteAtlas();
                }
            }
            GUI.backgroundColor = origin;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(RETRACT);
            _overrallSettings.LayoutIncludeInBuild();
            EditorGUILayout.EndHorizontal();
            _overrallSettings.LayoutSpriteAtlasPackingSettings();
            _overrallSettings.LayoutSpriteAtlasTextureSettings();
            LayoutTextureImporterPlatformSettings();
            if (GUILayout.Button("Apply Settings to All"))
            {
                _genSpriteAtlasItems.ForEach(e => e.ApplayOverrallSettings());
                Debug.Log("已将全集图集设置应用到全部图集预设");
            }
            GUILayout.Space(RETRACT);
            for (int i = 0; i < _genSpriteAtlasItemsCache.Count; ++i)
            {
                _genSpriteAtlasItemsCache[i].LayoutGUI();
            }
            GUILayout.Space(20);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void LayoutTextureImporterPlatformSettings()
        {
            showPlatformSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showPlatformSettings, "Platform Settings");
            if (showPlatformSettings)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(RETRACT);
                EditorGUILayout.BeginVertical();
                _platformSettings.overridden = EditorGUILayout.Toggle("Overriden", _platformSettings.overridden, GUILayout.Width(140f));
                int paddingIndex = EditorGUILayout.Popup("Padding", selectedMaxSizeIndex, Max_Tex_Size_Arr);
                if (paddingIndex != selectedMaxSizeIndex)
                {
                    _platformSettings.maxTextureSize = int.Parse(Max_Tex_Size_Arr[paddingIndex]);
                    selectedMaxSizeIndex = paddingIndex;
                }
                importerFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Format", _platformSettings.format);
                if (importerFormat != _platformSettings.format)
                {
                    _platformSettings.format = importerFormat;
                }
                compression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", _platformSettings.textureCompression);
                if (compression != _platformSettings.textureCompression)
                {
                    _platformSettings.textureCompression = compression;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public void OpenWindow()
        {
            EditorUIManager.Instance.OpenWindow<PresetsWindow>(this);
        }

        private void _UpdateItems()
        {
            string[] directories = IOAssistant.GetDirectories(IOAssistant.CombinePath(Application.dataPath, _spritesPath), string.Empty, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < directories.Length; ++i)
            {
                string dir = EditorUtil.GetPathNameInAssets(directories[i]);
                GenSpriteAtlasItem item = null;
                for (int j = 0; j < _genSpriteAtlasItems.Count; ++j)
                {
                    if (_genSpriteAtlasItems[j].directory == dir)
                    {
                        item = _genSpriteAtlasItems[j];
                        break;
                    }
                }
                if (item == null)
                {
                    item = new GenSpriteAtlasItem();
                    item.overrallOutput = _outputPath;
                    _genSpriteAtlasItems.Add(item);
                }
                _genSpriteAtlasItemsCache.Add(item);
                item.SetItemInfo(directories[i], this);
            }
        }

        public void Terminate()
        {
            _genSpriteAtlasItemsCache.Clear();
        }

        public void Load(IDataReader reader)
        {
            _spritesPath = reader.ReadString();
            _outputPath = reader.ReadString();
            _overrallSettings = new GenSpriteAtlasItem();
            _overrallSettings.LoadSpriteAtlasTextureSettings(reader);
            _overrallSettings.LoadSpriteAtlasPackingSettings(reader);
            _platformSettings.overridden = reader.ReadBool();
            _platformSettings.maxTextureSize = reader.ReadInt();
            int index = Max_Tex_Size_Arr.ToList().IndexOf(_platformSettings.maxTextureSize.ToString());
            selectedMaxSizeIndex = Mathf.Max(0, index);
            _platformSettings.format = (TextureImporterFormat)reader.ReadInt();
            importerFormat = _platformSettings.format;
            _platformSettings.textureCompression = (TextureImporterCompression)reader.ReadInt();
            int count = reader.ReadInt();
            _genSpriteAtlasItems = new List<GenSpriteAtlasItem>();
            for (int i = 0; i < count; ++i)
            {
                GenSpriteAtlasItem item = new GenSpriteAtlasItem();
                item.Load(reader);
                _genSpriteAtlasItems.Add(item);
            }
        }

        public void Save(IDataWriter writer)
        {
            writer.Write(_spritesPath);
            writer.Write(_outputPath);
            _overrallSettings.SaveSpriteAtlasTextureSettings(writer);
            _overrallSettings.SaveSpriteAtlasPackingSettings(writer);
            writer.Write(_platformSettings.overridden);
            writer.Write(_platformSettings.maxTextureSize);
            writer.Write((int)_platformSettings.format); 
            writer.Write((int)_platformSettings.textureCompression);
            int count = _genSpriteAtlasItems == null ? 0 : _genSpriteAtlasItems.Count;
            writer.Write(count);
            if (count > 0)
            {
                for (int i = 0; i < _genSpriteAtlasItems.Count; ++i)
                {
                    _genSpriteAtlasItems[i].Save(writer);
                }
            }
        }

        private void _GenerateSpriteAtlases ()
        {

        }

        public static void GenerateSpriteAtlas(string inputPath, string outputpath, bool holeDir, bool includeInBuild, BuildTarget buildTarget, SpriteAtlasTextureSettings texSettings, SpriteAtlasPackingSettings packSettings, TextureImporterPlatformSettings platformSettings = null,
            Action onComplete = null)
        {
            if (!Directory.Exists(outputpath)) { Directory.CreateDirectory(outputpath); }

            outputpath = IOAssistant.ConvertToUnityRelativePath(outputpath);
            SpriteAtlas atlas;
            string name = EditorUtil.TailorLast(inputPath);
            string assetPath = outputpath + "/" + name + IOAssistant.FileExtension_SpriteAtlas;
            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            if (atlas == null)
            {
                atlas = new SpriteAtlas();
                if (string.IsNullOrEmpty(name)) { name = "NewSpriteAtlas"; }
                AssetDatabase.CreateAsset(atlas, assetPath);
            }

            Object[] packables = atlas.GetPackables();
            if (packables != null && packables.Length > 0)
            {
                atlas.Remove(packables);
            }

            List<Object> sprites = new List<Object>();
            if (holeDir)
            {
                sprites.Add(AssetDatabase.LoadAssetAtPath<DefaultAsset>(IOAssistant.ConvertToUnityRelativePath(inputPath)));
            }
            else
            {
                List<string> files = IOAssistant.GetFiles(inputPath, string.Empty, SearchOption.AllDirectories, file => file.EndsWith(IOAssistant.FileExtension_Png) || file.EndsWith(IOAssistant.FileExtension_Jpg)).ToList();
                files.ForEach(e =>
                {
                    string path = IOAssistant.ConvertToUnityRelativePath(e);
                    sprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
                });
            }
            atlas.Add(sprites.ToArray());

            atlas.SetIncludeInBuild(includeInBuild);
            atlas.SetTextureSettings(texSettings);
            atlas.SetPackingSettings(packSettings);
            if (platformSettings != null)
            {
                TextureImporterPlatformSettings atlasSettings = atlas.GetPlatformSettings(buildTarget.ToString());
                atlasSettings.overridden = platformSettings.overridden;
                atlasSettings.maxTextureSize = platformSettings.maxTextureSize;
                atlasSettings.format = platformSettings.format;
                atlasSettings.textureCompression = platformSettings.textureCompression;
                atlas.SetPlatformSettings(atlasSettings);
            }

            onComplete?.Invoke();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public partial class SpriteAtlasGenerator
    {
        private class GenSpriteAtlasItem : IPersistant
        {
            public bool includeInBuild;
            public string overrallOutput;
            public string directory;

            private SpriteAtlasGenerator _generator;
            private Grouping _grouping;
            private string[] _subDirectories;
            private string[] _spriteAtlasFolder;
            private SpriteAtlasTextureSettings _texSettings;
            private SpriteAtlasPackingSettings _packSettings;
            private Object[][] _sprites;
            private bool 
                _showItem = false,
                _showTexSettings = true,
                _showPackingSettings = true;
            private bool[] _showItems;
            private int _paddingIndex;

            public GenSpriteAtlasItem() { }

            public void SetItemInfo(string directory, SpriteAtlasGenerator generator)
            {
                _generator = generator;
                this.directory = directory;
                _GrabDirectories();
                this.directory = EditorUtil.GetPathNameInAssets(directory);
                _grouping = _subDirectories == null || _subDirectories.Length == 0 ? Grouping.AllInOne : Grouping.OneByOne;
                int length = _grouping == Grouping.AllInOne ? 1 : _subDirectories.Length;
                switch (_grouping)
                {
                    case Grouping.AllInOne:
                        _spriteAtlasFolder = new string[1] { this.directory };
                        _showItems = new bool[1];
                        string path = IOAssistant.CombinePath(Application.dataPath, directory);
                        string[] files = IOAssistant.GetFiles(path, string.Empty, SearchOption.AllDirectories,
                            file => file.EndsWith(IOAssistant.FileExtension_Png) || file.EndsWith(IOAssistant.FileExtension_Jpg));
                        _sprites = new Sprite[1][];
                        _sprites[0] = new Sprite[files.Length];
                        for (int j = 0; j < files.Length; ++j)
                        {
                            _sprites[0][j] = AssetDatabase.LoadAssetAtPath<Sprite>(IOAssistant.ConvertToUnityRelativePath(files[j]));
                        }
                        break;
                    case Grouping.OneByOne:
                        _spriteAtlasFolder = new string[length];
                        _showItems = new bool[length];
                        _sprites = new Object[length][];
                        for (int i = 0; i < length; ++i)
                        {
                            _spriteAtlasFolder[i] = EditorUtil.TailorLast(_subDirectories[i]);
                            path = IOAssistant.CombinePath(Application.dataPath, _subDirectories[i]);
                            files = IOAssistant.GetFiles(path, string.Empty, SearchOption.AllDirectories, 
                                file => file.EndsWith(IOAssistant.FileExtension_Png) || file.EndsWith(IOAssistant.FileExtension_Jpg));
                            _sprites[i] = new Object[files.Length];
                            for (int j = 0; j < files.Length; ++j)
                            {
                                _sprites[i][j] = AssetDatabase.LoadAssetAtPath<Sprite>(IOAssistant.ConvertToUnityRelativePath(files[j]));
                            }
                        }
                        break;
                }
            }

            public void GenerateSpriteAtlas()
            {
                switch (_grouping)
                {
                    case Grouping.AllInOne:
                        _GenerateSpriteAtlas(directory, true);
                        break;
                    case Grouping.OneByOne:
                        for (int i = 0; i < _subDirectories.Length; ++i)
                        {
                            _GenerateSpriteAtlas(_subDirectories[i], false);
                        }
                        break;
                }
            }

            private void _GenerateSpriteAtlas(string directory, bool holeDir)
            {
                string inputPath = IOAssistant.CombinePath(Application.dataPath, directory);
                string outputPath;
                if (holeDir)
                {
                    outputPath = IOAssistant.CombinePath(Application.dataPath, _generator.outputPath);
                }
                else
                {
                    string[] names = directory.Split('/');
                    outputPath = IOAssistant.CombinePath(Application.dataPath, _generator.outputPath, names[1]);
                }
                SpriteAtlasGenerator.GenerateSpriteAtlas(inputPath, outputPath, holeDir, includeInBuild, _generator.buildTarget, _texSettings, _packSettings, _generator.platformSettings, null);
            }

            private void _GrabDirectories()
            {
                _subDirectories = IOAssistant.GetDirectories(directory, "*.*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < _subDirectories.Length; ++i)
                {
                    _subDirectories[i] = EditorUtil.GetPathNameInAssets(IOAssistant.ConvertToUnityRelativePath(_subDirectories[i]));
                }
            }

            public void SetSpriteAtlasTextureSettings(SpriteAtlasTextureSettings settings)
            {
                _texSettings = settings;
            }

            public void SetSpriteAtlasPackingSettings(SpriteAtlasPackingSettings settings)
            {
                _packSettings = settings;
            }

            public void LayoutGUI()
            {
                _LayoutSprites();
            }

            public void LayoutSpriteAtlasTextureSettings()
            {
                _showTexSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showTexSettings, "Texture Settings");
                if (_showTexSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    EditorGUILayout.BeginVertical();
                    _texSettings.readable = EditorGUILayout.Toggle("Read/Write", _texSettings.readable, GUILayout.Width(140f));
                    _texSettings.sRGB = EditorGUILayout.Toggle("sRGB", _texSettings.sRGB, GUILayout.Width(140f));
                    _texSettings.generateMipMaps = EditorGUILayout.Toggle("Generate MipMaps",
                        _texSettings.generateMipMaps, GUILayout.Width(140f));
                    _texSettings.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", _texSettings.filterMode);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            public void LayoutSpriteAtlasPackingSettings()
            {
                _showPackingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showPackingSettings, "Packing Settings");
                if (_showPackingSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    EditorGUILayout.BeginVertical();
                    _packSettings.enableRotation = EditorGUILayout.Toggle("Allow Rotation", _packSettings.enableRotation, GUILayout.Width(140f));
                    _packSettings.enableTightPacking = EditorGUILayout.Toggle("Allow TightPacking", _packSettings.enableTightPacking, GUILayout.Width(140f));
                    int paddingIndex = EditorGUILayout.Popup("Padding", _paddingIndex, Padding);
                    if (paddingIndex != _paddingIndex)
                    {
                        _packSettings.padding = int.Parse(Padding[paddingIndex]);
                        _paddingIndex = paddingIndex;
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            public void LayoutIncludeInBuild()
            {
                includeInBuild = EditorGUILayout.Toggle("Include In Build", includeInBuild);
            }

            private void _LayoutSprites()
            {
                EditorGUILayout.BeginVertical();
                _showItem = EditorGUILayout.BeginFoldoutHeaderGroup(_showItem, directory);
                if (_showItem)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginFoldoutHeaderGroup(true, "ignore");
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.LabelField("Settings");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    EditorGUILayout.BeginVertical();
                    LayoutIncludeInBuild();
                    LayoutSpriteAtlasTextureSettings();
                    LayoutSpriteAtlasPackingSettings();
                    if (GUILayout.Button("Apply Overrall Setting"))
                    {
                        ApplayOverrallSettings();
                        Debug.Log("已应用全局设置");
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(RETRACT * 0.5f);
                    for (int i = 0; i < _spriteAtlasFolder.Length; ++i)
                    {
                        EditorGUILayout.BeginVertical();
                        switch (_grouping)
                        {
                            case Grouping.AllInOne:
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(RETRACT);
                                EditorGUILayout.BeginVertical();
                                Color origin = GUI.backgroundColor;
                                GUI.backgroundColor = Color.green;
                                if (GUILayout.Button("Generate"))
                                {
                                    _GenerateSpriteAtlas(_spriteAtlasFolder[i], true);
                                }
                                GUI.backgroundColor = origin;
                                for (int j = 0; j < _sprites[0].Length; ++j)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField(_sprites[0][j].name, GUILayout.Width(RETRACT * 12));
                                    EditorGUILayout.ObjectField(_sprites[0][j], typeof(Object), false);
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.EndHorizontal();
                                break;
                            case Grouping.OneByOne:
                                EditorGUILayout.BeginVertical();
                                _showItems[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_showItems[i], _spriteAtlasFolder[i]);
                                if (_showItems[i])
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(RETRACT);
                                    EditorGUILayout.BeginVertical();
                                    origin = GUI.backgroundColor;
                                    GUI.backgroundColor = Color.green;
                                    if (GUILayout.Button("Generate"))
                                    {
                                        _GenerateSpriteAtlas(_subDirectories[i], false);
                                    }
                                    GUI.backgroundColor = origin;
                                    for (int j = 0; j < _sprites[i].Length; ++j)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(_sprites[i][j].name, GUILayout.Width(RETRACT * 12));
                                        EditorGUILayout.ObjectField(_sprites[i][j], typeof(Object), false);
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndFoldoutHeaderGroup();
                                EditorGUILayout.EndVertical();
                                break;
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(5);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }

            public void ApplayOverrallSettings()
            {
                includeInBuild = _generator._overrallSettings.includeInBuild;
                _texSettings = _generator._overrallSettings._texSettings;
                _packSettings = _generator._overrallSettings._packSettings;
                _paddingIndex = Padding.ToList().IndexOf(_packSettings.padding.ToString());
            }

            public void Save (IDataWriter writer)
            {
                writer.Write(directory);
                writer.Write(includeInBuild);
                SaveSpriteAtlasPackingSettings(writer);
                SaveSpriteAtlasTextureSettings(writer);
            }

            public void Load (IDataReader reader)
            {
                directory = reader.ReadString();
                includeInBuild = reader.ReadBool();
                LoadSpriteAtlasPackingSettings(reader);
                LoadSpriteAtlasTextureSettings(reader);
            }

            public void SaveSpriteAtlasTextureSettings (IDataWriter writer)
            {
                writer.Write(_texSettings.readable);
                writer.Write(_texSettings.sRGB);
                writer.Write(_texSettings.generateMipMaps);
                writer.Write((int)_texSettings.filterMode);
            }

            public void LoadSpriteAtlasTextureSettings (IDataReader reader)
            {
                _texSettings.readable = reader.ReadBool();
                _texSettings.sRGB = reader.ReadBool();
                _texSettings.generateMipMaps = reader.ReadBool();
                _texSettings.filterMode = (FilterMode)reader.ReadInt();
            }

            public void SaveSpriteAtlasPackingSettings (IDataWriter writer)
            {
                writer.Write(_packSettings.enableRotation);
                writer.Write(_packSettings.enableTightPacking);
                writer.Write(_packSettings.padding);
            }

            public void LoadSpriteAtlasPackingSettings (IDataReader reader)
            {
                _packSettings.enableRotation = reader.ReadBool();
                _packSettings.enableTightPacking = reader.ReadBool();
                _packSettings.padding = reader.ReadInt();
                int index = Padding.ToList().IndexOf(_packSettings.padding.ToString());
                _paddingIndex = Mathf.Max(0, index);
            }
        }
    }
}

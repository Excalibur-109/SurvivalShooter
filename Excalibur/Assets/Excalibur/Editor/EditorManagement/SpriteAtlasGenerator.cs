using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using Object = UnityEngine.Object;
using UnityEditorInternal;

namespace Excalibur
{
    public partial class SpriteAtlasGenerator : IWindowDrawer
    {
        public const float RETRACT = 20f;

        private string _spritesPath;
        private string _outputPath;
        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;

        private GenSpriteAtlasItem _overrallSettings = new GenSpriteAtlasItem();
        private List<GenSpriteAtlasItem> _genSpriteAtlasItems = new List<GenSpriteAtlasItem>();

        private TextureImporterPlatformSettings _platformSettings = new TextureImporterPlatformSettings();

        private TextureImporterFormat _textureImporterFormat;
        private TextureImporterCompression _textureImporterCompression;

        private static string[] Max_Tex_Size_Arr = new string[]
        {
            "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192"
        };

        private static string[] Padding = new string[] { "2", "4", "8" };

        public string title => "Generate SpriteAltas";

        Vector2 scrollPos;
        BuildTarget currentPlatform;
        bool showPlatformSettings;
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
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AtlasTex 源文件夹", GUILayout.Width(120f));
            EditorGUILayout.TextField(_spritesPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref _spritesPath, "选择AtlasTexture源目录",
                    () => _UpdateItems());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AtlasTex 输出文件夹", GUILayout.Width(120f));
            EditorGUILayout.TextField(_spritesPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref _outputPath, "选择AtlasTexture输出文件夹",() => 
                {
                    _genSpriteAtlasItems.ForEach(e => e.overrallOutput = _outputPath);
                    string path = IOAssistant.CombinePath(Application.dataPath, _outputPath);
                });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            currentPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Platform", _buildTarget);
            if (currentPlatform != _buildTarget)
            {
                _buildTarget = currentPlatform;
                _genSpriteAtlasItems.ForEach(e => e.buildTarget = _buildTarget);
            }
            EditorGUILayout.EndHorizontal();
            _overrallSettings.LayoutIncludeInBuild();
            _overrallSettings.LayoutSpriteAtlasPackingSettings();
            _overrallSettings.LayoutSpriteAtlasTextureSettings();
            LayoutTextureImporterPlatformSettings();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < _genSpriteAtlasItems.Count; ++i)
            {
                _genSpriteAtlasItems[i].LayoutGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        private void LayoutTextureImporterPlatformSettings()
        {
            EditorGUILayout.BeginVertical();
            showPlatformSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showPlatformSettings, "Platform Settings");
            if (showPlatformSettings)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(RETRACT);
                _platformSettings.overridden = EditorGUILayout.Toggle("Overriden", _platformSettings.overridden, GUILayout.Width(140f));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(RETRACT);
                int paddingIndex = EditorGUILayout.Popup("Padding", selectedMaxSizeIndex, Max_Tex_Size_Arr);
                if (paddingIndex != selectedMaxSizeIndex)
                {
                    _platformSettings.maxTextureSize = int.Parse(Max_Tex_Size_Arr[paddingIndex]);
                    selectedMaxSizeIndex = paddingIndex;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(RETRACT);
                importerFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Format", _platformSettings.format);
                if (importerFormat != _platformSettings.format)
                {
                    _platformSettings.format = importerFormat;
                    _genSpriteAtlasItems.ForEach(e => e.platformSettings.format = importerFormat);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(RETRACT);
                compression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", _platformSettings.textureCompression);
                if (compression != _platformSettings.textureCompression)
                {
                    _platformSettings.textureCompression = compression;
                    _genSpriteAtlasItems.ForEach(e => e.platformSettings.textureCompression = compression);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
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
                GenSpriteAtlasItem item = _genSpriteAtlasItems.Where(e => e.directory == EditorUtil.GetPathNameInAssets(directories[i])).FirstOrDefault();
                if (item == null)
                {
                    item = new GenSpriteAtlasItem();
                    item.overrallOutput = _outputPath;
                    _genSpriteAtlasItems.Add(item);
                }
                item.SetDirectoryAndPlatformSetting(directories[i], this);
            }
        }

        public void Terminate()
        {
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

        public static void GenerateSpriteAtlas(string inputPath, string outputpath, bool includeInBuild, BuildTarget buildTarget, TextureImporterPlatformSettings platformSettings, SpriteAtlasTextureSettings texSettings, SpriteAtlasPackingSettings packSettings)
        {
            SpriteAtlas atlas;
            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(outputpath + IOAssistant.FileExtension_SpriteAtlas);
            if (atlas == null)
            {
                string name = "NewSpriteAtlas";
                for (int i = 0; i < inputPath.Length; ++i)
                {
                    char value = inputPath[i];
                    if (value == '\\' || value == '/')
                    {
                        name = inputPath.Substring(i + 1);
                        break;
                    }
                }
                atlas = new SpriteAtlas();
                atlas.name = name;
                AssetDatabase.CreateAsset(atlas, outputpath);
            }

            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(inputPath);
            HashSet<Object> packables = new HashSet<Object>(atlas.GetPackables());
            Object[] outPackables = sprites.Where(e => !packables.Contains(e)).ToArray();
            atlas.Add(outPackables);

            atlas.SetIncludeInBuild(includeInBuild);
            atlas.SetTextureSettings(texSettings);
            atlas.SetPackingSettings(packSettings);
            TextureImporterPlatformSettings atlasSettings = atlas.GetPlatformSettings(buildTarget.ToString());
            atlasSettings.overridden = platformSettings.overridden;
            atlasSettings.maxTextureSize = platformSettings.maxTextureSize;
            atlasSettings.format = platformSettings.format;
            atlasSettings.textureCompression = platformSettings.textureCompression;
            atlas.SetPlatformSettings(atlasSettings);

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
            public BuildTarget buildTarget;
            public string directory;
            public TextureImporterPlatformSettings platformSettings;

            private SpriteAtlasGenerator _generator;
            private string _outputPath;
            private Grouping _grouping;
            private string[] _subDirectories;
            private string[] _spriteAtlasFolder;
            private SpriteAtlasTextureSettings _texSettings;
            private SpriteAtlasPackingSettings _packSettings;
            private Object[][] _sprites;
            private bool _showItem, _showTexSettings, _showPackingSettings;
            private bool[] _showItems;
            private int _paddingIndex;

            public GenSpriteAtlasItem() { }

            public void SetDirectoryAndPlatformSetting(string directory, SpriteAtlasGenerator generator)
            {
                _generator = generator;
                this.directory = directory;
                platformSettings = generator._platformSettings;
                buildTarget = generator._buildTarget;
                _GrabDirectories();
                this.directory = EditorUtil.GetPathNameInAssets(directory);
                _grouping = _subDirectories == null || _subDirectories.Length == 0 ? Grouping.AllInOne : Grouping.OneByOne;
                int length = _grouping == Grouping.AllInOne ? 1 : _subDirectories.Length;
                switch (_grouping)
                {
                    case Grouping.AllInOne:
                        _spriteAtlasFolder = new string[1] { this.directory };
                        _showItems = new bool[1];
                        _sprites = new Sprite[1][];
                        _sprites[0] = AssetDatabase.LoadAllAssetsAtPath("Assets/" + this.directory);
                        break;
                    case Grouping.OneByOne:
                        _spriteAtlasFolder = new string[length];
                        _showItems = new bool[length];
                        _sprites = new Object[length][];
                        for (int i = 0; i < length; ++i)
                        {
                            _spriteAtlasFolder[i] = _subDirectories[i].Split('/')[1];
                            List<string> subDirs = IOAssistant.GetDirectories(IOAssistant.CombinePath(Application.dataPath, _subDirectories[i]), string.Empty, SearchOption.TopDirectoryOnly).ToList();
                            subDirs.ForEach(e => e = IOAssistant.ConvertToUnityRelativePath(e));
                            string[] guidPaths = AssetDatabase.FindAssets("", subDirs.ToArray());
                            foreach (string item in guidPaths)
                            {
                                string fullPath = AssetDatabase.GUIDToAssetPath(item);
                                _sprites[i] = AssetDatabase.LoadAllAssetsAtPath(fullPath);
                            }
                        }
                        break;
                }
            }

            public void GenerateSpriteAtlases()
            {
                string inputPath, outputPath;
                switch (_grouping)
                {
                    case Grouping.AllInOne:
                        inputPath = IOAssistant.CombinePath(Application.dataPath, directory);
                        outputPath = IOAssistant.CombinePath(Application.dataPath, _outputPath);
                        GenerateSpriteAtlas(inputPath, outputPath, includeInBuild, buildTarget, platformSettings, _texSettings, _packSettings);
                        break;
                    case Grouping.OneByOne:
                        for (int i = 0; i < _subDirectories.Length; ++i)
                        {
                            inputPath = IOAssistant.CombinePath(Application.dataPath, _subDirectories[i]);
                            outputPath = IOAssistant.CombinePath(Application.dataPath, _outputPath, _spriteAtlasFolder[i]);
                            GenerateSpriteAtlas(inputPath, outputPath, includeInBuild, buildTarget, platformSettings, _texSettings, _packSettings);
                        }
                        break;
                }
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
                LayoutSpriteAtlasPackingSettings();
                LayoutSpriteAtlasTextureSettings();
                _LayoutSprites();
            }

            public void LayoutSpriteAtlasTextureSettings()
            {
                EditorGUILayout.BeginVertical();
                _showPackingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showPackingSettings, "Texture");
                if (_showPackingSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    _texSettings.readable = EditorGUILayout.Toggle("Read/Write", _texSettings.readable, GUILayout.Width(140f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    _texSettings.sRGB = EditorGUILayout.Toggle("sRGB", _texSettings.sRGB, GUILayout.Width(140f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    _texSettings.generateMipMaps = EditorGUILayout.Toggle("Generate MipMaps",
                        _texSettings.generateMipMaps, GUILayout.Width(140f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    _texSettings.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", _texSettings.filterMode);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }

            public void LayoutSpriteAtlasPackingSettings()
            {
                EditorGUILayout.BeginVertical();
                _showTexSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showTexSettings, "Packing");
                if (_showTexSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    _packSettings.enableRotation = EditorGUILayout.Toggle("Allow Rotation", _packSettings.enableRotation, GUILayout.Width(140f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    _packSettings.enableTightPacking = EditorGUILayout.Toggle("Allow TightPacking", _packSettings.enableTightPacking, GUILayout.Width(140f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    int paddingIndex = EditorGUILayout.Popup("Padding", _paddingIndex, Padding);
                    if (paddingIndex != _paddingIndex)
                    {
                        _packSettings.padding = int.Parse(Padding[paddingIndex]);
                        _paddingIndex = paddingIndex;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }

            public void LayoutIncludeInBuild()
            {
                EditorGUILayout.BeginHorizontal();
                includeInBuild = EditorGUILayout.Toggle("Include In Build", includeInBuild);
                EditorGUILayout.EndHorizontal();
            }

            private void _LayoutSprites()
            {
                EditorGUILayout.BeginVertical();
                _showItem = EditorGUILayout.BeginFoldoutHeaderGroup(_showItem, directory);
                if (_showItem)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(RETRACT);
                    for (int i = 0; i < _spriteAtlasFolder.Length; ++i)
                    {
                        EditorGUILayout.BeginVertical();
                        switch (_grouping)
                        {
                            case Grouping.AllInOne:
                                for (int j = 0; j < _sprites[0].Length; ++j)
                                {
                                    EditorGUILayout.ObjectField(new GUIContent(_sprites[0][j].name), _sprites[0][j], typeof(Object), false);
                                }
                                break;
                            case Grouping.OneByOne:
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(RETRACT);
                                EditorGUILayout.BeginVertical();
                                _showItems[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_showItems[i], _spriteAtlasFolder[i]);
                                if (_showItems[i])
                                {
                                    for (int j = 0; j < _sprites[i].Length; ++j)
                                    {
                                        EditorGUILayout.ObjectField(new GUIContent(_sprites[i][j].name), 
                                            _sprites[i][j], typeof(Object), false);
                                    }
                                }
                                EditorGUILayout.EndFoldoutHeaderGroup();
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.EndVertical();
                                break;
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }

            private void ApplayOverrallSettings()
            {
                includeInBuild = _generator._overrallSettings.includeInBuild;
                _texSettings = _generator._overrallSettings._texSettings;
                _packSettings = _generator._overrallSettings._packSettings;
            }

            public void Save (IDataWriter writer)
            {
                writer.Write(_outputPath);
                writer.Write(includeInBuild);
                SaveSpriteAtlasPackingSettings(writer);
                SaveSpriteAtlasTextureSettings(writer);
            }

            public void Load (IDataReader reader)
            {
                _outputPath = reader.ReadString();
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

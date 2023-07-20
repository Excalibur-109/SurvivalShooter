using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.U2D;
using UnityEditor.U2D;
using Excalibur.Algorithms;
using static UnityEditor.Progress;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Codice.CM.Interfaces;
using System.Diagnostics;

namespace Excalibur
{
    public class AssetBundleBuildPreset : IWindowDrawer
    {
        public string assetsPath;
        public string scenesPath;
        public string shadersPath;
        public string outPath;
        public BuildAssetBundleOptions buildAssetBundleOption = BuildAssetBundleOptions.None;
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;

        public string title => "AssetBundleBuild";

        public void Save(IDataWriter writer)
        {
            writer.Write(assetsPath);
            writer.Write(scenesPath);
            writer.Write(shadersPath);
            writer.Write(outPath);
            writer.Write((int)buildAssetBundleOption);
            writer.Write((int)buildTarget);
        }

        public void Load(IDataReader reader)
        {
            assetsPath = reader.ReadString();
            scenesPath = reader.ReadString();
            shadersPath = reader.ReadString();
            outPath = reader.ReadString();
            buildAssetBundleOption = (BuildAssetBundleOptions)reader.ReadInt();
            buildTarget = (BuildTarget)reader.ReadInt();
        }

        public void OpenWindow()
        {
            EditorUIManager.Instance.OpenWindow<PresetsWindow>(this);
        }

        public void OnEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("资源路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(assetsPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref assetsPath, "选择资源路径");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("场景路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(scenesPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref scenesPath, "选择场景路径");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("着色器路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(shadersPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref shadersPath, "选择着色器路径");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("输出路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(outPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref outPath, "选择");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            buildAssetBundleOption = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(buildAssetBundleOption);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildTarget);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Assets"))
            {
                AssetBundleBuilder.BuildAssetBundles(AssetsManager.BundleAssets.Assets);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Scenes"))
            {
                AssetBundleBuilder.BuildAssetBundles(AssetsManager.BundleAssets.Scenes);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Shaders"))
            {
                AssetBundleBuilder.BuildAssetBundles(AssetsManager.BundleAssets.Shaders);
            }
            EditorGUILayout.EndHorizontal();
        }

        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }

    public class ConfigurationPreset : IWindowDrawer
    {
        private const string FOLDER = "Tables";

        public string destinationPath;
        public string classPath;
        public string selectedDirectory;

        public string title => "Configuration";

        static Vector2 scrollPos;
        static string[] directories;
        static int selectedIndex, tempIndex;

        FormatExcelClsHandler clsHandler = new FormatExcelClsHandler();
        FormatXMLHandler xmlHandler = new FormatXMLHandler();

        public void OnEditorGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            tempIndex = EditorGUILayout.Popup(selectedIndex, directories);
            if (tempIndex != selectedIndex)
            {
                selectedDirectory = directories[tempIndex];
                selectedIndex = tempIndex;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("配置导出路径", GUILayout.Width(80f));
            EditorGUILayout.TextField(destinationPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref destinationPath, "选择导出路径", () =>
                    {
                        string path = Path.Combine(Application.dataPath, destinationPath);
                    });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("类导出路径", GUILayout.Width(80f));
            EditorGUILayout.TextField(classPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref classPath, "选择导出路径", () =>
                {
                    string path = Path.Combine(Application.dataPath, classPath);
                });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成相关类"))
            {
                if (string.IsNullOrEmpty(selectedDirectory))
                {
                    EditorUtility.DisplayDialog("Warning", "表目录不存在", "OK");
                    return;
                }
                
                if (string.IsNullOrEmpty(classPath))
                {
                    EditorUtility.DisplayDialog("Warning", "未选择输出路径", "OK");
                    return;
                }
                clsHandler.SetSrc(Path.Combine(EditorProjectPreset.GameAssetsPath, FOLDER, selectedDirectory));
                clsHandler.SetDst(Path.Combine(Application.dataPath, classPath));
                //EditorUtility.ClearProgressBar();
                clsHandler.Serialize();
            }
            //if (clsHandler != null)
            //{
            //    try
            //    {
            //        EditorUtility.DisplayProgressBar("Generating classes", "generate" + clsHandler.processInfo, clsHandler.process);
            //        if (clsHandler.process >= 1f)
            //        {
            //            clsHandler = null;
            //            EditorUtility.ClearProgressBar();
            //            AssetDatabase.Refresh();
            //        }
            //    }
            //    catch (ArgumentNullException e)
            //    {
            //        clsHandler = null;
            //        EditorUtility.ClearProgressBar();
            //        AssetDatabase.Refresh();
            //        throw e;
            //    }
            //}
            if (GUILayout.Button("生成配置"))
            {
                if (string.IsNullOrEmpty(selectedDirectory))
                {
                    EditorUtility.DisplayDialog("Warning", "表目录不存在", "OK");
                    return;
                }
                if (string.IsNullOrEmpty(destinationPath))
                {
                    EditorUtility.DisplayDialog("Warning", "未选择输出路径", "OK");
                    return;
                }
                xmlHandler.SetSrc(Path.Combine(EditorProjectPreset.GameAssetsPath, FOLDER, selectedDirectory));
                xmlHandler.SetDst(Path.Combine(Application.dataPath, destinationPath));
                //EditorUtility.ClearProgressBar();
                xmlHandler.Serialize();
            }
            //if (xmlHandler != null)
            //{
            //    try
            //    {
            //        EditorUtility.DisplayProgressBar("Generating configs", "generate" + xmlHandler.processInfo, xmlHandler.process);
            //        if (xmlHandler.process >= 1f)
            //        {
            //            xmlHandler = null;
            //            EditorUtility.ClearProgressBar();
            //            AssetDatabase.Refresh();
            //        }
            //    }
            //    catch (ArgumentNullException e)
            //    {
            //        xmlHandler = null;
            //        EditorUtility.ClearProgressBar();
            //        AssetDatabase.Refresh();
            //        throw e;
            //    }
            //}
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("打开路径"))
            {
                string path = IOAssistant.CombinePath(EditorProjectPreset.GameAssetsPath, FOLDER, selectedDirectory);
                Process.Start(path);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void OpenWindow()
        {
            EditorUIManager.Instance.OpenWindow<PresetsWindow>(this);
        }

        public void Load(IDataReader reader)
        {
            destinationPath = reader.ReadString();
            classPath = reader.ReadString();
            selectedDirectory = reader.ReadString();
        }

        public void Save(IDataWriter writer)
        {
            writer.Write(destinationPath);
            writer.Write(classPath);
            writer.Write(selectedDirectory);
        }

        public void Initialize()
        {
            string path = Path.Combine(EditorProjectPreset.GameAssetsPath, FOLDER);
            directories = Directory.GetDirectories(path);
                
            if (string.IsNullOrEmpty(selectedDirectory))
            {
                selectedDirectory = directories[0];
            }
            for (int i = 0; i < directories.Length; ++i)
            {
                string directory = directories[i];
                for (int j = directories[i].Length - 1; j >= 0; --j)
                {
                    char value = directory[j];
                    if (value == '\\')
                    {
                        directories[i] = directory.Substring(j + 1);
                        break;
                    }
                }
                if (selectedIndex == -1 && selectedDirectory == directories[i])
                {
                    selectedIndex = i;
                }
            }
        }

        public void Terminate()
        {
            directories = null;
            selectedIndex = -1;
            if (!string.IsNullOrEmpty(destinationPath))
            {
                ProjectSettings settings = Resources.Load<ProjectSettings>(CP.ProjectSettings);
                if (settings != null && settings.configurationPath != destinationPath)
                {
                    settings.configurationPath = destinationPath;
                }
            }
        }
    }

    public class ComponentsGen : IWindowDrawer
    {
        public string exportPath;
        List<ComItems> items = new List<ComItems>();
        List<Vector2> itemsView = new List<Vector2>();
        List<bool> itemsFoldState = new List<bool>();

        string[] comTypes = new string[] { "class", "struct" };
        int comTypeIndex = 0;

        static Color guiColor;
        bool gening = false;

        private class ComItems
        {
            public string fileName;
            public List<string> clsNames = new List<string>();
            public List<List<string>> types = new List<List<string>>();
            public List<List<string>> names = new List<List<string>>();

        }

        public string title => "ComponentsGen";

        public void Initialize()
        {
            gening = false;
            guiColor = GUI.backgroundColor;
            _LoadComponents();
        }

        static Vector2 scrollPos;

        public void OnEditorGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.HelpBox("1.Only append type allowed.\n2.不允许重复类名", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("数据文件夹", GUILayout.Width(120f));
            EditorGUILayout.TextField(exportPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref exportPath, "选择数据文件夹", () => _LoadComponents());
            }
            EditorGUILayout.EndHorizontal();
            int selectedIndex = EditorGUILayout.Popup(comTypeIndex, comTypes);
            if (selectedIndex != comTypeIndex)
            {
                comTypeIndex = selectedIndex;
            }

            //if (GUILayout.Button("Convert all to selected type(class/struct)"))
            //{
            //    _ConvertType();
            //}

            if (gening) { return; }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate All"))
            {
                _GenAll();
            }
            GUI.backgroundColor = guiColor;

            EditorGUILayout.Space(20);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < items.Count; ++i)
            {
                ComItems item = items[i];
                itemsFoldState[i] = EditorGUILayout.BeginFoldoutHeaderGroup(itemsFoldState[i], items[i].fileName);
                if (itemsFoldState[i])
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(20);
                    itemsView[i] = EditorGUILayout.BeginScrollView(itemsView[i]);
                    EditorGUILayout.BeginVertical();
                    for (int j = 0; j < item.clsNames.Count; ++j)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Class Name:", GUILayout.Width(100));
                        item.clsNames[j] = EditorGUILayout.TextField(item.clsNames[j]);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField("Fields:");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        for (int k = 0; k < item.types[j].Count; ++k)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Type:", GUILayout.Width(70));
                            item.types[j][k] = EditorGUILayout.TextField(item.types[j][k]);
                            EditorGUILayout.LabelField("Name:", GUILayout.Width(70));
                            item.names[j][k] = EditorGUILayout.TextField(item.names[j][k]);
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("Create Field"))
                        {
                            if (item.types[j].Contains(string.Empty)) { return; }
                            item.types[j].Add(string.Empty);
                            item.names[j].Add(string.Empty);
                        }
                        EditorGUILayout.Space(2);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Create New Class/Struct Here:");

                    if (GUILayout.Button("Create New Class/Struct"))
                    {
                        if (item.clsNames.Contains(string.Empty)) { return; }
                        item.clsNames.Add(string.Empty);
                        item.types.Add(new List<string>() { string.Empty });
                        item.names.Add(new List<string>() { string.Empty });
                    }
                    EditorGUILayout.EndVertical();

                    if (item.clsNames.Count > 0)
                    {
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button("Append"))
                        {
                            _GenAppendItem(item);
                        }
                        GUI.backgroundColor = guiColor;
                    }
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (GUILayout.Button("Add New File"))
            {
                ComItems tmp = new ComItems()
                {
                    fileName = $"ComponentDatas_{items.Count}",
                };
                items.Add(tmp);
                itemsView.Add(Vector2.zero);
                itemsFoldState.Add(false);
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        public void OpenWindow()
        {
            EditorWindow.GetWindow<GenComponentData>(title).Show();
        }

        public void Save(IDataWriter writer)
        {
            writer.Write(exportPath);
        }

        public void Load(IDataReader reader)
        {
            exportPath = reader.ReadString();
        }

        public void Terminate()
        {
            items.Clear();
        }

        private void _LoadComponents()
        {
            if (string.IsNullOrEmpty(exportPath)) { return; }
            items.Clear();
            string[] files = IOAssistant.GetFiles(Path.Combine(Application.dataPath, exportPath), string.Empty, SearchOption.TopDirectoryOnly, file => file.EndsWith(IOAssistant.FileExt_CS));
            if (files.Length > 0)
            {
                string clsName = string.Empty;
                for (int i = 0; i < files.Length; ++i)
                {
                    string file = files[i];
                    ComItems item = new ComItems()
                    {
                        fileName = Path.GetFileNameWithoutExtension(file),
                    };
                    items.Add(item);
                    itemsFoldState.Add(false);
                    itemsView.Add(Vector2.zero);
                    using (StreamReader reader = new StreamReader(File.OpenRead(file)))
                    {
                        int current = -1;
                        while (true)
                        {
                            string clsLine = reader.ReadLine();
                            if (string.IsNullOrEmpty(clsLine) && reader.Peek() < 0)
                            {
                                break;
                            }
                            if (KMP.Contains(clsLine, "public"))
                            {
                                string tmp = clsLine.TrimStart();
                                string[] elements = tmp.Split(' ');
                                if (KMP.Contains(clsLine, "class") || KMP.Contains(clsLine, "struct"))
                                {
                                    clsName = elements[2];
                                    item.clsNames.Add(clsName);
                                    item.types.Add(new List<string>());
                                    item.names.Add(new List<string>());
                                    ++current;
                                }
                                else
                                {
                                    item.types[current].Add(elements[1]);
                                    item.names[current].Add(elements[2].TrimEnd(new char[] { ';' }));
                                }
                            }
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        private static string _GetHeadStr()
        {
            string head = "using UnityEngine;\r\nusing Excalibur;\r\n\n";
            return head;
        }

        private void _ConvertType()
        {

            _LoadComponents();
        }

        private void _GenAll()
        {
            gening = true;
            for (int i = 0; i < items.Count; ++i)
            {
                _GenAppendItem(items[i]);
            }

            gening = false;
        }

        private void _GenAppendItem(ComItems item)
        {
            if (item.clsNames.Count == 0)
            {
                return;
            }

            gening = true;
            string file = IOAssistant.CombinePath(Application.dataPath, exportPath, item.fileName + IOAssistant.FileExt_CS);
            StringBuilder sb = new StringBuilder();
            sb.Append(_GetHeadStr());
            for (int i = 0; i < item.clsNames.Count; ++i)
            {
                string clsName = item.clsNames[i];
                if (string.IsNullOrEmpty(clsName)) { continue; }
                sb.AppendLine(string.Format("public {0} {1} : IComponent", comTypes[comTypeIndex], item.clsNames[i]));
                sb.AppendLine("{");
                for (int j = 0; j < item.types[i].Count; j++)
                {
                    string type = item.types[i][j];
                    string name = item.names[i][j];
                    if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name))
                    {
                        continue;
                    }
                    sb.AppendLine(string.Format("\tpublic {0} {1};", type, name));
                }
                sb.AppendLine("}\r\n");
            }
            File.WriteAllText(file, sb.ToString(), new UTF8Encoding(true));
            //using (StreamWriter writer = new StreamWriter(File.OpenWrite(file), new UTF8Encoding(true)))
            //{
            //    writer.Write(sb.ToString());
            //    writer.Dispose();
            //}
            gening = false;
        }
    }
}

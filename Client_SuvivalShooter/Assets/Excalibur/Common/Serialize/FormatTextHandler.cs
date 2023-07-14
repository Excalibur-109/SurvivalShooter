using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Excalibur
{
    public abstract class FormatFileHandler
    {
        protected string _srcPath;
        protected string _dstPath;
        protected string _result;
        protected Encoding _encoding = Encoding.UTF8; // new UTF8Encoding (true);
        public FormatFileHandler () { }

        public void SetSrc (string path) => _srcPath = path;
        public void SetDst (string path) => _dstPath = path;
        public void SetEncoding (Encoding encoding) => _encoding = encoding;
        public abstract void Serialize ();
        public abstract void Deserialize ();

        public static string configPath = "/Configurations";

        public float process;
        public string processInfo;

        public static string GetConfigClsName ()
        {
            return "Configurations.cs";
        }

        public static string GetDesializeConfigClsName ()
        {
            return "DeserializeConfigurations.cs";
        }

        public static string GetConfigContainerString ()
        {
            string str = "using System.Collections.Generic;\nusing System;\nusing UnityEngine;\nusing Excalibur;\r\n\r\n";
            return str;
        }

        public static string GetConfigClsString ()
        {
            string str = "///summary #ANNOTATION# /// summary\r\npublic static class #TYPE#Cfg\r\n{\r\n    public static Dictionary<int, #TYPE#> Config = new Dictionary<int, #TYPE#> ();\r\n\r\n    public class #TYPE#\r\n    {#FIELDS#\r\n    }\r\n\r\n    public static string GetName () => typeof (#TYPE#).Name;\r\n\r\n    public static void Deserialize () => Config = FormatXMLHandler.Deserialize<#TYPE#> (GetName ());\r\n\r\n    public static #TYPE# TryGetValue (int id)\r\n    {\r\n        #TYPE# value = default (#TYPE#);\r\n        try\r\n        {\r\n            value = Config[id];\r\n        }\r\n        catch (Exception e)\r\n        {\r\n            Debug.LogError ($\"{GetName ()}配置表不存在id为 ({id})的数据\");\r\n        }\r\n        return value;\r\n    }\r\n}\r\n";
            return str;
        }

        public static string GetDeserializeConfigurationsStr ()
        {
            string str = "public static class DeserializeConfigurations \r\n{\r\n\tpublic static void DeserilaizeConfigs()\r\n\t{\r\n#READ#\t}\r\n}";
            return str;
        }

        #region Serialize

        public static int[] ParseToIntArray (string value, char identifier = '|')
        {
            if (string.IsNullOrEmpty (value) || string.IsNullOrWhiteSpace (value)) { return null; }
            string[] array = value.Split (identifier);
            return ParseStringArrayToIntArray (array);
        }

        public static int[][] ParseToInt2Array (string value, char identifier1 = '|', char identifier2 = ':')
        {
            if (string.IsNullOrEmpty (value) || string.IsNullOrWhiteSpace (value)) { return null; }
            string[] array = value.Split (identifier1);
            int[][] ret = new int[array.Length][];
            for (int i = 0; i < array.Length; ++i)
            {
                string[] temp = array[i].Split (identifier2);
                ret[i] = ParseStringArrayToIntArray (temp); 
            }
            return ret;
        }

        public static float[][] ParseToFloat2Array (string value, char identifier1 = '|', char identifier2 = ':')
        {
            if (string.IsNullOrEmpty (value) || string.IsNullOrWhiteSpace (value)) { return null; }
            string[] array = value.Split (identifier1);
            float[][] ret = new float[array.Length][];
            for (int i = 0; i < array.Length; ++i)
            {
                string[] temp = array[i].Split (identifier2);
                ret[i] = ParseStringArrayToFloatArray (temp);
            }
            return ret;
        }

        public static float[] ParseToFloatArray (string value, char identifier = '|')
        {
            if (string.IsNullOrEmpty (value) || string.IsNullOrWhiteSpace (value)) { return null; }
            string[] array = value.Split (identifier);
            return ParseStringArrayToFloatArray (array);
        }

        public static int[] ParseStringArrayToIntArray (string[] array)
        {
            if (array == null) { return null; }
            int[] ret = new int[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                int.TryParse (array[i], out ret[i]);
            }
            return ret;
        }

        public static float[] ParseStringArrayToFloatArray (string[] array)
        {
            if (array == null) { return null; }
            float[] ret = new float[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                float.TryParse (array[i], out ret[i]);
            }
            return ret;
        }

        #endregion

        public static object FormatParse (string type, string value)
        {
            if (string.IsNullOrEmpty (type) | string.IsNullOrEmpty (value)) { return null; }
            object o = null;
            switch (type)
            {
                case "int":
                    int.TryParse (value, out int intRet);
                    o = intRet;
                    break;
                case "uint":
                    uint.TryParse (value, out uint uintRet);
                    o = uintRet;
                    break;
                case "float":
                    float.TryParse (value, out float floatRet);
                    o = floatRet;
                    break;
                case "string":
                    o = value;
                    break;
                case "int[]":
                    o = ParseToIntArray (value);
                    break;
                case "int[][]":
                    o = ParseToInt2Array (value);
                    break;
                case "float[]":
                    o = ParseToFloatArray (value);
                    break;
                case "float[][]":
                    o = ParseToFloat2Array (value);
                    break;
            }
            return o;
        }

        public static string GetFieldString (string type, string fieldName, string annotation = "")
        {
            return string.Format ("\n        ///summary {0} ///summary\n        public {1} {2};", annotation, type, fieldName);
        }
    }

    /// <summary> 生成配置数据相关类 /// </summary>
    public class FormatExcelClsHandler : FormatFileHandler
    {
        public override void Serialize ()
        {
            StringBuilder desrilaizeSB = new StringBuilder();
            process = 0f;
            try
            {
                string[] files = Directory.GetFiles (_srcPath);
                int total = files.Length;
                StringBuilder sb = new StringBuilder (GetConfigContainerString ());
                for (int i = 0; i < files.Length; ++i)
                {
                    string file = files[i].Replace ("\\", "/");
                    processInfo = file;
                    using (ExcelPackage package = new ExcelPackage (File.OpenRead (file)))
                    {
                        int sheetCount = package.Workbook.Worksheets.Count;
                        int current = 1;
                        while (current <= sheetCount)
                        {
                            ExcelWorksheet sheet = package.Workbook.Worksheets[current];
                            string name = sheet.Name;
                            if (name.Contains ("&"))
                            {
                                string[] ret = name.Split ('&');
                                string type = ret[1];
                                desrilaizeSB.AppendLine("\t\t" + type + "Cfg.Deserialize();");
                                StringBuilder sb2 = new StringBuilder ();
                                int column = 0;
                                while (column < sheet.Dimension.Columns)
                                {
                                    ++column;
                                    if (sheet.GetValue (2, column) == null) { continue; }
                                    sb2.Append (GetFieldString (sheet.GetValue (2, column).ToString (), sheet.GetValue (3, column).ToString (), sheet.GetValue (1, column).ToString ()));
                                    process += (float)(column + 1) / sheet.Dimension.Columns / total;
                                }
                                string clsStr = GetConfigClsString ();
                                clsStr = clsStr.Replace ("#ANNOTATION#", ret[0]);
                                clsStr = clsStr.Replace ("#FIELDS#", sb2.ToString ());
                                sb.AppendLine (clsStr.Replace ("#TYPE#", type));
                            }
                            ++current;
                        }

                        process = (float)(i + 1) / total;
                        package.Dispose ();
                    }
                }
                _result = sb.ToString ();
                if (!Directory.Exists(_dstPath)) { Directory.CreateDirectory(_dstPath); };  
                using (StreamWriter writer = new StreamWriter(File.Open(Path.Combine(_dstPath, GetConfigClsName()), FileMode.OpenOrCreate), _encoding))
                {
                    writer.Write(_result);
                }
                
                _result = GetDeserializeConfigurationsStr().Replace("#READ#", desrilaizeSB.ToString());
                if (!string.IsNullOrEmpty(_result))
                {
                    using (StreamWriter writer = new StreamWriter(File.Open(Path.Combine(_dstPath, GetDesializeConfigClsName()), FileMode.OpenOrCreate), _encoding))
                    {
                        writer.Write(_result);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void Deserialize ()
        {
        }
    }


    public class FormatXMLHandler : FormatFileHandler
    {
        public override void Serialize ()
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(e => e.GetName().Name == "Assembly-CSharp").ToArray()[0];
            string[] files = Directory.GetFiles (_srcPath);
            int total = files.Length;
            for (int i = 0; i < files.Length; ++i)
                {
                    string file = files[i].Replace ("\\", "/");
                    using (ExcelPackage package = new ExcelPackage (File.OpenRead (file)))
                    {
                        int sheetCount = package.Workbook.Worksheets.Count;
                        int current = 1;
                        while (current <= sheetCount)
                        {
                            ExcelWorksheet sheet = package.Workbook.Worksheets[current];
                            string name = sheet.Name;
                            if (name.Contains ("&"))
                            {
                                string[] ret = name.Split ('&');
                                string type = ret[1];
                                string typeName = string.Format ("{0}Cfg+{1}", type, type); /// 嵌套类中类名为+号连接。e.g. 外部类名+嵌套类名
                                Type typeRet = assembly.GetType (typeName);
                                List<object> list = new List<object> ();
                                FieldInfo[] fields = typeRet.GetFields ();
                                int row = 4, column;
                                while (row <= sheet.Dimension.Rows)
                                {
                                    column = 0;
                                    int fieldIndex = 0;
                                    var obj = Activator.CreateInstance (typeRet);
                                    while (column < sheet.Dimension.Columns)
                                    {
                                        ++column;
                                        object typeStr = sheet.GetValue (2, column);
                                        if (typeStr == null || string.IsNullOrEmpty (typeStr.ToString ())) { continue; }
                                        if (sheet.GetValue (row, column) != null)
                                        {
                                            object cellValue = sheet.GetValue (row, column);
                                            obj.GetType ().GetField (fields[fieldIndex++].Name)
                                                .SetValue (obj, FormatParse (typeStr.ToString (), cellValue.ToString ()));
                                        }
                                    }
                                    list.Add (obj);
                                    ++row;
                                }

                                XmlDocument xmlDocument = new XmlDocument ();
                                XmlDeclaration declaration = xmlDocument.CreateXmlDeclaration ("1.0", "utf-8", "");
                                xmlDocument.AppendChild (declaration);
                                XmlElement root = xmlDocument.CreateElement ($"{type}Cfg");
                                xmlDocument.AppendChild (root);
                                /// xmlElement Create
                                for (int j = 0; j < list.Count; ++j)
                                {
                                    XmlElement typeElement = xmlDocument.CreateElement (typeRet.Name);
                                    object o = list[j];
                                    for (int k = 0; k < fields.Length; ++k)
                                    {
                                        XmlElement element = xmlDocument.CreateElement (fields[k].Name);
                                        var value = o.GetType ().GetField (fields[k].Name).GetValue (o);
                                        switch (value)
                                        {
                                            case int a1:
                                            case uint a2:
                                            case float a3:
                                            case string a4:
                                                element.InnerText = value.ToString ();
                                                break;
                                            case int[] a5:
                                                if (a5 != null)
                                                {
                                                    for (int h = 0; h < a5.Length; ++h)
                                                    {
                                                        XmlElement child = xmlDocument.CreateElement ("int");
                                                        child.InnerText = a5[h].ToString ();
                                                        element.AppendChild (child);
                                                    }
                                                }
                                                break;
                                            case float[] a6:
                                                if (a6 != null)
                                                {
                                                    for (int h = 0; h < a6.Length; ++h)
                                                    {
                                                        XmlElement child = xmlDocument.CreateElement ("float");
                                                        child.InnerText = a6[h].ToString ();
                                                        element.AppendChild (child);
                                                    }
                                                }
                                                break;
                                            case int[][] a7:
                                                if (a7 != null)
                                                {
                                                    for (int h = 0; h < a7.Length; ++h)
                                                    {
                                                        XmlElement child = xmlDocument.CreateElement ($"index_{h}");
                                                        int[] t = a7[h];
                                                        if (t != null)
                                                        {
                                                            for (int m = 0; m < t.Length; ++m)
                                                            {
                                                                XmlElement c = xmlDocument.CreateElement ("int");
                                                                c.InnerText = t[m].ToString ();
                                                                child.AppendChild (c);
                                                            }
                                                        }
                                                        element.AppendChild (child);
                                                    }
                                                }
                                                break;
                                            case float[][] a8:
                                                if (a8 != null)
                                                {
                                                    for (int h = 0; h < a8.Length; ++h)
                                                    {
                                                        XmlElement child = xmlDocument.CreateElement ($"index_{h}");
                                                        float[] t = a8[h];
                                                        if (t != null)
                                                        {
                                                            for (int m = 0; m < t.Length; ++m)
                                                            {
                                                                XmlElement c = xmlDocument.CreateElement ("float");
                                                                c.InnerText = t[m].ToString ();
                                                                child.AppendChild (c);
                                                            }
                                                        }
                                                        element.AppendChild (child);
                                                    }
                                                }
                                                break;
                                        }
                                        typeElement.AppendChild (element);
                                    }
                                    root.AppendChild (typeElement);
                                }

                                string filePath = _dstPath + $"/{type}.xml";

                                XmlWriterSettings setting = new XmlWriterSettings ();
                                setting.Encoding = Encoding.UTF8;

                                using (StreamWriter sw = new StreamWriter (filePath, false, Encoding.UTF8))
                                {
                                    xmlDocument.Save (sw);
                                    sw.Close ();
                                }
                            }
                            ++current;
                        }
                        package.Dispose ();
                    }
                    process = (float)(i + 1) / total;
                }
        }

        public override void Deserialize ()
        {
        }

        public static Dictionary<int, T> Deserialize<T> (string name)
        {
            string path = string.Format ("{0}/{1}.xml", Application.dataPath + configPath, name);
            XmlDocument xmlDocument = new XmlDocument ();
            xmlDocument.Load (path);
            XmlNodeList nodes = xmlDocument.ChildNodes[1].ChildNodes;
            Dictionary<int, T> dic = new Dictionary<int, T> ();
            for (int i = 0; i < nodes.Count; ++i)
            {
                XmlNode node = nodes[i];
                T t = (T)Activator.CreateInstance (typeof (T));
                FieldInfo[] fields = t.GetType ().GetFields ();
                for (int j = 0; j < fields.Length; ++j)
                {
                    FieldInfo field = fields[j];
                    XmlNode n = node[field.Name];
                    string value = n.InnerText;
                    if (value == null || string.IsNullOrEmpty (value)) { continue; }
                    if (Comparer.IsTheSameType (field.FieldType, typeof (int)))
                    {
                        field.SetValue (t, int.Parse (value));
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (float)))
                    {
                        field.SetValue (t, float.Parse (value));
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (string)))
                    {
                        field.SetValue (t, value);
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (string)))
                    {
                        field.SetValue (t, value);
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (int[])))
                    {
                        int[] array = ParseXmlNodeToIntArray (n);
                        field.SetValue (t, array);
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (float[])))
                    {
                        float[] array = ParseXmlNodeToFloatArray (n);
                        field.SetValue (t, array);
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (int[][])))
                    {
                        int[][] array = ParseXmlNodeToIntArray2 (n);
                        field.SetValue (t, array);
                    }
                    else if (Comparer.IsTheSameType (field.FieldType, typeof (float[][])))
                    {
                        float[][] array = ParseXmlNodeToFloatArray2 (n);
                        field.SetValue (t, array);
                    }
                }
                dic.Add ( (int)fields[0].GetValue (t), t);
            }
            return dic;
        }

        private static int[] ParseXmlNodeToIntArray (XmlNode node)
        {
            int childCount = node.ChildNodes.Count;
            if (childCount == 0) { return null; }
            XmlNodeList nodeList = node.ChildNodes;
            int[] array = new int[childCount];
            for (int i = 0; i < childCount; ++i)
            {
                XmlNode child = nodeList[i];
                int.TryParse (child.InnerText, out array[i]);
            }
            return array;
        }

        private static float[] ParseXmlNodeToFloatArray (XmlNode node)
        {
            int childCount = node.ChildNodes.Count;
            if (childCount == 0) { return null; }
            XmlNodeList nodeList = node.ChildNodes;
            float[] array = new float[childCount];
            for (int i = 0; i < childCount; ++i)
            {
                XmlNode child = nodeList[i];
                float.TryParse (child.InnerText, out array[i]);
            }
            return array;
        }

        private static int[][] ParseXmlNodeToIntArray2 (XmlNode node)
        {
            int childCount = node.ChildNodes.Count;
            if (childCount == 0) { return null; }
            XmlNodeList nodeList = node.ChildNodes;
            int[][] array = new int[childCount][];
            for (int i = 0; i < childCount; ++i)
            {
                XmlNode child = nodeList[i];
                int[] a = ParseXmlNodeToIntArray (child);
                array[i] = a;
            }
            return array;
        }

        private static float[][] ParseXmlNodeToFloatArray2 (XmlNode node)
        {
            int childCount = node.ChildNodes.Count;
            if (childCount == 0) { return null; }
            XmlNodeList nodeList = node.ChildNodes;
            float[][] array = new float[childCount][];
            for (int i = 0; i < childCount; ++i)
            {
                XmlNode child = nodeList[i];
                float[] a = ParseXmlNodeToFloatArray (child);
                array[i] = a;
            }
            return array;
        }

        public static string XmlSerialize<T> (T obj)
        {
            string xmlString = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer (typeof (T));

            using (MemoryStream ms = new MemoryStream ())
            {
                xmlSerializer.Serialize (ms, obj);
                xmlString = Encoding.UTF8.GetString (ms.ToArray ());
                ms.Close ();
                ms.Dispose ();
            }
            return xmlString;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;

namespace MacacaGames.EffectSystem.Editor
{
    public class EffectSystemScriptBacker
    {

        public static void BakeAllEffectEnum(string effectEnumJson)
        {
            Dictionary<string, List<string>> json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(ModifyString(effectEnumJson));
            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace MacacaGames.EffectSystem");
            sb.AppendLine("{");
            sb.AppendLine("	public struct EffectSystemScriptable");
            sb.AppendLine("	{");
            sb.AppendLine();

            // BuildVaribale(sb, bp);
            foreach (var m in json)
            {
                BuildVaribale(sb, m.Key, m.Value);
            }

            sb.AppendLine();
            sb.AppendLine("	}");

            sb.AppendLine("}");


            string ScriptFile = GetPathToGeneratedScriptLocalization();

            var filePath = Application.dataPath + ScriptFile.Substring("Assets".Length);

            System.IO.File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            AssetDatabase.ImportAsset(ScriptFile);
        }


        static void BuildVaribale(StringBuilder sb, string varibaleName, List<string> bp)
        {
            if (bp == null)
            {
                return;
            }
            if (bp.Count == 0)
            {
                return;
            }
            sb.AppendLine($"		public struct {varibaleName}");
            sb.AppendLine("		{");
            foreach (var item in bp)
            {
                sb.AppendLine("		    public const string  " + item + " = \"" + item + "\";");
                sb.AppendLine();
            }
            sb.AppendLine("		}");
        }

        static string GetPathToGeneratedScriptLocalization()
        {

            CheckAndCreateResourceFolder();
            string[] assets = AssetDatabase.FindAssets("EffectSystemScriptable");
            if (assets.Length > 0)
            {
                try
                {
                    string FilePath = AssetDatabase.GUIDToAssetPath(assets[0]);
                    return FilePath;
                }
                catch (System.Exception)
                { }
            }

            return "Packages/MacacaGamesEffectSystem/Runtime/Utility/EffectSystemScriptable.cs";
        }

        public static void CheckAndCreateResourceFolder()
        {
            string effectSystemFolder =  "Packages/MacacaGamesEffectSystem/Runtime/Utility/";
            if (!Directory.Exists(effectSystemFolder))
            {
                Directory.CreateDirectory(effectSystemFolder);
                using (FileStream fs = File.Create(effectSystemFolder + "Auto Create by ViewSystem.txt"))
                {
                    Byte[] info = System.Text.Encoding.UTF8.GetBytes("This folder and contain datas is auto Created by ViewSystem, Delete this folder or any datas may cause ViewSystem works not properly.");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
                AssetDatabase.Refresh();
            }
        }

        public static string ModifyString(string json)
        {
            string result = json;
            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (result[i].ToString() == "[")
                {
                    if ((result[Mathf.Clamp(i + 1, 0, result.Length - 1)].ToString() == "\"" && result[Mathf.Clamp(i - 1, 0, result.Length - 1)].ToString() != ":") || i == 0)
                    {
                        result = result.Remove(i, 1);
                    }
                }
                else if (result[i].ToString() == "]")
                {
                    if ((result[Mathf.Clamp(i - 1, 0, result.Length - 1)].ToString() == "\"" && result[Mathf.Clamp(i + 1, 0, result.Length - 1)].ToString() != "}") || i == result.Length - 1)
                    {
                        result = result.Remove(i, 1);
                    }
                }
            }
            result = result.Replace("{", "");
            result = result.Replace("}", "");
            result = result.Insert(0, "{").Insert(result.Length + 1, "}");
            Debug.Log(result);
            return result;
        }
    }
}
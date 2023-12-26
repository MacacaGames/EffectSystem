using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Data;
using System.Linq;
using System.IO;
namespace MacacaGames.EffectSystem
{

    public class TextTemplateCreater : OdinEditorWindow
    {
        //[MenuItem("Window/Text Template Creater Window")]
        private static void OpenWindow()
        {
            GetWindow<TextTemplateCreater>().Show();
        }

        [BoxGroup("setting/設定")]
        [LabelText("模板")]
        [SerializeField]
        TextAsset template;

        [HorizontalGroup("setting", 0.3F, LabelWidth = 120)]
        [BoxGroup("setting/設定")]
        [LabelText("輸出指定Key")]
        [SerializeField]
        string exportKeyName;


        [BoxGroup("setting/資料")]
        [LabelText("")]
        [Multiline(10)]
        [SerializeField]
        string dataStr;

        public DataTable ConvertDataStr(string data)
        {
            DataTable dataTable = new DataTable();
            string[] lines = data.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    List<DataColumn> cols = lines[i].Trim().Split('\t').Select(_ => new DataColumn(_, typeof(string))).ToList();
                    dataTable.Columns.AddRange(cols.ToArray());
                }
                else
                {
                    DataRow row = dataTable.NewRow();
                    string[] items = lines[i].Trim().Split('\t');

                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        row[j] = (j >= items.Length) ? "" : items[j];
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }


        [Button(ButtonHeight = 30, Style = ButtonStyle.FoldoutButton)]
        public void CreateText()
        {
            string container = "";

            DataTable dataTable = ConvertDataStr(dataStr);
            for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                string temp = template.text;

                for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {

                    temp = temp.Replace(
                        dataTable.Columns[colIndex].ColumnName,
                        dataTable.Rows[rowIndex][colIndex] as string
                        );
                }

                container += temp + "\n";

                File.WriteAllText($"{Application.dataPath}/EFFECT.txt", container, System.Text.Encoding.UTF8);

                /*
                try
                {
                    File.WriteAllText($"{Application.dataPath}/{dataTable.Rows[rowIndex][exportKeyName] as string}", temp, System.Text.Encoding.UTF8);
                }
                catch(System.Exception e)
                {
                    Debug.Log(e.Message);
                }

                Debug.Log($"'{Application.dataPath}/{dataTable.Rows[rowIndex][exportKeyName] as string}' Done.");
                */
            }


        }






    }
}
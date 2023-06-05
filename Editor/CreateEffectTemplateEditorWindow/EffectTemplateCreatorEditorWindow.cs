
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using MacacaGames.EffectSystem;
namespace MacacaGames.EffectSystem.Editor
{

    public class EffectTemplateCreatorEditorWindow : EditorWindow
    {

        enum EffectKind
        {
            Element = 0,
            Trigger = 1,
        }


        [SerializeField]
        string effectName;

        [SerializeField]
        string effectTypeName;

        [SerializeField]
        EffectKind effectKind;


        [MenuItem("Window/Creator Effect Template")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<EffectTemplateCreatorEditorWindow>();
            wnd.Show();
            Selection.objects = new Object[] { wnd };
        }


        protected void OnEnable()
        {
            rootPath = $"{Application.dataPath}/0_Game/Scripts/Effect";
        }

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("EffectClassName", GUILayout.Width(100F));
                effectName = GUILayout.TextField(effectName);
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("EffectTypeName", GUILayout.Width(100F));
                effectTypeName = GUILayout.TextField(effectTypeName);
            }

            effectKind = (EffectKind)EditorGUILayout.EnumPopup(effectKind);

            GUILayout.Space(20F);
            if (GUILayout.Button("Create"))
            {
                AddEffectTemplate();
            }

        }



        const string effectTypeLine = "//{EFFECT_TYPE_LINE}";
        const string effectTypeQueryLine = "//{EFFECT_TYPE_QUERY_LINE}";

        string rootPath = ""; //$"{Application.dataPath}/Assets/0_Game/Scripts/Effect";
        string managerPath => $"{rootPath}/EffectManager.cs";

        string elementPath => $"{rootPath}/Elements";
        string triggerPath => $"{rootPath}/Triggers";


        string elementTemplatePath => $"{rootPath}/CreateEffectTemplateEditorWindow/Effect_Template.txt";
        string triggerTemplatePath => $"{rootPath}/CreateEffectTemplateEditorWindow/Effect_Trigger_Template.txt";

        //string Template

        //[Button]
        void AddEffectTemplate()
        {
            string s = File.ReadAllText(managerPath);

            s = s.Replace(effectTypeLine, $"{effectTypeName} = {GetEffectTypeMax() + 1}," + $"\n\t\t{effectTypeLine}");
            s = s.Replace(effectTypeQueryLine, $"[EffectType.{effectTypeName}] = typeof({effectName})," + $"\n\t\t{effectTypeQueryLine}");

            File.WriteAllText($"{managerPath}", s);

            if (effectKind == EffectKind.Element)
            {
                string t = File.ReadAllText(elementTemplatePath);
                t = t.Replace("{Effect_Template}", effectName);
                File.WriteAllText($"{elementPath}/{effectName}.cs", t);
            }
            else if (effectKind == EffectKind.Trigger)
            {
                string t = File.ReadAllText(triggerTemplatePath);
                t = t.Replace("{Effect_Trigger_Template}", effectName);
                File.WriteAllText($"{triggerPath}/{effectName}.cs", t);
            }

            EditorUtility.DisplayDialog("Effect Template Creator", $"{effectTypeName} is created now.", "OK");


            int GetEffectTypeMax()
            {
                return System.Enum.GetValues(typeof(EffectSystemScriptable.EffectType)).Cast<int>().Max();
            }
        }


    }

}
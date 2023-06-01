using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using MacacaGames;
using System.Reflection;
using System.Linq;

namespace MacacaGames.EffectSystem
{
#if (UNITY_EDITOR && false)
//[CustomPropertyDrawer(typeof(EffectInfo ))]
public class EffectInfoDrawer : OdinValueDrawer<EffectInfo >
{
    bool isUnfold = false;

    bool isSubInfoUnfold = false;
        
    protected override void DrawPropertyLayout(GUIContent label)
    {
        /*
        if(GUILayout.Button("Edit") == true)
        {
            EditorWindow.GetWindow<EffectInfoInspectorEditorWindow>();
        }
        */

        
        var groupedEffect = FindAllEffectTypeWithGroup();

        EffectInfo effectInfo = this.ValueEntry.SmartValue;

        using (var horizon = new GUILayout.HorizontalScope())
        {
            var c = groupedEffect.FirstOrDefault(m => m.item == effectInfo.type);
            CMEditorLayout.GroupedPopupField(ValueEntry.GetHashCode(), GUIContent.none, groupedEffect, c,
                (selected) =>
                {
                    effectInfo.type = selected.item;
                    ValueEntry.SmartValue = effectInfo;
                }
            );

            const float levelWidth = 120F;
            EditorGUIUtility.labelWidth = 40;
            effectInfo.level = EditorGUILayout.IntField("Level", effectInfo.level, GUILayout.Width(levelWidth));

            EditorGUIUtility.labelWidth = 0;
            isUnfold = EditorGUILayout.Toggle(isUnfold, GUILayout.Width(16));
        }

        const float tabWidth = 30F;
        const float detailLabelWidth = 70F;

        if (isUnfold == false)
            GUI.enabled = false;

        if (isUnfold)
        {
            using (var horizon = new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));
                EditorGUILayout.LabelField("Condition", GUILayout.Width(detailLabelWidth));
                effectInfo.activeCondition = (ActiveCondition)EditorGUILayout.EnumPopup(effectInfo.activeCondition);
                effectInfo.deactiveCondition = (DeactiveCondition)EditorGUILayout.EnumPopup(effectInfo.deactiveCondition);
            }

        }

        if(isUnfold)
        {
            using (var horizon = new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));

                EditorGUILayout.LabelField("MaintainTime", GUILayout.Width(detailLabelWidth));
                effectInfo.activeMaintainTime = EditorGUILayout.FloatField(effectInfo.activeMaintainTime, GUILayout.Width(50));


                EditorGUILayout.LabelField("CD(s)", GUILayout.Width(detailLabelWidth));
                effectInfo.coldDownTime = EditorGUILayout.FloatField(effectInfo.coldDownTime, GUILayout.Width(50));

            }
        }

        if (isUnfold)
        {
            using (var horizon = new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));

                EditorGUILayout.LabelField("Transition", GUILayout.Width(detailLabelWidth));
                effectInfo.triggerTransType = (TriggerTransType)EditorGUILayout.EnumPopup(effectInfo.triggerTransType);

                EditorGUILayout.LabelField("Flag", GUILayout.Width(detailLabelWidth));
                effectInfo.flag = (EffectInfo .EffectInfoFlag)EditorGUILayout.EnumFlagsField(effectInfo.flag);
            }
        }
        
        if (isUnfold || string.IsNullOrEmpty(effectInfo.note) == false)
        {
            using (var horizon = new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));
                EditorGUILayout.LabelField("Note", GUILayout.Width(detailLabelWidth));
                effectInfo.note = EditorGUILayout.TextField(effectInfo.note);
            }
        }

        if (isUnfold)
        {
            using (var horizon = new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));

                using (new GUILayout.VerticalScope())
                {
                    isSubInfoUnfold = EditorGUILayout.Foldout(isSubInfoUnfold, "SubInfo");
                    if (isSubInfoUnfold)
                    {

                        foreach (var subInfo in effectInfo.subInfos)
                        {
                            PropertyTree subInfoTree = PropertyTree.Create(subInfo);
                            subInfoTree.Draw(false);
                        }
                    }

                }
            }
        }

        GUI.enabled = true;

        ValueEntry.SmartValue = effectInfo;

        bool HasSetCondition()
        {
            return effectInfo.activeCondition != ActiveCondition.OnEffectStart || effectInfo.deactiveCondition != DeactiveCondition.None;
        }

        bool HasSetActiveAndDeactiveCondition()
        {
            return effectInfo.activeCondition != ActiveCondition.OnEffectStart && effectInfo.deactiveCondition != DeactiveCondition.None;
        }
        
    }


    CMEditorLayout.GroupedPopupData<EffectType>[] FindAllEffectTypeWithGroup()
    {
        List<CMEditorLayout.GroupedPopupData<EffectType>> result = new List<CMEditorLayout.GroupedPopupData<EffectType>>();

        foreach (var name in Enum.GetNames(typeof(EffectType)))
        {
            Enum.TryParse<EffectType>(name, true, out EffectType effectType);
            var type = EffectTypeQuery[effectType];
            var arrts = type.GetCustomAttributes(typeof(EffectTypeGroupAttribute), true);
            if (arrts.Length == 0)
            {
                result.Add(new CMEditorLayout.GroupedPopupData<EffectType> { group = "", item = effectType });
            }
            else
            {
                foreach (System.Attribute attr in arrts)
                {
                    if (attr is EffectTypeGroupAttribute)
                    {
                        EffectTypeGroupAttribute a = (EffectTypeGroupAttribute)attr;
                        result.Add(new CMEditorLayout.GroupedPopupData<EffectType> { group = a.group, item = effectType });
                        break;
                    }
                }
            }
        }
        return result.OrderBy(m => m.group).ToArray();
    }



    /*
public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
{
    var groupedEffect = FindAllEffectTypeWithGroup();

    EditorGUILayout.PropertyField(property);

    if (GUILayout.Button("Edit") == true)
    {
        EffectInfoInspectorEditorWindow.CreateWindow(property);
    }


    EditorGUI.BeginProperty(position, label, property);

    using (var horizon = new GUILayout.HorizontalScope())
    {
        var c = groupedEffect.FirstOrDefault(m => m.item == (EffectType)property.FindPropertyRelative("type").enumValueIndex);

        CMEditorLayout.GroupedPopupField(property.GetHashCode(), GUIContent.none, groupedEffect, c,
            (selected) =>
            {
                property.FindPropertyRelative("type").enumValueIndex = (int)selected.item;
                property.serializedObject.ApplyModifiedProperties();
            }
        );

        const float levelWidth = 120F;
        EditorGUIUtility.labelWidth = 40;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("level"), GUILayout.Width(levelWidth));

        EditorGUIUtility.labelWidth = 0;
        isUnfold = EditorGUILayout.Toggle(isUnfold, GUILayout.Width(16));
    }

    const float tabWidth = 30F;
    const float detailLabelWidth = 70F;

    if (isUnfold == false)
        GUI.enabled = false;

    if (isUnfold || HasSetCondition())
    {
        using (var horizon = new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));
            EditorGUILayout.LabelField("Condition", GUILayout.Width(detailLabelWidth));

            EditorGUILayout.PropertyField(property.FindPropertyRelative("activeCondition"), new GUIContent(""));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("deactiveCondition"), new GUIContent(""));
        }

        using (var horizon = new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));
            EditorGUILayout.LabelField("TransType", GUILayout.Width(detailLabelWidth));

            EditorGUILayout.PropertyField(property.FindPropertyRelative("triggeTransType"), new GUIContent(""));

        }
    }

    if (isUnfold == false && GUIColdDownTimeActiveCondition() ||
        isUnfold == true)
    {
        using (var horizon = new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));
            EditorGUILayout.LabelField("CD(s)", GUILayout.Width(detailLabelWidth));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("coldDownTime"), new GUIContent(""));
        }
    }

    if (isUnfold || string.IsNullOrEmpty(property.FindPropertyRelative("note").stringValue) == false)
    {
        using (var horizon = new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("", GUILayout.Width(tabWidth));
            EditorGUILayout.LabelField("Note", GUILayout.Width(detailLabelWidth));

            EditorGUILayout.PropertyField(property.FindPropertyRelative("note"), new GUIContent(""));
        }
    }

    GUI.enabled = true;

    EditorGUI.EndProperty();


    bool HasSetCondition()
        {
            return
                property.FindPropertyRelative("activeCondition").enumValueIndex != (int)ActiveCondition.OnEffectStart ||
                property.FindPropertyRelative("deactiveCondition").enumValueIndex != (int)DeactiveCondition.None;
        }

        bool GUIColdDownTimeActiveCondition()
        {
            return property.FindPropertyRelative("coldDownTime").floatValue > 0 && HasSetCondition();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0F;
    }
        */
}


#endif
}
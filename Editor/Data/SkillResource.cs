using MacacaGames.EffectSystem;
using MacacaGames.EffectSystem.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "GameResource/SkillResource", fileName = "SkillResource")]
public class SkillResource : ScriptableObject
{
    [SerializeField]
    Sprite defaultSkillIcon;
    [SerializeField]
    List<Sprite> skillIcons = new List<Sprite>();
    public Sprite GetSkillIcon(string iconName)
    {
        var icon = skillIcons.SingleOrDefault(m => iconName == m.name);
        if (icon == null)
        {
            return defaultSkillIcon;
        }

        return icon;
    }
    [SerializeField]
    Sprite defaultSkillRangeIcon;
    [SerializeField]
    List<Sprite> skillRangeIcons = new List<Sprite>();
    public Sprite GetSkillRangeIconIcon(string iconName)
    {
        var icon = skillRangeIcons.SingleOrDefault(m => iconName == m.name);
        if (icon == null)
        {
            return defaultSkillRangeIcon;
        }

        return icon;
    }
    [SerializeField]
    Sprite damageTypePhysic, damageTypeMagical, damageTypeTrue;
    public Sprite GetSkillDamageTypeIcon(string damageType)
    {


        switch (damageType)
        {
            case EffectSystemScriptable.DamageType.Physical:
                return damageTypePhysic;
            case EffectSystemScriptable.DamageType.Magical:
                return damageTypeMagical;
            case EffectSystemScriptable.DamageType.True:
                return damageTypeTrue;


        }
        return damageTypePhysic;

    }

    [Button]
    void BackeAllEffectEnum(string Json)
    {
        EffectSystemScriptBacker.BakeAllEffectEnum(Json);
    }
}
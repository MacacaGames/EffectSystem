using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using MacacaGames;
using MacacaGames.EffectSystem.Model;
using MacacaGames.GameSystem;
using ProjectInfinity.Model;


public class DescriptionBuilderForEffect : DescriptionBuilder
{
    public static string GetEffectDescriptionFormat<T>(string description, T script)
    {
        string result = description;
        List<string> parameters = GetTargetParams(result);
        foreach (var parameter in parameters)
        {
            result = result.Replace("{[" + parameter + "]}", TryGetEffectDescriptionValue(parameter, script));
        }
        return result;
    }
    
    /// <summary>
    /// EffectInfo的第Index個subInfo
    /// </summary>
    /// <param name="script"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static EffectInfo? GetEffectSubInfoByIndex<T>(T script, int index)
    {
        bool isSuccess = ReturnListValueInScriptByStr($"subInfos[{index}]", script, out var result);
        if (isSuccess)
        {
            return (EffectInfo)result;
        }
        Debug.Log($"[DescriptionBuilder] EffectSubInfoByIndex | {result}");
        return null;
    }
    

    /// <summary>
    /// 撈ActiveRequirement的ConditionRequirement
    /// </summary>
    /// <param name="script"></param>
    /// <param name="parameter"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static ConditionRequirement GetActiveRequirementByIndex<T>(T script, int index)
    {
        object effectInfo = script;
        EffectInfo info = (EffectInfo)effectInfo;

        ConditionRequirement activeConditionRequirement =
            info.activeRequirementLists.Find(x => x.id == info.activeRequirement[index]);

        return activeConditionRequirement;
    }

    /// <summary>
    /// 撈DeactiveRequirement的ConditionRequirement
    /// </summary>
    /// <param name="script"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static ConditionRequirement GetDeactiveRequirementByIndex<T>(T script, int index)
    {
        object effectInfo = script;
        EffectInfo info = (EffectInfo)effectInfo;

        ConditionRequirement deactiveConditionRequirement =
            info.deactiveRequirementLists.Find(x => x.id == info.deactiveRequirement[index]);

        return deactiveConditionRequirement;
    }

    /// <summary>
    /// 計算數量，和第index個subinfo一樣EffectType
    /// </summary>
    /// <param name="script"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static int GetSubInfoAmountByIndex<T>(T script, int index)
    {
        object effectInfo = script;
        EffectInfo info = (EffectInfo)effectInfo;

        return info.subInfos.Count(x => x.type == info.subInfos[index].type);
    }

    /// <summary>
    /// 在script中搜尋parameter
    /// </summary>
    /// <param name="script"></param>
    /// <param name="parameter"></param>
    /// <param name="convertPercent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static string GetScriptParameter<T>(T script, string parameter, bool convertPercent)
    {
        bool isSuccess = ReturnValueInScriptByStr(parameter, script, out var result);

        if (!isSuccess) return $"SkillParameterToPercent error: {result}";

        if (convertPercent)
        {
            //整數的話不用計算直接加上%
            if (IsPropertyInt(script, parameter))
            {
                return $"{result}%";
            }

            double resultPercent = Convert.ToDouble(result);
            //將數字格式化成不帶小數點的百分比
            return resultPercent.ToString("P0");
        }

        return result.ToString();
    }

    /// <summary>
    /// 變數是否為Int
    /// </summary>
    /// <param name="script"></param>
    /// <param name="parameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static bool IsPropertyInt<T>(T script, string parameter)
    {
        PropertyInfo propInfo = script.GetType().GetProperty(parameter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propInfo != null)
        {
            Type propertyType = propInfo.PropertyType;
            return propertyType == typeof(int);
        }

        //如果屬性不存在，嘗試獲取變數
        FieldInfo fieldInfo = script.GetType().GetField(parameter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            Type propertyType = fieldInfo.FieldType;
            return propertyType == typeof(int);
        }

        return false;
    }

    static string TryGetEffectDescriptionValue<T>(string parameter, T script)
    {
        string[] orders = parameter.Split(".");

        object result = script;
        foreach (var order in orders)
        {
            result = OrderHandler(result, order);
        }

        return result.ToString();
    }

    static object OrderHandler<T>(T script, string order)
    {
        object result = null;

        //if there is one or more number
        Regex regex = new Regex(@"\d+");
        MatchCollection matches = regex.Matches(order);

        //判斷是否為百分比
        bool converPercent = order.EndsWith("%");
        order = converPercent ? order.Replace("%", string.Empty) : order;

        int index = -1;

        if (matches.Count <= 0)
        {
            //就只搜尋script的order
            return GetScriptParameter(script, order, converPercent);
        }
        index = Convert.ToInt32(matches.First().Value);

        order = order.Replace($"{index}", string.Empty);

        if (order is "subInfos")
        {
            return GetEffectSubInfoByIndex(script, index);
        }

        if (order is "activeRequirements")
        {
            return GetActiveRequirementByIndex(script, index);
        }

        if (order is "deactiveRequirements")
        {
            return GetDeactiveRequirementByIndex(script, index);
        }

        if (order is "subInfoAmount")
        {
            return GetSubInfoAmountByIndex(script, index);
        }

        return null;
    }
    
    #region Testing

    public static void RunTest()
    { 
        EffectInfo testEffectInfo =  new EffectInfo();  
        testEffectInfo.value = 1;
        testEffectInfo.subInfoIds = new List<string>(){"lemur_self_hide_e1sub1"};
        Debug.Log(GetEffectDescriptionFormat("Deal {[value%]} x ATK damage. Gain stealth for {[subInfos0.maintainTime]} actions.", testEffectInfo));
    }
    #endregion
}




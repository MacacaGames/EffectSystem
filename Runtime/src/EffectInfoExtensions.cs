using MacacaGames.EffectSystem;
using MacacaGames.EffectSystem.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public static class EffectInfoExtensions
{
    /// <summary>
    /// 修改 EffectInfo 結構體中的指定屬性值，並返回修改後的新結構體實例
    /// </summary>
    /// <param name="effectInfo"></param>
    /// <param name="parameterName">example: nameof(info.value)</param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static EffectInfo SetEffectInfoValue(this EffectInfo effectInfo, string parameterName, object newValue)
    {
        if (string.IsNullOrEmpty(parameterName))
            throw new ArgumentNullException(parameterName);

        // 將EffectInfo轉換為JSON
        string json = JsonConvert.SerializeObject(effectInfo);
        
        // 解析JSON為JObject
        JObject jObject = JObject.Parse(json);
        
        // 檢查參數是否存在
        if (!jObject.ContainsKey(parameterName))
            throw new ArgumentException($"Parameter '{parameterName}' not found in EffectInfo");
            
        // 修改參數值 - 保留原始類型
        JToken token = JToken.FromObject(newValue);
        jObject[parameterName] = token;
        
        // 將修改後的JObject轉換回EffectInfo
        EffectInfo updatedInfo = JsonConvert.DeserializeObject<EffectInfo>(jObject.ToString());
        
        // 回傳修改後的 EffectInfo 結構體
        return updatedInfo;
    }

    public static bool isShowBattleLog = true;
    
    public static void Log(string msg)
    {
        if(!isShowBattleLog) return;
        Debug.Log(msg);
    }
    
    public static void LogError(string msg)
    {
        if(!isShowBattleLog) return;
        Debug.LogError(msg);
    }
    
    public static void LogWarning(string msg)
    {
        if(!isShowBattleLog) return;
        Debug.LogWarning(msg);
    }
    
    public static void Log(Exception msg)
    {
        if(!isShowBattleLog) return;
        Debug.Log(msg);
    }
    
    public static void LogError(Exception msg)
    {
        if(!isShowBattleLog) return;
        Debug.LogError(msg);
    }
    
    public static void LogWarning(Exception msg)
    {
        if(!isShowBattleLog) return;
        Debug.LogWarning(msg);
    }
}
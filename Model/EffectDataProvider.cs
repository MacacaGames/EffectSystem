using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MacacaGames.EffectSystem.Model
{

    public static class EffectDataProvider
    {
        public static Func<List<string>, List<EffectInfo>> GetEffectInfo { get; private set; }

        public static void SetEffectInfoDelegate(Func<List<string>, List<EffectInfo>> GetEffectInfo)
        {
            EffectDataProvider.GetEffectInfo = GetEffectInfo;
        }

        /// <summary>
        /// 需要依賴外部回傳值，server、client皆須註冊。
        /// </summary>
        public static Func<List<string>, List<EffectViewInfo>> GetEffectViewInfo { get; private set; }
        public static void SeEffectViewInfoDelegate(Func<List<string>, List<EffectViewInfo>> GetEffectViewInfo)
        {
            EffectDataProvider.GetEffectViewInfo = GetEffectViewInfo;
        }

        public static Func<string, string> GetEffectDescriptionString { get; private set; }

        public static void SetEffectDescriptionStringDelegate(Func<string, string> GetEffectDescriptionString)
        {
            EffectDataProvider.GetEffectDescriptionString = GetEffectDescriptionString;
        }

        public static Dictionary<string, Type> EffectTypeQuery = new Dictionary<string, Type>();
        public static void RegisteEffectTypeQuery(Dictionary<string, Type> EffectTypeQuery)
        {
            EffectDataProvider.EffectTypeQuery = EffectTypeQuery;
        }
    }
}
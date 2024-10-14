using System;
using System.Collections.Generic;

namespace MacacaGames.EffectSystem.Model
{
    public static class  EffectDataProvider
    {
        public static Func<List<string>, List<EffectInfo>> GetEffectInfo { get; private set; }
        public static void SetEffectInfoDelegate(Func<List<string>, List<EffectInfo>> GetEffectInfo)
        {
            EffectDataProvider.GetEffectInfo = GetEffectInfo;
        }
        public static Func<string, string> GetEffectDescriptionString { get; private set; }
        public static Func<List<string>, List<EffectViewInfo>> GetEffectViewInfo { get; private set; }
        public static void SeEffectViewInfoDelegate(Func<List<string>, List<EffectViewInfo>> GetEffectInfo)
        {
            EffectDataProvider.GetEffectViewInfo = GetEffectInfo;
        }

        public static void SetEffectDescriptionStringDelegate(Func<string, string> GetEffectDescriptionString)
        {
            EffectDataProvider.GetEffectDescriptionString = GetEffectDescriptionString;
        }
    }
}
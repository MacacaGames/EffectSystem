using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public partial struct EffectSystemScriptable
    {
        public partial struct DeactiveCondition
        {
            public const string None = "None";
            public const string AfterActive = "AfterActive";
        }
        public partial struct ActiveCondition
        {
            public const string None = "None";
            public const string OnEffectStart = "OnEffectStart";
        }
    }
}
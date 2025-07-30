
namespace MacacaGames.EffectSystem
{
    public partial struct EffectSystemScriptableBuiltIn
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
            public const string OnEffectCooldownEnd = "OnEffectCooldownEnd";
        }
        public partial struct TimerTickerId
        {
            public const string Default = "Default";
        }
    }
}
using System.Collections;
using System.Collections.Generic;


namespace MacacaGames.EffectSystem
{
    [EffectTypeGroup("Trigger")]
    public abstract class EffectTriggerBase : EffectBase
    {
        public override void OnActive(EffectSystem.EffectTriggerConditionInfo condidionInfo)
        {
            ExecuteActive(condidionInfo);
            OnTrigger(condidionInfo);
        }

        protected abstract void OnTrigger(EffectSystem.EffectTriggerConditionInfo conditionInfo);

    }
}
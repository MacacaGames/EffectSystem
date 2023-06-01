using System.Collections;
using System.Collections.Generic;


namespace MacacaGames.EffectSystem
{
    [EffectTypeGroup("Trigger")]
    public abstract class EffectTriggerBase : EffectBase
    {
        public override void OnActive(EffectManager.EffectTriggerConditionInfo condidionInfo)
        {
            ExecuteActive(condidionInfo);
            OnTrigger(condidionInfo);
        }

        protected abstract void OnTrigger(EffectManager.EffectTriggerConditionInfo conditionInfo);

    }
}
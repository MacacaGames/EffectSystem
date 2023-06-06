using System.Collections;
using System.Collections.Generic;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    [EffectTypeGroup("Trigger")]
    public abstract class EffectTriggerBase : EffectBase
    {
        public override void OnActive(EffectTriggerConditionInfo condidionInfo)
        {
            ExecuteActive(condidionInfo);
            OnTrigger(condidionInfo);
        }

        protected abstract void OnTrigger(EffectTriggerConditionInfo conditionInfo);

    }
}
using System;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    [EffectTypeGroup("Trigger")]
    public abstract class EffectTriggerBase : EffectInstanceBase
    {
        public EffectTriggerBase(EffectSystem effectSystem) : base(effectSystem)
        {
        }
        public override void OnActive(EffectTriggerConditionInfo condidionInfo)
        {
            base.OnActive(condidionInfo);
            OnTrigger(condidionInfo);
        }

        protected abstract void OnTrigger(EffectTriggerConditionInfo conditionInfo);

        
    }
}
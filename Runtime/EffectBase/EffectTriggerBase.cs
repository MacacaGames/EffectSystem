using System;
using System.Collections;
using System.Collections.Generic;
using MacacaGames.EffectSystem.Model;
using System.Reflection;
using System.Linq;

namespace MacacaGames.EffectSystem
{
    [EffectTypeGroup("Trigger")]
    public abstract class EffectTriggerBase : EffectInstanceBase
    {
        public override void OnActive(EffectTriggerConditionInfo condidionInfo)
        {
            base.OnActive(condidionInfo);
            OnTrigger(condidionInfo);
        }

        protected abstract void OnTrigger(EffectTriggerConditionInfo conditionInfo);

        
    }
}
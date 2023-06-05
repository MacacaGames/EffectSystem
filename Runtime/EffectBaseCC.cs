using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    public class EffectBaseCC : EffectBase
    {
        public override void OnDeactive(EffectSystem.EffectTriggerConditionInfo triggerConditionInfo)
        {
            var effect = GetCCPreventionEffect();
            effectManager.AddRequestedEffect(owner, effect);
            base.OnDeactive(triggerConditionInfo);
        }

        public EffectInfo GetCCPreventionEffect()
        {
            EffectInfo effect = new EffectInfo();
            effect.type = EffectSystemScriptable.EffectType.CCPrevention;
            effect.deactiveCondition = EffectSystemScriptable.DeactiveCondition.OnOwnerAfterDoAttack;
            effect.activeRequirementLists = new List<List<ConditionRequirement>>();
            effect.deactiveRequirementLists = new List<List<ConditionRequirement>>();
            effect.activeMaintainActions = 5;
            effect.logic = EffectSystemScriptable.EffectInfoLogic.DestroyAfterMaintainTimeEnd;
            // effect.subInfos = new List<EffectInfo>();
            effect.parameters = new List<int>();
            // effect.viewInfos = new List<EffectViewInfo>();
            effect.tags = new List<string>() { "unremovable" };

            return effect;
        }
    }
}
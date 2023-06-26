using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MacacaGames.EffectSystem.Model;
using System;
using System.Threading.Tasks;

namespace MacacaGames.EffectSystem
{
    public class EffectCondition
    {
        public EffectInstanceBase effectInstance;

        EffectInfo effectInfo => effectInstance.info;

        public IEffectTimer maintainTimeTimer { get; private set; }
        public IEffectTimer cooldownTimeTimer { get; private set; }

        public bool isActive { get; private set; }

        public EffectCondition(EffectInstanceBase effectInstance)
        {
            this.effectInstance = effectInstance;
            isActive = false;

            cooldownTimeTimer = new DefaultTimerBase(
                null, OnColdDownTimeEnd, null, null
            );
            maintainTimeTimer = new DefaultTimerBase(
               null, OnMaintainTimeEnd, null, null
           );
        }

        public void Start()
        {
            //在EditMode時不走Condition，直接Active
            if (Application.isPlaying == false)
            {
                isActive = true;
                return;
            }

            if (effectInfo.activeCondition == EffectSystemScriptable.ActiveCondition.OnEffectStart)
            {
                OnActive(new EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });
            }
            else
            {
                cooldownTimeTimer.OnTimerComplete();
            }
        }

        public void End()
        {
            if (isActive == true)
            {
                OnDeactive(new EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });
            }

            maintainTimeTimer.Stop();
        }

        // Active / Deactive
        public void OnActive(EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            //檢查機率觸發
            if (UnityEngine.Random.Range(0F, 100F) >= effectInfo.activeProbability && effectInfo.activeProbability != 0F)
            {
                //Debug.Log("Active 機率沒中！");
                return;
            }

            //檢查ColdDown
            if (cooldownTimeTimer.IsCounting == true)
            {
                return;
            }

            ForceActive(info);
        }

        void ForceActive(EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            //Trans處理
            if (isActive == true)
            {
                switch (effectInfo.triggerTransType)
                {
                    case TriggerTransType.CutOldOne:
                        ForceDeactive(new EffectTriggerConditionInfo
                        {
                            owner = effectInstance.owner
                        });
                        break;

                    case TriggerTransType.SkipNewOne:
                        return;
                }
            }

            isActive = true;

            effectInstance.OnActive(info);

            if (effectInfo.deactiveCondition == EffectSystemScriptable.DeactiveCondition.AfterActive)
            {
                ForceDeactive(new EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });

                if (effectInfo.logic == EffectLifeCycleLogic.OnlyActiveOnce)
                {
                    effectInstance.RemoveEffect();
                }
            }
            else
            {
                //不會自毀的話才算時間
                maintainTimeTimer.Start(effectInfo.maintainTime);
            }
        }

        public void OnDeactive(EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            //只有Active時才能Deactive
            if (isActive == true)
            {
                //檢查機率觸發
                if (effectInfo.deactiveProbability != 0F && UnityEngine.Random.Range(0F, 100F) >= effectInfo.deactiveProbability)
                {
                    Debug.Log("Dective 機率沒中！");
                    return;
                }

                ForceDeactive(info);
            }
        }

        public void ForceDeactive(EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            isActive = false;

            maintainTimeTimer.Stop();
            cooldownTimeTimer.Start(effectInfo.cooldownTime);

            effectInstance.OnDeactive(info);
        }

        void OnColdDownTimeEnd()
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            effectInstance.OnColdownEnd();
            if (effectInfo.logic == EffectLifeCycleLogic.ReactiveAfterCooldownEnd)
            {
                OnActive(new EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });
            }
        }

        void OnMaintainTimeEnd()
        {
            ForceDeactive(new EffectTriggerConditionInfo
            {
                owner = effectInstance.owner
            });
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MacacaGames.EffectSystem.Model;
using System;
using System.Threading.Tasks;
using Random = System.Random;

namespace MacacaGames.EffectSystem
{
    public class EffectCondition
    {
        public EffectInstanceBase effectInstance;

        EffectInfo effectInfo => effectInstance.info;

        public IEffectTimer maintainTimeTimer { get; private set; }
        public IEffectTimer cooldownTimeTimer { get; private set; }

        public bool isActive { get; private set; }
        public bool isFirstTimeActive { get; private set; }

        public EffectCondition(EffectInstanceBase effectInstance)
        {
            this.effectInstance = effectInstance;
            isActive = false;

            cooldownTimeTimer = new DefaultTimerBase(
                null, OnCooldownTimeEnd, null, null,null
            );
            maintainTimeTimer = new DefaultTimerBase(
               null, OnMaintainTimeEnd, null, null,null
           );
        }

        public void Start()
        {
#if !Server
            //在EditMode時不走Condition，直接Active
            if (Application.isPlaying == false)
            {
                isActive = true;
                return;
            }
#endif

            isFirstTimeActive = true;
            if (effectInfo.activeCondition == EffectSystemScriptableBuiltIn.ActiveCondition.OnEffectStart)
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


            if (effectInfo.activeRequirementLists != null)
            {
                foreach (var requirement in effectInfo.activeRequirementLists)
                {
                    if (requirement.IsRequirementsFulfilled(info) == false)
                    {
                        Debug.Log($"[Effect Debug] {info} 的 ActiveRequirementList {requirement} 條件不符合，無法啟動");
                        return;
                    }
                }
            }

            var random = new System.Random();
            //檢查機率觸發
            if (random.NextDouble() > effectInfo.activeProbability)
            {
                //Debug.Log("Active 機率沒中！");
                return;
            }

            //檢查Cooldown
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
                    
                    case TriggerTransType.KeepOldOneWithoutTimerReset:
                    {
                        if (isFirstTimeActive)
                        {
                            isFirstTimeActive = false;
                        }
                        else if (!isFirstTimeActive && maintainTimeTimer.IsFinish)
                        {
                            isActive = false;
                            return;
                        }
                        break;
                    }
                }
            }

            isActive = true;

            effectInstance.OnActive(info);

            if (effectInfo.deactiveCondition == EffectSystemScriptableBuiltIn.DeactiveCondition.AfterActive)
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
                if (isFirstTimeActive)
                {
                    //不會自毀的話才算時間
                    maintainTimeTimer.Start(effectInfo.maintainTime);
                }
            }
        }

        public void OnDeactive(EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            if (effectInfo.deactiveRequirementLists != null)
            {
                foreach (var requirement in effectInfo.deactiveRequirementLists)
                {
                    if (requirement.IsRequirementsFulfilled(info) == false)
                    {
                        Debug.Log($"[Effect Debug] {info} 的 ActiveRequirementList {requirement} 條件不符合，無法deactive");
                        return;
                    }
                }
            }


            //只有Active時才能Deactive
            if (isActive == true)
            {
                var random = new System.Random();
                //檢查機率觸發
                if (random.NextDouble() > effectInfo.deactiveProbability)
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

            effectInstance.OnDeactive(info);
        }

        void OnCooldownTimeEnd()
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            effectInstance.OnCooldownEnd();
            if (effectInfo.activeCondition == EffectSystemScriptableBuiltIn.ActiveCondition.OnEffectCooldownEnd)
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
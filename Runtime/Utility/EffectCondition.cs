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


        public bool isActive { get; private set; }

        public EffectCondition(EffectInstanceBase effectInstance)
        {
            this.effectInstance = effectInstance;
            isActive = false;
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
                OnColdDownTimeEnd();
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

            StopActiveMaintainTime();
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
            if (CheckColdDownTime() == false)
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
                StartActiveMaintainTime();
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

            StopActiveMaintainTime();
            StartColdDownTime();

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

        #region Time Management
        public void Update(float delta)
        {
            if (currentColddownStartTime > 0)
            {
                currentColddownStartTime -= delta;
            }
            if (currentMaintainTime > 0)
            {
                currentMaintainTime -= delta;
            }
        }
        #endregion
        #region Cold Time         

        bool CheckColdDownTime()
        {
            if (effectInfo.cooldownTime < 0)
                return false;

            return currentColddownStartTime > 0;
        }

        float currentColddownStartTime = 0;
        Coroutine coldTimeCoroutine;
        void StartColdDownTime()
        {
            currentColddownStartTime = effectInfo.cooldownTime;
            if (effectInfo.cooldownTime > 0F)
            {
                coldTimeCoroutine = CoroutineManager.Instance.StartCoroutine(ColdDownTimeChecking());
            }

            IEnumerator ColdDownTimeChecking()
            {
                while (true)
                {
                    if (currentColddownStartTime <= 0 || Mathf.Approximately(currentColddownStartTime, 0))
                        break;
                    yield return null;
                }
                ColdDownTimeEndTrigger();
            }

            void ColdDownTimeEndTrigger()
            {
                coldTimeCoroutine = null;
                OnColdDownTimeEnd();
            }
        }

        void StopColdDownTime()
        {
            if (coldTimeCoroutine != null)
            {
                CoroutineManager.Instance.StopCoroutine(coldTimeCoroutine);
            }
        }
        #endregion

        #region Maintain Time

        float currentMaintainTime = 0;
        Coroutine maintainTimeCoroutine = null;
        void StartActiveMaintainTime()
        {
            float currentMaintainTime = effectInfo.maintainTime;

            if (currentMaintainTime > 0F)
            {
                CoroutineManager.Instance.StartCoroutine(MaintainTimeChecking());
            }

            IEnumerator MaintainTimeChecking()
            {
                while (true)
                {
                    if (currentMaintainTime <= 0 || Mathf.Approximately(currentMaintainTime, 0))
                        break;
                    yield return null;
                }
                MaintainTimeEndTrigger();
            }
        }

        void StopActiveMaintainTime()
        {
            if (maintainTimeCoroutine != null)
            {
                CoroutineManager.Instance.StopCoroutine(maintainTimeCoroutine);
            }
        }

        void MaintainTimeEndTrigger()
        {
            maintainTimeCoroutine = null;
            ForceDeactive(new EffectTriggerConditionInfo
            {
                owner = effectInstance.owner
            });
        }

        #endregion

        public void ResetActiveTime()
        {
            if (maintainTimeCoroutine != null)
            {
                StopActiveMaintainTime();
                MaintainTimeEndTrigger();
            }
        }

        public void ResetColdDownTime()
        {
            StopColdDownTime();
        }
    }
}
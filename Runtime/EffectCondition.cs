using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rayark.Mast;
using Coroutine = Rayark.Mast.Coroutine;
using MacacaGames.GameSystem;
using System.Linq;
using MacacaGames.EffectSystem.Model;
using System;

namespace MacacaGames.EffectSystem
{
    public class EffectCondition
    {

        public EffectBase effectInstance;

        EffectInfo effectInfo => effectInstance.info;

        float lastActiveTime = -1F;
        float lastActiveRound = -1;
        int currentAction = 0;
        int maintainActions = 0;
        int currentRound = 0;
        int maintainRounds = 0;
        bool isCountingMaintainTime = false;

        public bool isActive { get; private set; }

        public EffectCondition(EffectBase effectInstance)
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
                OnActive(new EffectSystem.EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });
            }
            // else
            // {
            //     OnCooldownTimeEnd();
            // }
        }

        public void End()
        {
            if (isActive == true)
            {
                OnDeactive(new EffectSystem.EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });
            }

            // StopActiveMaintainTime();
        }


        // Active / Deactive

        public void OnActive(EffectSystem.EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            if (IsOneOfRequirementsListFullfilled(info, effectInfo.activeRequirementLists, effectInfo) == false)
            {
                return;
            }

            //檢查機率觸發
            if (UnityEngine.Random.Range(0F, 100F) >= effectInfo.activeProbability && effectInfo.activeProbability != 0F)
            {
                //Debug.Log("Active 機率沒中！");
                return;
            }

            //檢查Cooldown
            if (CheckCooldownTime() == false)
            {
                return;
            }

            ForceActive(info);

        }

        void ForceActive(EffectSystem.EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;


            //Trans處理
            if (isActive == true)
            {
                switch (effectInfo.triggerTransType)
                {
                    case EffectSystemScriptable.TriggerTransType.CutOldOne:
                        ForceDeactive(new EffectSystem.EffectTriggerConditionInfo
                        {
                            owner = effectInstance.owner
                        });
                        break;

                    case EffectSystemScriptable.TriggerTransType.SkipNewOne:
                        return;
                }
            }

            isActive = true;

            effectInstance.OnActive(info);

            if (effectInfo.deactiveCondition == EffectSystemScriptable.DeactiveCondition.AfterActive)
            {
                ForceDeactive(new EffectSystem.EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });

                if (effectInfo.logic == EffectSystemScriptable.EffectInfoLogic.OnlyActiveOnce)
                {
                    effectInstance.RemoveEffect();
                }
            }
            else
            {
                //不會自毀的話才算時間
                StartActiveMaintainTime();
            }

            //記下冷卻開始時間
            lastActiveTime = GetCurrentTime();


        }

        public void OnDeactive(EffectSystem.EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            //只有Active時才能Deactive
            if (isActive == true)
            {
                if (IsOneOfRequirementsListFullfilled(info, effectInfo.deactiveRequirementLists, effectInfo) == false)
                {
                    return;
                }

                //檢查機率觸發
                if (effectInfo.deactiveProbability != 0F && UnityEngine.Random.Range(0F, 100F) >= effectInfo.deactiveProbability)
                {
                    Debug.Log("Dective 機率沒中！");
                    return;
                }

                ForceDeactive(info);
            }

        }

        public void ForceDeactive(EffectSystem.EffectTriggerConditionInfo info)
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            isActive = false;

            // StopActiveMaintainTime();
            StartCooldownTime();

            effectInstance.OnDeactive(info);

        }

        public bool IsOneOfRequirementsListFullfilled(EffectSystem.EffectTriggerConditionInfo info, List<List<ConditionRequirement>> requirementLists, EffectInfo targetInfo)
        {
            if (requirementLists == null)
            {
                Debug.Log($"{targetInfo.type}, active or deactive is null, what");
            }
            foreach (var requirementList in requirementLists)
            {
                if (IsRequirementsFullfilled(info, requirementList))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsRequirementsFullfilled(EffectSystem.EffectTriggerConditionInfo info, List<ConditionRequirement> requirements)
        {
            foreach (var requirement in requirements)
            {
                var effectable = requirement.isCheckOwner ? info.owner : info.target;
                var characterInstance = effectable;
                var sourceValue = Mathf.FloorToInt(characterInstance.GetRuntimeValue(requirement.inputType));

                bool IsRequirementFullfilled = false;
                switch (requirement.requirementLogic)
                {
                    case EffectSystemScriptable.ConditionLogic.None:
                        IsRequirementFullfilled = true;
                        break;
                    case EffectSystemScriptable.ConditionLogic.Greater:
                        IsRequirementFullfilled = sourceValue > requirement.conditionValue;
                        break;
                    case EffectSystemScriptable.ConditionLogic.GreaterEqual:
                        IsRequirementFullfilled = sourceValue >= requirement.conditionValue;
                        break;
                    case EffectSystemScriptable.ConditionLogic.Equal:
                        IsRequirementFullfilled = sourceValue == requirement.conditionValue;
                        break;
                    case EffectSystemScriptable.ConditionLogic.LessEqual:
                        IsRequirementFullfilled = sourceValue <= requirement.conditionValue;
                        break;
                    case EffectSystemScriptable.ConditionLogic.Less:
                        IsRequirementFullfilled = sourceValue < requirement.conditionValue;
                        break;
                    default:
                        IsRequirementFullfilled = false;
                        break;
                }

                if (IsRequirementFullfilled == false)
                {
                    return false;
                }
            }

            return true;
        }


        void OnCooldownTimeEnd()
        {
            if (effectInstance.RemoveSleepyEffect())
                return;

            effectInstance.OnCooldownEnd();
            if (effectInfo.logic == EffectSystemScriptable.EffectInfoLogic.ReactiveAfterCooldownEnd)
            {
                OnActive(new EffectSystem.EffectTriggerConditionInfo
                {
                    owner = effectInstance.owner
                });
            }
        }

        #region Cold Time         


        bool CheckCooldownTime()
        {
            if (lastActiveTime < 0)
                return true;

            if (effectInfo.cooldownTime < 0)
                return false;

            return GetCurrentTime() - lastActiveTime >= effectInfo.cooldownTime;
        }

        float GetCurrentTime()
        {
            if (GamePlayTime == null)
            {
                throw new Exception("Please assign GamePlayTime impl");
            }
            return GamePlayTime.Invoke();
            // return ApplicationController.Instance.GetGamePlayController().GetGamePlayData<MainGamePlayData>().gamePlayTime;
        }
        public static Func<float> GamePlayTime;


        Coroutine coldTimeCoroutine = null;
        void StartCooldownTime()
        {
            float t = effectInfo.cooldownTime;
            if (t > 0F)
            {
                coldTimeCoroutine = new Coroutine(CooldownTime(t));
                ApplicationController.Instance.GetGamePlayController().AddToUpdateExecuter(coldTimeCoroutine);
            }

            IEnumerator CooldownTime(float time)
            {
                yield return Coroutine.Sleep(time);
                CooldownTimeEndTrigger();
            }

            void CooldownTimeEndTrigger()
            {
                OnCooldownTimeEnd();
            }
        }

        // void StopCooldownTime()
        // {
        //     if (coldTimeCoroutine != null)
        //     {
        //         ApplicationController.Instance.GetGamePlayController().RemoveFromUpdateExecuter(coldTimeCoroutine);
        //     }
        // }

        #endregion

        #region Maintain Time

        Coroutine maintainCoroutine = null;
        void StartActiveMaintainTime()
        {
            if (isCountingMaintainTime == true)
            {
                return;
            }
            // float t = effectInfo.activeMaintainActions;

            // if (t > 0F)
            // {
            //     maintainCoroutine = new Coroutine(MaintainTime(t));
            //     ApplicationController.Instance.GetGamePlayController().AddToUpdateExecuter(maintainCoroutine);
            // }

            // IEnumerator MaintainTime(float time)
            // {
            //     yield return Coroutine.Sleep(time);
            //     MaintainTimeEndTrigger();
            // }
            isCountingMaintainTime = true;
            maintainActions = effectInfo.activeMaintainActions;
            maintainRounds = effectInfo.activeMaintainRounds;

        }


        public void UpdateMaintainAction()
        {
            if (maintainActions == 0) return;

            currentAction += 1;
            if (currentAction == maintainActions)
            {
                MaintainTimeEndTrigger();
            }
        }
        public void UpdateMaintainRound()
        {
            if (maintainRounds == 0) return;

            currentRound += 1;
            if (currentRound == maintainRounds)
            {
                MaintainTimeEndTrigger();
            }
        }

        // void StopActiveMaintainTime()
        // {
        //     if (maintainCoroutine != null)
        //     {
        //         ApplicationController.Instance.GetGamePlayController().RemoveFromUpdateExecuter(maintainCoroutine);
        //     }
        // }

        void MaintainTimeEndTrigger()
        {
            ForceDeactive(new EffectSystem.EffectTriggerConditionInfo
            {
                owner = effectInstance.owner
            });

            if (effectInfo.logic == EffectSystemScriptable.EffectInfoLogic.DestroyAfterMaintainTimeEnd)
            {
                effectInstance.RemoveEffect();
            }

            ResetMaintainTime();
        }


        #endregion

        public void ResetMaintainTime()
        {
            isCountingMaintainTime = false;
            currentAction = -1;
            currentRound = -1;

            // if (maintainCoroutine != null)
            // {
            //     StopActiveMaintainTime();
            //     MaintainTimeEndTrigger();
            // }
        }

        public void ResetCooldownTime()
        {
            lastActiveTime = -1;
            lastActiveRound = -1;

            // StopCooldownTime();
        }


    }
}
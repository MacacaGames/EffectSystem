﻿#define USE_LOG

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using MacacaGames.EffectSystem.Model;
using System.Reflection;
using System.Linq;

namespace MacacaGames.EffectSystem
{
    public class EffectInstanceBase
    {
        protected EffectSystem effectSystem => EffectSystem.Instance;
        public float input
        {
            get
            {
                return info.value;
            }
        }

        public EffectInfo info;
        public EffectCondition condition;
        public EffectSystem.EffectInstanceList effectList => effectSystem.GetEffectListByType(owner, info.type);

        public bool isPooled;
        public bool isActive => condition?.isActive ?? false;
        public bool isUsing { get; private set; } = false;

        public IEffectableObject owner;
        public object source;

        /// <summary>若Owner此類Effect效果值大於此設定值，將不會附加效果</summary>
        public virtual float maxEffectValue => float.PositiveInfinity;

        List<EffectViewBase> effectViewList = new List<EffectViewBase>();

        /// <summary>此EffectType所取得的數值上下限。</summary>
        public virtual (float min, float max) sumLimit => (min: 0F, max: float.PositiveInfinity);

        /// <summary>標記此Effect的Tag。</summary>
        public List<string> tags = new List<string> { };

        public virtual string GetEditorInfo()
        {
            return "";
        }

        public void Reset(EffectInfo effectInfo)
        {
            isUsing = false;
            isSleep = false;

            if (condition != null)
            {
                effectSystem.RemoveFromTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default, condition.maintainTimeTimer);
                effectSystem.RemoveFromTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default, condition.cooldownTimeTimer);
                condition = null;
            }
            owner = null;

            info = effectInfo;

            effectViewList.Clear();
            tags = effectInfo.tags != null ? effectInfo.tags : new List<string>();

            if (condition == null)
            {
                condition = new EffectCondition(this);
            }
        }

        public void Start()
        {
#if (USE_LOG)
            Debug.Log(GetStartEffectLog());
#endif

            isUsing = true;

            condition = new EffectCondition(this);

            effectSystem.RegistEffectTriggerCondition(this);
            AddEffectView(this);

            foreach (var effectView in effectViewList)
                effectView.OnStart();

            // Sould be Add before Start(), since in some case the effect may deactive immiditily after start, then the condition will be null
            effectSystem.AddToTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default, condition.maintainTimeTimer);
            effectSystem.AddToTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default, condition.cooldownTimeTimer);
            condition.Start();

            OnStart();
        }

        /// <summary>
        /// Excude when an Effect is attach
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Excude when an Effect is Active by ActiveCondition
        /// </summary>
        /// <param name="triggerConditionInfo"></param>
        public virtual void OnActive(EffectTriggerConditionInfo triggerConditionInfo)
        {
            modelsCache = triggerConditionInfo.models;
            InjectModels(this);
            ExecuteActive(triggerConditionInfo);
        }

        /// <summary>
        /// Excude when an Effect is Deactive DctiveCondition
        /// </summary>
        /// <param name="triggerConditionInfo"></param>
        public virtual void OnDeactive(EffectTriggerConditionInfo triggerConditionInfo)
        {
            modelsCache = triggerConditionInfo.models;
            InjectModels(this);
            ExecuteDeactive(triggerConditionInfo);
        }

        public void ExecuteActive(EffectTriggerConditionInfo triggerConditionInfo)
        {
            effectList.SetDirty(true);

            //觸發 IEffectableObject 的 Callback
            owner.OnEffectActive(info);

            foreach (var effectView in effectViewList)
                effectView.OnActive();

        }
        public void ExecuteDeactive(EffectTriggerConditionInfo triggerConditionInfo)
        {
            effectList.SetDirty(true);

            //觸發 IEffectableObject 的 Callback
            owner.OnEffectDeactive(info);

            foreach (var effectView in effectViewList)
                effectView.OnDeactive();


            //(Flag)Deactive時自動銷毀 >> 不再啟動
            if (info.logic == EffectLifeCycleLogic.OnlyActiveOnce)
            {
                SetSleep();
            }
        }

        public void End()
        {
            effectSystem.RemoveFromTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default, condition.maintainTimeTimer);
            effectSystem.RemoveFromTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default, condition.cooldownTimeTimer);

            if (isUsing == false)
            {

#if (UNITY_EDITOR)
                Debug.LogError($"[Effect Debug] 有一個還沒Start卻呼叫End的Effect\n Owner: {owner},\n Info: {info}");
                //Debug.Break();
#endif
            }
            else
            {
                effectSystem.UnregistEffectTriggerCondition(this);

                condition.End();
                condition = null;
                OnEnd();

                foreach (var effectView in effectViewList)
                    effectView.OnEnd();

                RemoveEffectView(this);
                isUsing = false;
            }


#if (USE_LOG)
            Debug.Log(GetEndEffectLog());
#endif

        }
        /// <summary>當Effect被消除時執行</summary>
        protected virtual void OnEnd() { }

        /// <summary>
        /// Excude when the colddown is finish
        /// </summary>
        public virtual void OnColdownEnd()
        {
            foreach (var effectView in effectViewList)
                effectView.OnColdDownEnd();
        }


        //Sleep就不會再被觸發、提前移除EffectView，等待特定Trigger觸發RemoveSleepyEffect來被移除
        bool isSleep { get; set; }
        void SetSleep()
        {
            isSleep = true;
            RemoveEffectView(this);
        }

        public bool RemoveSleepyEffect()
        {
            if (isSleep == true)
            {
                Debug.Log($"Recover Sleep Effect: {info}");
                RemoveEffect();
                return true;
            }
            return false;
        }

        public void RemoveEffect()
        {
            effectSystem.RemoveEffect(owner, this);
        }

        static void AddEffectView(EffectInstanceBase effect)
        {
            if (effect.info.viewInfos == null || effect.info.viewInfos.Count == 0)
                return;

            foreach (var viewInfo in effect.info.viewInfos)
            {
                if (viewInfo.prefab == null)
                    throw new Exception($"[EFFECT] {effect.GetType().Name}(Owner:{effect.owner})有未填入值的ViewInfo.prefab");
                EffectViewBase effectView = EffectSystem.Instance.RequestEffectView(effect.info, viewInfo, effect.owner);
                effectView.transform.rotation = Quaternion.identity;    //重設旋轉角度
                effect.effectViewList.Add(effectView);
            }
        }
        static void RemoveEffectView(EffectInstanceBase effect)
        {
            foreach (var effectView in effect.effectViewList)
            {
                EffectSystem.Instance.RecoveryEffectView(effectView);
                //UnityEngine.Object.Destroy(viewAnimator.gameObject);
            }
            effect.effectViewList.Clear();
        }

        public int GetValueInt()
        {
            return Mathf.FloorToInt(GetValue());
        }
        public float GetValue()
        {
            //Condition存在，且Condition成立則傳值，否則傳回0
            if (condition != null && condition.isActive == false) return 0F;

            return GetOriginValue();
        }
        public virtual float GetOriginValue()
        {

            return info.value;
        }

        protected virtual string GetStartEffectLog()
        {
            return $"[EFFECT] {owner} <color=#2043CF>＋</color><color=#2043CF>{GetType().Name}</color> [{input}]";
        }
        protected virtual string GetEndEffectLog()
        {
            return $"[EFFECT] {owner} <color=#CF2121>－</color><color=#CF2121>{GetType().Name}</color> [{input}]";
        }

        #region Inject
        protected static object[] modelsCache = null;

        internal void InjectModels(object targetObject)
        {
            Type contract = targetObject.GetType();

            IEnumerable<MemberInfo> members =
            contract.FindMembers(
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                (m, i) => m.GetCustomAttribute(typeof(EffectInstanceBaseInjectAttribute), true) != null,
                null);

            var groupedMember = members.GroupBy(m => m.GetMemberType());
            foreach (var gp in groupedMember)
            {
                var isMultiple = gp.Count() > 1;
                foreach (var info in gp)
                {
                    var target = GetModelInstance(info, isMultiple);
                    if (target != null)
                    {
                        info.SetValue(targetObject, target);
                    }
                }
            }
        }

        internal object GetModelInstance(MemberInfo memberInfo, bool isMultiple = false)
        {
            Type typeToSearch = memberInfo.GetMemberType();
            return SearchInModels(typeToSearch);
        }

        object SearchInModels(Type typeToSearch)
        {
            var models = modelsCache;

            // If type is string, also search from parameter
            if (typeToSearch == typeof(string))
            {
                var parameter = info.GetParameterByKey(typeToSearch.Name);
                if (!string.IsNullOrEmpty(parameter))
                {
                    return parameter;
                }
            }

            if (models == null || models.Length == 0)
            {
                return null;
            }

            try
            {
                return models.SingleOrDefault(model => model.GetType() == typeToSearch);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("When using EffectSystem model biding, each Type only available for one instance, if you would like to bind multiple instance of a Type use Collections(List, Array) instead.");
            }
        }
        #endregion

    }
}
#define USE_LOG

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using MacacaGames.EffectSystem.Model;
using System.Reflection;

namespace MacacaGames.EffectSystem
{
    public class EffectInstanceBase
    {
        protected EffectSystem effectSystem;

        public EffectInstanceBase(EffectSystem effectSystem)
        {
            this.effectSystem = effectSystem;
        }

        List<EffectViewBase> effectViewList = new List<EffectViewBase>();

        public float input
        {
            get { return info.value; }
        }

        public EffectInfo info;
        public EffectCondition condition;
        public EffectInstanceList effectList => effectSystem.GetEffectListByType(owner, info.type);

        public bool isPooled;
        public bool isActive => condition?.isActive ?? false;
        public bool isUsing { get; private set; } = false;

        public IEffectableObject owner;
        public object source;

        /// <summary>若Owner此類Effect效果值大於此設定值，將不會附加效果</summary>
        public virtual float maxEffectValue => float.PositiveInfinity;


        /// <summary>此EffectType所取得的數值上下限。</summary>
        public virtual (float min, float max) sumLimit => (min: 0F, max: float.PositiveInfinity);
        /// <summary>此EffectType所取得的層數上下限，若超過則會擠掉最舊的effect。</summary>
        public virtual int countLimit => int.MaxValue;
        
        /// <summary>maintainTime timer的ID，用來決定該由哪種timer來迭代冷卻。</summary>
        public virtual string maintainTimeTimerId => EffectSystemScriptableBuiltIn.TimerTickerId.Default;
        /// <summary>cooldown timer的ID，用來決定該由哪種timer來迭代冷卻。</summary>
        public virtual string cooldownTimeTimerId => EffectSystemScriptableBuiltIn.TimerTickerId.Default;

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
                effectSystem.RemoveFromTimerTicker(maintainTimeTimerId, condition.maintainTimeTimer);
                effectSystem.RemoveFromTimerTicker(cooldownTimeTimerId, condition.cooldownTimeTimer);
                condition = null;
            }

            owner = null;

            info = effectInfo;

            ClearViewList();
            tags = effectInfo.tags  != null ? effectInfo.tags.Take(effectInfo.tags.Count).ToList() : new List<string>();

            if (condition == null)
            {
                condition = new EffectCondition(this);
            }
        }

        public virtual void ClearViewList()
        {
            effectViewList.Clear();
        }

        public void Start()
        {
#if (USE_LOG)
            EffectInfoExtensions.Log(GetStartEffectLog());
#endif

            isUsing = true;

            condition = new EffectCondition(this);

            effectSystem.RegistEffectTriggerCondition(this);
            AddEffectView(this);

            EffectViewOnStart();

            // Sould be Add before Start(), since in some case the effect may deactive immiditily after start, then the condition will be null
            effectSystem.AddToTimerTicker(maintainTimeTimerId, condition.maintainTimeTimer);
            effectSystem.AddToTimerTicker(cooldownTimeTimerId, condition.cooldownTimeTimer);
            condition.Start();

            OnStart();
        }

        public virtual void EffectViewOnStart()
        {
            foreach (var effectView in effectViewList)
                effectView.OnStart();
        }

        /// <summary>
        /// Excude when an Effect is attach
        /// </summary>
        protected virtual void OnStart()
        {
            //觸發 IEffectableObject 的 Callback
            owner.OnEffectStart(info);
        }

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
        /// Excude when an Effect is Deactive by DeactiveCondition
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

            EffectViewOnActive();
        }

        public virtual void EffectViewOnActive()
        {
            foreach (var effectView in effectViewList)
                effectView.OnActive();
        }

        public void ExecuteDeactive(EffectTriggerConditionInfo triggerConditionInfo)
        {
            effectList.SetDirty(true);

            //觸發 IEffectableObject 的 Callback
            owner.OnEffectDeactive(info);

            EffectViewOnDeactive();


            //(Flag)Deactive時自動銷毀 >> 不再啟動
            if (info.logic == EffectLifeCycleLogic.OnlyActiveOnce 
                || info.logic == EffectLifeCycleLogic.ReactiveAfterCooldownEnd && condition.maintainTimeTimer.IsFinish)
            {
                SetSleep();
            }
        }

        public virtual void EffectViewOnDeactive()
        {
            foreach (var effectView in effectViewList)
                effectView.OnDeactive();
        }

        public void End()
        {
            effectSystem.RemoveFromTimerTicker(maintainTimeTimerId, condition.maintainTimeTimer);
            effectSystem.RemoveFromTimerTicker(cooldownTimeTimerId, condition.cooldownTimeTimer);

            if (isUsing == false)
            {
#if (UNITY_EDITOR)
                EffectInfoExtensions.LogError($"[Effect EffectInfoExtensions] 有一個還沒Start卻呼叫End的Effect\n Owner: {owner},\n Info: {info}");
                //EffectInfoExtensions.Break();
#endif
            }
            else
            {
                effectSystem.UnregistEffectTriggerCondition(this);

                condition.End();
                condition = null;
                OnEnd();

                EffectViewOnEnd();

                RemoveEffectView(this);
                isUsing = false;
            }


#if (USE_LOG)
            EffectInfoExtensions.Log(GetEndEffectLog());
#endif
        }

        public virtual void EffectViewOnEnd()
        {
            foreach (var effectView in effectViewList)
                effectView.OnEnd();
        }

        /// <summary>當Effect被消除時執行</summary>
        protected virtual void OnEnd()
        {
            //觸發 IEffectableObject 的 Callback
            owner.OnEffectEnd(info);
        }

        /// <summary>
        /// Excude when the cooldown is end
        /// </summary>
        public virtual void OnCooldownEnd()
        {
            EffectViewOnCooldownEnd();
        }

        public virtual void EffectViewOnCooldownEnd()
        {
            foreach (var effectView in effectViewList)
                effectView.OnCooldownEnd();
        }


        //Sleep就不會再被觸發、提前移除EffectView，等待特定Trigger觸發RemoveSleepyEffect來被移除
        public bool isSleep { get; private set; }

        void SetSleep()
        {
            isSleep = true;
            RemoveEffectView(this);
        }

        public bool RemoveSleepyEffect()
        {
            if (isSleep == true)
            {
                EffectInfoExtensions.Log($"Recover Sleep Effect: {info}");
                RemoveEffect();
                return true;
            }

            return false;
        }

        public void RemoveEffect()
        {
            effectSystem.RemoveEffect(owner, this);
        }

        public virtual void AddEffectView(EffectInstanceBase effect)
        {
#if !Server
            if (effect.info.viewInfos == null || effect.info.viewInfos.Count == 0)
                return;

            foreach (var viewInfo in effect.info.viewInfos)
            {
                if (viewInfo.prefab == null)
                    throw new Exception($"[EFFECT] {effect.GetType().Name}(Owner:{effect.owner})有未填入值的ViewInfo.prefab");
                EffectViewBase effectView =
                    EffectSystem.Instance.RequestEffectView(effect.info, viewInfo, effect.owner);
                effectView.transform.rotation = Quaternion.identity; //重設旋轉角度
                effectViewList.Add(effectView);
            }
#endif
        }

        public virtual void RemoveEffectView(EffectInstanceBase effect)
        {
#if !Server
            foreach (var effectView in effect.effectViewList)
            {
                effectSystem.RecoveryEffectView(effectView);
                //UnityEngine.Object.Destroy(viewAnimator.gameObject);
            }

            effect.effectViewList.Clear();
#endif
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
                    BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.Static,
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
                throw new InvalidOperationException(
                    "When using EffectSystem model biding, each Type only available for one instance, if you would like to bind multiple instance of a Type use Collections(List, Array) instead.");
            }
        }

        #endregion

        #region 修改 EffectInfo

        /// <summary>
        /// 修改 EffectInfo 的各種數值
        /// </summary>
        /// <param name="parameterName">example: nameof(info.value)</param>
        /// <param name="newValue"></param>
        public void SetEffectInfoValue(string parameterName, object newValue)
        {
            info = info.SetEffectInfoValue(parameterName, newValue);
        }

        #endregion
    }
}
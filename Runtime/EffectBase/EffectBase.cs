//#define USE_LOG

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    public abstract class EffectBase
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
        public EffectSystem.EffectList effectList => effectSystem.GetEffectListByType(owner, info.type);

        public bool isPooled;
        public bool isActive => condition?.isActive ?? false;
        public bool isUsing { get; private set; } = false;

        public IEffectableObject owner;

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

            condition = null;
            owner = null;

            info = effectInfo;

            effectViewList.Clear();
            tags = effectInfo.tags != null ? effectInfo.tags : new List<string>();

            if (condition == null)
            {
                condition = new EffectCondition(this);
            }

            ResetActiveTime();
            ResetColdDownTime();
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

            condition.Start();
            OnStart();

        }
        /// <summary>當Effect被附加時執行</summary>
        protected virtual void OnStart() { }

        public virtual void OnActive(EffectTriggerConditionInfo triggerConditionInfo)
        {
            ExecuteActive(triggerConditionInfo);
        }
        public virtual void OnDeactive(EffectTriggerConditionInfo triggerConditionInfo)
        {
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
            if (info.logic == EffectInfoLogic.OnlyActiveOnce)
            {
                SetSleep();
            }
        }

        public void End()
        {
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

        public virtual void OnCooldownEnd()
        {
            foreach (var effectView in effectViewList)
                effectView.OnColdDownEnd();
        }
        public void UpdateEffectCondition(float delta)
        {
            condition.Update(delta);
        }

        public void ResetActiveTime()
        {
            condition.ResetActiveTime();
        }

        public void ResetColdDownTime()
        {
            condition.ResetColdDownTime();
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

        static void AddEffectView(EffectBase effect)
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
        static void RemoveEffectView(EffectBase effect)
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

            return GetOriginValue() * owner.GetRuntimeValue(info.GetParameterByKey("inputType"));
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

    }
}
#define USE_POOL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    public class EffectSystem : MonoBehaviour
    {
        public static EffectSystem Instance;

        public Dictionary<string, TimerTicker> timerTickers = new Dictionary<string, TimerTicker>();
        void Awake()
        {
            Instance = this;
            if (effectViewPoolFolder == null)
            {
                effectViewPoolFolder = transform;
            }
            AddTimerTicker(EffectSystemScriptableBuiltIn.TimerTickerId.Default);
        }
        #region TimeManagement


        /// <summary>
        /// Add a timer Ticker to the system
        /// </summary>
        /// <param name="Id">The ticker's Id</param>  
        public void AddTimerTicker(string Id)
        {
            timerTickers.TryAdd(Id, new TimerTicker(Id));
        }

        /// <summary>
        /// Tick a TimerTicker by Id
        /// </summary>
        /// <param name="Id">The id of the TimerTicker</param>
        /// <param name="delta">The time delta value, should be positive</param>
        public void TickEffectTimer(string Id, float delta)
        {
            if (timerTickers.TryGetValue(Id, out TimerTicker timerTicker))
            {
                timerTicker.Tick(delta);
            }
        }

        public void AddToTimerTicker(string Id, IEffectTimer effectTimer)
        {
            if (timerTickers.TryGetValue(Id, out TimerTicker timerTicker))
            {
                timerTicker.AddTimer(effectTimer);
            }
            else
            {
                Debug.LogError($"No available timer with id: {Id}, is found, do nothing, you may need to check it out!");
            }
        }

        public void RemoveFromTimerTicker(string Id, IEffectTimer effectTimer)
        {
            if (timerTickers.TryGetValue(Id, out TimerTicker timerTicker))
            {
                timerTicker.RemoveTimer(effectTimer);
            }
            else
            {
                Debug.LogError($"No available timer with id: {Id}, is found, do nothing, you may need to check it out!");
            }
        }



        #endregion

        #region EffectDefine
        static Type effectInstanceBaseType = typeof(EffectInstanceBase);
        static Dictionary<string, Type> EffectTypeInstanceCache = new Dictionary<string, Type>();

        static Type QueryEffectTypeWithDefault(string effectType)
        {
            string typeFullName = $"Effect_{effectType}";
            Type result = effectInstanceBaseType;
            if (EffectTypeInstanceCache.TryGetValue(effectType, out result))
            {
                return result;
            }
            
            var type = Utility.GetType(typeFullName);
            if (type != null)
            {
                if (!type.IsSubclassOf(effectInstanceBaseType))
                {
                    Debug.LogError($"The EffectTypeInstance implements of EffectType: {effectType}, is not inherit from MacacaGames.EffectSystem.EffectInstanceBase please checked, system will automatically fallback to MacacaGames.EffectSystem.EffectInstanceBase");
                    return result;
                }
                EffectTypeInstanceCache.TryAdd(effectType, type);
                result = type;
                return result;
            }
            result = effectInstanceBaseType;
            return result;
        }

        #endregion


        #region ConditionDefine

        public Dictionary<string, Func<Action<EffectTriggerConditionInfo>, IEnumerator>> EffectConditionTriggerQuery = new Dictionary<string, Func<Action<EffectTriggerConditionInfo>, IEnumerator>>();    //給對應Trigger綁定Delegate的

        #endregion

        public static EffectCalculator calculator = new EffectCalculator();

        public class EffectInstanceList : ICollection<EffectInstanceBase>
        {
            private List<EffectInstanceBase> effects = new List<EffectInstanceBase>();

            public int Count => effects.Count;

            bool isDirty = false;

            public float sumLimitMax { get; private set; }
            public float sumLimitMin { get; private set; }

            public void Add(EffectInstanceBase effect)
            {
                effects.Add(effect);
                SetDirty(true);
            }
            public bool Remove(EffectInstanceBase effect)
            {
                bool result = effects.Remove(effect);
                SetDirty(true);
                return result;
            }

            public EffectInstanceList(string effectType)
            {
                var effect = calculator.GetLimit(effectType);

                sumLimitMin = effect.sumLimitMin;
                sumLimitMax = effect.sumLimitMax;

                sumValueCache = 0F;
            }

            float sumValueCache = 0F;
            public void SetDirty(bool b) => isDirty = b;

            public float GetSum()
            {
                return Mathf.Clamp(GetSumWithoutLimit(), sumLimitMin, sumLimitMax);
            }

            float GetSumWithoutLimit()
            {
                float result = 0F;
                if (isDirty == true)
                {
                    foreach (var effect in effects)
                        result += effect.GetValue();

                    sumValueCache = result;
                    SetDirty(false);
                }
                else
                {
                    result = sumValueCache;
                }
                return result;
            }

            /// <summary>判斷加總值是否超出上下限，若大於等於上限或小於下限視為超出。</summary>
            public bool isSumOutOfLimit
            {
                get
                {
                    float sum = GetSumWithoutLimit();
                    return sum >= sumLimitMax || sum < sumLimitMin;
                }
            }

            public bool IsReadOnly => false;

            public IEnumerator<EffectInstanceBase> GetEnumerator()
            {
                for (int i = 0; i < effects.Count; i++)
                    yield return effects[i];
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < effects.Count; i++)
                    yield return effects[i];
            }

            public void Clear()
            {
                effects.Clear();
            }

            public bool Contains(EffectInstanceBase item)
            {
                return effects.Contains(item);
            }

            public void CopyTo(EffectInstanceBase[] array, int arrayIndex)
            {
                effects.CopyTo(array, arrayIndex);
            }
        }

        public class EffectableObjectInfo
        {
            public Dictionary<string, EffectInstanceList> effectLists = new Dictionary<string, EffectInstanceList>();
            public Dictionary<string, Action<EffectTriggerConditionInfo>> activeCondition = new Dictionary<string, Action<EffectTriggerConditionInfo>>();
            public Dictionary<string, Action<EffectTriggerConditionInfo>> deactiveCondition = new Dictionary<string, Action<EffectTriggerConditionInfo>>();
        }


        ///<summary>所有Owner的每種Type的EffectList。</summary>
        Dictionary<IEffectableObject, EffectableObjectInfo> effectableObjectQuery = new Dictionary<IEffectableObject, EffectableObjectInfo>();

        public List<IEffectableObject> GetEffectableObjects()
        {
            if (effectableObjectQuery == null || effectableObjectQuery.Count == 0)
            {
                return null;
            }
            return effectableObjectQuery.Select(m => m.Key).ToList();
        }

        #region EffectableObjectInfo

        EffectableObjectInfo GetEffectableObjectInfo(IEffectableObject owner)
        {
            if (effectableObjectQuery.TryGetValue(owner, out EffectableObjectInfo result) == false)
            {
                result = new EffectableObjectInfo();
                effectableObjectQuery.Add(owner, result);
            }
            return result;
        }

        ///<summary>移除指定EffectableObject，結束他擁有的所有Effect、解除所有註冊的Condition。</summary>
        public void CleanEffectableObject(IEffectableObject owner)
        {
            var effectQuery = GetEffectList(owner);
            List<string> keyList = effectQuery.Keys.ToList();

            for (int i = 0; i < keyList.Count; i++)
                RemoveEffectsByType(owner, keyList[i]);

            effectableObjectQuery.Remove(owner);
        }

        #endregion




        Transform effectViewPoolFolder;
        public Dictionary<GameObject, Queue<EffectViewBase>> effectViewPool = new Dictionary<GameObject, Queue<EffectViewBase>>();

        public EffectViewBase RequestEffectView(EffectInfo info, EffectViewInfo viewInfo, IEffectableObject owner)
        {
            if (effectViewPool.TryGetValue(viewInfo.prefab, out var q) == false)
            {
                q = new Queue<EffectViewBase>();
                effectViewPool.Add(viewInfo.prefab, q);
            }

            EffectViewBase effectView = null;

            if (q.Count == 0)
            {
                GameObject instance = Instantiate(viewInfo.prefab);

                effectView = instance.GetComponent<EffectViewBase>();

                if (effectView != null)
                {
                    NormalizeTransform(effectView.transform, owner.GetEffectViewParent(viewInfo.viewRootType));
                    effectView.Init(info, viewInfo);
                }
                else
                {
                    throw new Exception("[EffectView] 在EffectView上找不到EffectViewBase。");
                }
            }
            else
            {
                effectView = q.Dequeue();
                NormalizeTransform(effectView.transform, owner.GetEffectViewParent(viewInfo.viewRootType));
            }


            return effectView;

            void NormalizeTransform(Transform t, Transform parent)
            {
                t.SetParent(parent);
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;
            }
        }

        public void RecoveryEffectView(EffectViewBase effectView)
        {
            var queryKey = effectView.viewInfo.prefab;

            effectViewPool[queryKey].Enqueue(effectView);
            effectView.transform.SetParent(effectViewPoolFolder);
        }

        #region Condition

        public void ResetEffectableObjectQuery()
        {
            effectableObjectQuery = new Dictionary<IEffectableObject, EffectableObjectInfo>();
        }

        /// <summary>
        /// Trigger a condition
        /// </summary>
        /// <param name="owner">The target object to apply the trigger</param>
        /// <param name="condition">The condition</param>
        /// <param name="info">The info of a Trigger</param>
        public void EffectTriggerCondition(string condition, IEffectableObject owner, EffectTriggerConditionInfo info)
        {
            if (effectableObjectQuery.TryGetValue(owner, out EffectableObjectInfo ownerInfo))
            {
                if (ownerInfo.activeCondition.TryGetValue(condition, out var activeAction))
                {
                    activeAction?.Invoke(info);
                }
                if (ownerInfo.deactiveCondition.TryGetValue(condition, out var deactiveAction))
                {
                    deactiveAction?.Invoke(info);
                }
            }
        }

        /// <summary>
        /// Trigger a condition
        /// </summary>
        /// <param name="owner">The target object to apply the trigger</param>
        /// <param name="condition">The condition</param>
        /// <param name="target">The target to apply a effect, for instance a effect trigger by A object by the effect should apply to B object as target</param>
        public void EffectTriggerCondition(string condition, IEffectableObject owner, IEffectableObject target = null)
        {
            EffectTriggerCondition(condition, owner, new EffectTriggerConditionInfo(owner, target));
        }

        public void RegistEffectTriggerCondition(EffectInstanceBase effect)
        {
            //ActiveCondition
            string activeCondition = effect.info.activeCondition;

            var activeConditionList = effectableObjectQuery[effect.owner].activeCondition;

            if (activeConditionList.TryGetValue(activeCondition, out var activeItem) == false)
            {
                activeConditionList.Add(activeCondition, null);
            }

            activeConditionList[activeCondition] += effect.condition.OnActive;

            //DeactiveCondition
            string deactiveCondition = effect.info.deactiveCondition;

            var deactiveConditionList = effectableObjectQuery[effect.owner].deactiveCondition;

            if (deactiveConditionList.TryGetValue(deactiveCondition, out var deactiveItem) == false)
            {
                deactiveConditionList.Add(deactiveCondition, null);
            }

            deactiveConditionList[deactiveCondition] += effect.condition.OnDeactive;
        }

        public void UnregistEffectTriggerCondition(EffectInstanceBase effect)
        {
            EffectableObjectInfo effectableObjectInfo = effectableObjectQuery[effect.owner];

            //ActiveCondition
            string activeCondition = effect.info.activeCondition;
            if (effectableObjectInfo.activeCondition.ContainsKey(activeCondition) == true)
            {
                effectableObjectInfo.activeCondition[activeCondition] -= effect.condition.OnActive;
            }
            else
            {
#if (UNITY_EDITOR)
                Debug.Log($"[Effect Debug] Owner: {effect.owner},\n Condition: {activeCondition},\n Info: {effect.info}");
                //Debug.Break();
#endif
            }


            //DeactiveCondition
            string deactiveCondition = effect.info.deactiveCondition;
            if (effectableObjectInfo.deactiveCondition.ContainsKey(deactiveCondition) == true)
            {
                effectableObjectInfo.deactiveCondition[deactiveCondition] -= effect.condition.OnDeactive;
            }
            else
            {
#if (UNITY_EDITOR)
                Debug.Log($"[Effect Debug] Owner: {effect.owner},\n Condition: {activeCondition},\n Info: {effect.info}");
                //Debug.Break();
#endif
            }
        }

        #endregion

        #region EffectPool

        //EffectPool <EffectSystemScriptable.EffectType, EffectBase>
        static Dictionary<string, Queue<EffectInstanceBase>> effectPoolQuery = new Dictionary<string, Queue<EffectInstanceBase>>();

        static EffectInstanceBase RequestEffect(string type)
        {
#if (USE_POOL)
            Queue<EffectInstanceBase> q = null;
            if (effectPoolQuery.TryGetValue(type, out Queue<EffectInstanceBase> qResult))
            {
                if (qResult == null)
                {
                    q = new Queue<EffectInstanceBase>();
                    effectPoolQuery[type] = q;
                }
                else
                {
                    q = qResult;
                }
            }
            else
            {
                q = new Queue<EffectInstanceBase>();
                effectPoolQuery.Add(type, q);
            }

            EffectInstanceBase effect = null;
            if (q.Count == 0)
            {
                //pool沒有東西的話就生出來，並標記為pooled
                effect = Activator.CreateInstance(QueryEffectTypeWithDefault(type)) as EffectInstanceBase;
                effect.isPooled = true;
            }
            else
            {
                effect = q.Dequeue();
                effect.owner = null;
            }

            return effect;
#else
        return CreateEffect(type);
#endif
        }

        public static EffectInstanceBase RequestEffect(EffectInfo info)
        {
#if (USE_POOL)
            EffectInstanceBase effect = RequestEffect(info.type);
            effect.Reset(info);

            //effect = SetLevel(effect, info.level);

            return effect;
#else
        return CreateEffect(info);
#endif
        }

        public static void RecoveryEffectBase(EffectInstanceBase effect, string type)
        {
#if (USE_POOL)

            if (effect.isUsing == true)
            {
                Debug.LogError($"嘗試回收一個尚未End的Effect : {effect.info}");
            }

            effectPoolQuery[type].Enqueue(effect);
#endif
        }

        //EffectCondition
        static Queue<EffectCondition> effectConditionPool = new Queue<EffectCondition>();

        #endregion

        #region 創造Effect實體

        ///<summary>創造一個Effect實體。</summary>
        static EffectInstanceBase CreateEffect(string type)
        {
            EffectInstanceBase effect;

            effect = Activator.CreateInstance(QueryEffectTypeWithDefault(type)) as EffectInstanceBase;

            effect.info = new EffectInfo
            {
                type = type
            };

            effect.isPooled = false;

            return effect;
        }

        ///<summary>創造一個Effect實體，且有指定的Info。</summary>
        public static EffectInstanceBase CreateEffect(EffectInfo info)
        {
            EffectInstanceBase effect = CreateEffect(info.type);
            effect.info = info;

            // effect = SetLevel(effect, info.inputBase);

            return effect;
        }

        // ///<summary>將已存在的Effect實體賦予指定的Level。</summary>
        // static EffectBase SetLevel(EffectBase effect, int level)
        // {
        //     effect.input = level;
        //     return effect;
        // }

        #endregion

        #region Effect效果說明與數值顯示
        public static string GetDefaultEffectsDescription(EffectGroup effectGroup)
        {
            return string.Join("\n", GetDefaultEffectsDescriptions(effectGroup.effects));
        }

        public static string GetDefaultEffectsDescription(IEnumerable<EffectInfo> infos)
        {
            return string.Join("\n", GetDefaultEffectsDescriptions(infos));
        }

        public static IEnumerable<string> GetDefaultEffectsDescriptions(IEnumerable<EffectInfo> infos)
        {
            return infos.Select(_ => GetDefaultEffectDescription(_));
        }

        /// <summary>
        /// 取得指定EffectInfo的說明。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetDefaultEffectDescription(EffectInfo info)
        {
            if (EffectDataProvider.GetEffectDescriptionString == null)
            {
                throw new Exception("Please use EffectDataProvider.SetEffectDescriptionStringDelegate to assign impl");
            }
            string str = EffectDataProvider.GetEffectDescriptionString?.Invoke(QueryEffectTypeWithDefault(info.type).Name);
            string result = GetCustomEffectDescription(str, info);

            result = ReplaceEffectAffix(result);

            ValidEffectDescription(result);

            return result;
        }

        public static string GetCustomEffectsDescription(string str, EffectGroup effectGroups)
        {
            return GetCustomEffectsDescription(str, effectGroups.effects);
        }

        public static string GetCustomEffectsDescription(string str, IEnumerable<EffectInfo> infos)
        {
            for (int i = 0; i < 50; i++)
            {
                foreach (EffectInfo info in infos)
                    str = GetCustomEffectDescription(str, info);

                if (str.Contains('{') == false)
                    break;
            }

            str = ReplaceEffectAffix(str);
            ValidEffectDescription(str);

            return str;
        }

        static string GetCustomEffectDescription(string str, EffectInfo info)
        {

            string param = GetTargetParam();

            //取代Effect數值
            if (string.IsNullOrEmpty(param) == false)
            {
                string v = GetEffectValueFormatString(param, GetEffectInfoHierarchyValue(param));
                str = str.Replace("{" + param + "}", v);
            }

            return str;

            string GetTargetParam()
            {
                if (string.IsNullOrEmpty(str))
                {
                    return "parameter error, please check the source str";
                }
                int begin = str.IndexOf('{');
                int end = str.IndexOf('}');

                string p;

                //若參數不存在，回傳空
                if (begin < 0)
                    return "";

                for (int i = 0; i < 50; i++)
                {
                    //取得參數內容
                    p = str.Substring(begin + 1, end - begin - 1);

                    string typeAndIdName = GetEffectInfoTypeIdString(info);

                    //若以type或Id開頭
                    if (p.Length >= typeAndIdName.Length && p.Substring(0, typeAndIdName.Length) == typeAndIdName)
                    {
                        //且Id後緊接連接字元，為了篩掉同樣開頭但不同長度的id也會被比對成功
                        if (p.Length >= typeAndIdName.Length + 1 && new[] { '.', '>' }.Contains(p[typeAndIdName.Length]))
                        {
                            return p;
                        }
                        //或是整段都是Id
                        else if (p == typeAndIdName)
                        {
                            return p;
                        }
                        //否則有格式化的整段Id
                        else if (p.Substring(0, p.IndexOf(':')) == typeAndIdName)
                        {
                            return p;
                        }
                    }

                    //否則找下一個參數
                    begin = str.IndexOf('{', begin + 1);
                    end = str.IndexOf('}', begin + 1);

                    //若下一個參數不存在，表示沒有符合的參數，回傳空
                    if (begin < 0)
                        return "";
                }

                throw new Exception("[EFFECT] 填入EffectDescription時，字串模板中有太多參數！很可能是字串模板填錯所導致");
            }

            //解析字串組，依照階層找到對應值
            float GetEffectInfoHierarchyValue(string paramValue)
            {
                if (paramValue.IndexOf(':') > 0)
                {
                    paramValue = paramValue.Substring(0, paramValue.IndexOf(':'));
                }

                if (paramValue.IndexOf('>') > 0)
                {
                    paramValue = paramValue.Replace(">", ".>.");
                }

                string[] path = paramValue.Split('.');

                List<EffectInfo> currentInfos = new List<EffectInfo> { info };
                EffectInfo currentInfo = info;

                foreach (var p in path)
                {
                    if (p.StartsWith("Effect_") || p.StartsWith("#"))
                    {
                        currentInfo = currentInfos.First(_ => GetEffectInfoTypeIdString(_) == p);
                    }
                    else if (p == "subInfos" || p == ">")
                    {
                        currentInfos = currentInfo.subInfos;
                    }
                    else if (p == "level" || p == "lv")
                    {
                        return currentInfo.value;
                    }
                    else if (p == "value" || p == "val")
                    {
                        return CreateEffect(currentInfo).GetOriginValue();
                    }
                    else if (p == "maintainTime" || p == "time")
                    {
                        return currentInfo.maintainTime;
                    }
                    else if (p == "cooldownTime" || p == "cd")
                    {
                        return currentInfo.cooldownTime;
                    }
                    else if (p == "activeProbability" || p == "activeProb")
                    {
                        return currentInfo.activeProbability;
                    }
                    else if (p == "deactiveProbability" || p == "deactiveProb")
                    {
                        return currentInfo.deactiveProbability;
                    }
                }

                //若沒有指定是哪一個值，則用value
                if (path.Last().StartsWith("Effect_") || path.Last().StartsWith("#"))
                {
                    return CreateEffect(currentInfo).GetOriginValue();
                }

                throw new Exception("[Effect] 在組裝EffectDescription時，沒有匹配的參數內容，或內容解析失敗。");
            }

            //處理{__:%}的部分
            string GetEffectValueFormatString(string paramValue, float effectValue)
            {
                int formatBegin = paramValue.IndexOf(':');
                if (formatBegin > 0)
                {
                    if (paramValue.Substring(formatBegin + 1, 1) == "%")
                    {
                        return (effectValue * 100F).ToString("#.#") + "%";
                    }
                }

                return effectValue.ToString();    //一般數值
            }
        }

        internal static string GetEffectInfoTypeIdString(EffectInfo info)
        {
            if (string.IsNullOrEmpty(info.id))
                return QueryEffectTypeWithDefault(info.type).Name;
            else
                return $"#{info.id}";
        }

        static string ReplaceEffectAffix(string des)
        {
            const string affixHead = "{Affix_";
            const string affixFoot = "}";

            for (int i = 0; i < 50; i++)
            {
                int headIndex = des.IndexOf(affixHead);
                if (headIndex >= 0)
                {
                    int footIndex = des.IndexOf(affixFoot, headIndex);
                    string param = des.Substring(headIndex, footIndex + affixFoot.Length - headIndex);
                    string paramInner = param.Substring(1, param.Length - 2);

                    if (EffectDataProvider.GetEffectDescriptionString == null)
                    {
                        throw new Exception("Please assign GetEffectDescriptionStr impl");
                    }
                    string affixString = EffectDataProvider.GetEffectDescriptionString?.Invoke(paramInner);

                    des = des.Replace(param, affixString);
                }
                else
                {
                    break;
                }
            }

            return des;
        }

        static void ValidEffectDescription(string str)
        {
            if (str.Contains("{"))
            {
                throw new Exception($"[Effect] 描述字串填入Effect數值時，仍有未被處理的內容：{str}");
            }
        }

        //MAGIC 正規表達式
        static Regex pattern = new Regex(@" *[0-9]+ *%*");
        public static string GetColorNumberDescription(string text, string colorHex)
        {
            if (colorHex[0] != '#')
                colorHex = $"#{colorHex}";

            string preffix = $"<color={colorHex}>";
            string suffix = "</color>";

            return pattern.Replace(text, delegate (Match m)
            {
                string replacement = $"{preffix}{m.Value}{suffix}";
                return replacement;
            });
        }
        public static string GetColorNumberDescription(string text, Color color)
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            return GetColorNumberDescription(text, colorHex);
        }
        #endregion
        #region EffectList相關

        ///<summary>取得指定Owner的EffectList。</summary>
        Dictionary<string, EffectInstanceList> GetEffectList(IEffectableObject owner)
        {
            return GetEffectableObjectInfo(owner).effectLists;
        }

        ///<summary>取得指定Owner的指定EffectType的EffectList。</summary>
        public EffectInstanceList GetEffectListByType(IEffectableObject owner, string effectType)
        {
            var effectQuery = GetEffectList(owner);

            //若Dictionary中沒有東西則新增EffectList
            if (!effectQuery.ContainsKey(effectType))
            {
                EffectInstanceList effectList = new EffectInstanceList(effectType);
                effectQuery.Add(effectType, effectList);
            }
            return effectQuery[effectType];
        }

        #endregion

        #region 取得Effect的Value總和

        /// <summary>
        /// Get the sum value of the EffectType on an IEffectableObject
        /// </summary>
        /// <param name="target">The target IEffectableObject</param>
        /// <param name="effectType">The EffectType</param>
        /// <returns></returns>
        public float GetEffectSum(IEffectableObject target, string effectType)
        {
            var effectQuery = GetEffectList(target);

            if (!effectQuery.ContainsKey(effectType)) return 0F;

            return effectQuery[effectType].GetSum();
        }

        ///<summary>取得複數指定EffectType的Value總和的總和。</summary>
        public float GetEffectsSum(IEffectableObject target, params string[] effectType)
        {
            float result = 0F;

            foreach (var key in effectType)
                result += GetEffectSum(target, key);

            return result;
        }

        #endregion

        #region 附加Effect
        /// <summary>
        /// Add one or more Effect(s) to an IEffectableObject
        /// will do the ApprovedAddEffect checking before the effect is added
        /// </summary>
        /// <param name="owner">The target obejct would like to add the Effect</param>
        /// <param name="effectGroup">The EffectGroup you would like to add the the owner</param>
        /// <param name="tags">Add the tags on the EffectInstance which is add on this requrest, it is very helpful to manage the Effect Instance, </param>
        public List<EffectInstanceBase> AddRequestedEffects(IEffectableObject owner, EffectGroup effectGroup, params string[] tags)
        {
            return AddRequestedEffects(owner, effectGroup.effects, tags);
        }

        /// <summary>
        /// Add one or more Effect(s) to an IEffectableObject
        /// will do the ApprovedAddEffect checking before the effect is added
        /// </summary>
        /// <param name="owner">The target obejct would like to add the Effect</param>
        /// <param name="effectInfos">The EffectInfos you would like to add the the owner</param>
        /// <param name="tags">Add the tags on the EffectInstance which is add on this requrest, it is very helpful to manage the Effect Instance, </param>
        public List<EffectInstanceBase> AddRequestedEffects(IEffectableObject owner, IEnumerable<EffectInfo> effectInfos, params string[] tags)
        {
            if (effectInfos == null)
            {
                Debug.LogError("[Effect] AddRequestedEffects, Effects為null");
                return null;
            }
            List<EffectInstanceBase> result = new List<EffectInstanceBase>();
            foreach (var effectStruct in effectInfos)
            {
                result.Add(AddRequestedEffect(owner, effectStruct, tags));
            }
            return result;
        }

        ///<summary>附加指定的Effect實體，從物件池拿。</summary>
        public EffectInstanceBase AddRequestedEffect(IEffectableObject owner, EffectInfo effectInfo, params string[] tags)
        {
            if (owner.IsAlive() == false)
                return null;

            if (owner.ApprovedAddEffect(effectInfo) == false)
                return null;

            EffectInstanceBase effect = RequestEffect(effectInfo);
            EffectInstanceList effectList = GetEffectListByType(owner, effectInfo.type);

            if (effectList.GetSum() < effect.maxEffectValue)
            {
                effect.owner = owner;
                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        AddTag(effect, tag);
                    }
                }

                effectList.Add(effect);
                effect.Start();
            }
            else
            {
                RecoveryEffectBase(effect, effectInfo.type);
            }

            return effect;
#if (UNITY_EDITOR)
            OnEffectChange?.Invoke();   //Callback
#endif
        }

        /// <summary>
        /// Add one or more Effect(s) to an IEffectableObject
        /// will do the ApprovedAddEffect checking before the effect is added
        /// Difference between AddRequestedEffect is this method always create new Instance
        /// </summary>
        /// <param name="owner">The target obejct would like to add the Effect</param>
        /// <param name="effectInfos">The EffectInfos you would like to add the the owner</param>
        /// <param name="tags">Add the tags on the EffectInstance which is add on this requrest, it is very helpful to manage the Effect Instance, </param>
        [Obsolete("Use the AddRequestedEffect instead, AddRequestedEffect will manage the Effect Instance by object pool, it is better for performance")]
        public List<EffectInstanceBase> AddEffects(IEffectableObject owner, IEnumerable<EffectInfo> effectInfos, params string[] tags)
        {
            List<EffectInstanceBase> result = new List<EffectInstanceBase>();
            foreach (var effectStruct in effectInfos)
            {
                result.Add(AddEffect(owner, effectStruct, tags));
            }
            return result;
        }

        /// <summary>
        /// Add one or more Effect(s) to an IEffectableObject
        /// will do the ApprovedAddEffect checking before the effect is added
        /// Difference between AddRequestedEffect is this method always create new Instance
        /// </summary>
        /// <param name="owner">The target obejct would like to add the Effect</param>
        /// <param name="effectInfos">The EffectInfos you would like to add the the owner</param>
        /// <param name="tags">Add the tags on the EffectInstance which is add on this requrest, it is very helpful to manage the Effect Instance, </param>
        [Obsolete("Use the AddRequestedEffect instead, AddRequestedEffect will manage the Effect Instance by object pool, it is better for performance")]
        public EffectInstanceBase AddEffect(IEffectableObject owner, EffectInfo effectInfo, params string[] tags)
        {
            if (owner.IsAlive() == false)
            {
                Debug.LogError("[Effect] Effect被附加在一個死體上");
                return null;
            }

            if (owner.ApprovedAddEffect(effectInfo) == false)
                return null;

            EffectInstanceBase effect = CreateEffect(effectInfo);
            EffectInstanceList effectList = GetEffectListByType(owner, effectInfo.type);

            if (effectList.GetSum() < effect.maxEffectValue)
            {
                effect.owner = owner;
                // SetLevel(effect, effectInfo.inputBase);

                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        AddTag(effect, tag);
                    }
                }

                effectList.Add(effect);
                effect.Start();
            }
            else
            {
                //Debug.Log($"{owner}:{effectInfo.type} 當前值已達上限({effectList.GetSum()}/{effect.maxEffectValue})");
            }

            return effect;
#if (UNITY_EDITOR)
            OnEffectChange?.Invoke();   //Callback
#endif
        }


        void AddTag(EffectInstanceBase effect, string tag)
        {
            if (string.IsNullOrEmpty(tag) == false)
                effect.tags.Add(tag);
        }
        #endregion

        #region 移除Effect

        /// <summary>
        /// Remove all EffectInstances on IEffectableObject by tags
        /// </summary>
        /// <param name="owner">The target IEffectableObject</param>
        /// <param name="tag">The tag on EffectInstance would like to remove</param>
        public void RemoveEffectByTag(IEffectableObject owner, string tag)
        {
            var effectQuery = GetEffectList(owner);
            foreach (var effectList in effectQuery.Values)
            {
                foreach (var effect in effectList.ToArray())
                {
                    if (effect.tags.Contains(tag))
                    {
                        RemoveEffect(owner, effect);
                    }
                }
            }
        }

        /// <summary>
        /// Remove all EffectInstances on IEffectableObject by EffectType
        /// </summary>
        /// <param name="owner">The target IEffectableObject</param>
        /// <param name="effectType">The EffectType would like to remove</param>
        public void RemoveEffectsByType(IEffectableObject owner, string effectType)
        {
            var effectQuery = GetEffectList(owner);
            if (effectQuery.TryGetValue(effectType, out EffectInstanceList effectList))
            {
                foreach (var item in effectList.ToArray())
                {
                    RemoveEffect(owner, item);
                }
                effectQuery.Remove(effectType);
            }
        }
        /// <summary>
        /// Remove specific count of EffectInstances on IEffectableObject by EffectType and Tag
        /// </summary>
        /// <param name="owner">The target IEffectableObject</param>
        /// <param name="effectType">The EffectType would like to remove</param>
        public void RemoveEffectsByTypeAndTag(IEffectableObject owner, string effectType, string tag, int count, bool isRemoveOldFirst = true)
        {
            int currentRemoveCount = 0;
;            var effectQuery = GetEffectList(owner);
            if (effectQuery.TryGetValue(effectType, out EffectInstanceList effectList))
            {
                var sortEffectList = isRemoveOldFirst? effectList.ToArray() : effectList.ToArray().Reverse();
                foreach (var effect in sortEffectList)
                {
                    if(effect.tags.Contains(tag) == false) continue;

                    RemoveEffect(owner, effect);
                    currentRemoveCount++;
                    if(currentRemoveCount >= count)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove  EffectInstances on IEffectableObject 
        /// </summary>
        /// <param name="owner">The target IEffectableObject</param>
        /// <param name="effect">The Effect Instance would like to remove</param>
        public void RemoveEffect(IEffectableObject owner, EffectInstanceBase effect)
        {
            var effectQuery = GetEffectList(owner);

            if (effectQuery.TryGetValue(effect.info.type, out EffectInstanceList effectList))
            {
                effect.End();
                effectList.Remove(effect);

                if (effect.isPooled)
                {
                    RecoveryEffectBase(effect, effect.info.type);
                }
            }
            else
            {
                Debug.LogError($"Owner查無此Type的EffectList。\nowner: {owner}, type: {effect.info.type}, info: {effect.info}");
            }


#if (UNITY_EDITOR)
            OnEffectChange?.Invoke();   //Callback
#endif
        }

        #endregion

        #region 查詢Effect
        /// <summary>
        /// Get Effect Instance by tag
        /// </summary>
        /// <param name="owner">The target IEffectableObject</param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public List<EffectInstanceBase> GetEffectsByTag(IEffectableObject owner, string tag)
        {
            List<EffectInstanceBase> effects = new List<EffectInstanceBase>();
            var effectQuery = GetEffectList(owner);
            foreach (var effectList in effectQuery.Values)
            {
                foreach (var effect in effectList.ToArray())
                {
                    if (effect.tags.Contains(tag))
                    {
                        effects.Add(effect);
                    }
                }
            }

            return effects;
        }
        /// <summary>
        /// Get Effect Instance by type
        /// </summary>
        /// <param name="owner">The target IEffectableObject</param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public List<EffectInstanceBase> GetEffectsByType(IEffectableObject owner, string type, bool onlyGetActive = true)
        {
            List<EffectInstanceBase> effects = new List<EffectInstanceBase>();
            var effectQuery = GetEffectList(owner);
            foreach (var effectList in effectQuery.Values)
            {
                foreach (var effect in effectList.ToArray())
                {
                    if (effect.info.type == type)
                    {
                        if(onlyGetActive && effect.isActive == false) continue;
                        effects.Add(effect);
                    }
                }
            }

            return effects;
        }
        #endregion



#if (UNITY_EDITOR)

        //Editor階段可以在外部取得EffectQuery
        public Dictionary<string, EffectInstanceList> GetEffectQuery(IEffectableObject owner)
        {
            if (owner == null)
                return null;

            if (effectableObjectQuery.TryGetValue(owner, out var result) == true)
            {
                return result.effectLists;
            }
            else
            {
                return null;
            }
        }



        //Effect變動時的Callback
        public Action OnEffectChange;

#endif

    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EffectTypeGroupAttribute : Attribute
    {
        public EffectTypeGroupAttribute(string group)
        {
            this.group = group;
        }
        public string group;
    }
}
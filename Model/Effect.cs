using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Reflection;
using System.Linq;

namespace MacacaGames.EffectSystem.Model
{

    [MessagePack.MessagePackObject(true)]
    [Serializable]
    public partial struct EffectInfo
    {
        public string id;
        public string type;
        public EffectTaxonomy taxonomy;
        public float value;
        public string activeCondition;
        public List<string> activeRequirement;
        public string deactiveCondition;
        public List<string> deactiveRequirement;
        public TriggerTransType triggerTransType;
        public float activeProbability; // Active、Deacitve機率，0-100
        public float deactiveProbability;
        public float maintainTime;
        public int cooldownTime;
        public EffectLifeCycleLogic logic;
        public string colliderType;
        public List<string> subInfoIds;
        public Dictionary<string, string> parameters;
        public List<string> viewInfoIds;
        public List<string> tags;
        public string GetParameterByKey(string key)
        {
            string result = null;
            if (parameters.TryGetValue(key, out result))
            {
                Console.WriteLine($"parameter with key cannot be found in effect id: {id}");
            }
            return result;
        }
        public void SetParameterByKey(string key, string value)
        {
            if (parameters.ContainsKey(key))
            {
                parameters[key] = value;
            }
            else
            {
                parameters.Add(key, value);
            }
        }
        public bool HasError()
        {

            // TODO why
            // (int rangeMin, int rangeMax) = (100, 10000);
            // if (activeCondition == deactiveCondition &&
            //     activeCondition > rangeMin && activeCondition < rangeMax)
            //     return true;

            if (value == 0)
                return true;

            return false;
        }
        public string GetErrorInfo()
        {
            // TODO why
            // (int rangeMin, int rangeMax) = (100, 10000);
            // if (activeCondition == deactiveCondition &&
            //     activeCondition > rangeMin && (activeCondition < rangeMax)
            // {
            //     return "相同的Condition條件屬於未定義的行為，在定義前不應使用。";
            // }

            if (value == 0)
            {
                return "Effect value should not be 0 or less";
            }

            return "(無錯誤訊息)";
        }

        public EffectInfo GetTypeDomain()
        {
            return new EffectInfo
            {
                type = type,
                taxonomy = EffectTaxonomy.Domain,
            };
        }

        public EffectInfo GetTypeKingdom()
        {
            return new EffectInfo
            {
                type = type,
                taxonomy = EffectTaxonomy.Kingdom,
                activeCondition = activeCondition,
                triggerTransType = triggerTransType,
                deactiveCondition = deactiveCondition,
                cooldownTime = cooldownTime,
                maintainTime = maintainTime,
            };
        }

        static FieldInfo[] fields = null;
        public override string ToString()
        {
            string result = "";
            result += "EffectInfo {\n";

            string tab = "    ";

            if (fields == null)
                fields = GetType().GetFields();

            foreach (var item in fields)
            {
                result += $"{tab}{item.Name} = {item.GetValue(this)},\n";
            }

            result += "}";

            return result;
        }


        public EffectInfo GetCopy()
        {
            EffectInfo result = this;
            // // result.subInfos = subInfos.Select(_ => _.GetCopy()).ToList();
            // result.subInfoIds = subInfoIds;
            // result._subInfos = _subInfos.Select(_ => _.GetCopy()).ToList();
            // result.parameters = parameters;
            // result.viewInfoIds = viewInfoIds;
            // result._viewInfos = _viewInfos;
            //有Array時都要新增

            return result;
        }
    }

    [MessagePack.MessagePackObject(true)]
    [Serializable]
    public struct ConditionRequirement
    {
        public string id;
        public string conditionParameter;
        public ConditionLogic requirementLogic;
        public bool isCheckOwner;
        public int conditionValue;

        public bool IsRequirementsFullfilled(EffectTriggerConditionInfo info)
        {
            var effectable = isCheckOwner ? info.owner : info.target;
            var sourceValue = (int)System.MathF.Floor(effectable.GetRuntimeValue(conditionParameter));
            bool IsRequirementFullfilled = true;
            switch (requirementLogic)
            {
                case ConditionLogic.None:
                    IsRequirementFullfilled = true;
                    break;
                case ConditionLogic.Greater:
                    IsRequirementFullfilled = sourceValue > conditionValue;
                    break;
                case ConditionLogic.GreaterEqual:
                    IsRequirementFullfilled = sourceValue >= conditionValue;
                    break;
                case ConditionLogic.Equal:
                    IsRequirementFullfilled = sourceValue == conditionValue;
                    break;
                case ConditionLogic.LessEqual:
                    IsRequirementFullfilled = sourceValue <= conditionValue;
                    break;
                case ConditionLogic.Less:
                    IsRequirementFullfilled = sourceValue < conditionValue;
                    break;
                default:
                    IsRequirementFullfilled = false;
                    break;
            }
            return IsRequirementFullfilled;
        }
    }

    /// <summary>
    /// The data context for doing a trigger active
    /// </summary>
    public struct EffectTriggerConditionInfo
    {
        public IEffectableObject owner;     //Effect的Owner
        public IEffectableObject target     //Effect的第一個Target
        {
            get
            {
                if (targets == null || targets.Count == 0)
                {
                    return null;
                }
                return targets[0];
            }
        }
        public List<IEffectableObject> targets;   //Effect的Targets

        // function to cast owner to type T
        public T GetOwner<T>() where T : class
        {
            if(owner == null)
            {
                throw new Exception("EffectTriggerConditionInfo GetOwner but no owner");
            } 

            var result = owner as T;
            if(result == null)
            {
                throw new Exception("EffectTriggerConditionInfo GetOwner but cast failed");
            }

            return result;
        }
        // function to cast targets to type T
        public List<T> GetTargets<T>() where T : class
        {
            if (targets == null || targets.Count == 0)
            {
                throw new Exception("EffectTriggerConditionInfo GetTargets but no targets");
            }


            var list = new List<T>();
            try
            {
                list = targets.Select(x => x as T).ToList();
            }
            catch
            {
                throw new Exception("EffectTriggerConditionInfo GetTargets but cast failed");
            }

            return list;
        }
        public EffectTriggerConditionInfo(IEffectableObject owner, IEffectableObject target = null, IEffectableObject effectableObject = null)
        {
            this.owner = owner;
            if (target == null)
            {
                this.targets = new List<IEffectableObject>();
            }
            else
            {
                this.targets = new List<IEffectableObject>() { target };
            }
        }
        public EffectTriggerConditionInfo(IEffectableObject owner, List<IEffectableObject> targets = null)
        {
            this.owner = owner;
            this.targets = targets;
        }
    }
}
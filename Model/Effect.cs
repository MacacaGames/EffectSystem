using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Reflection;
using System.Linq;
#if !Server
using UnityEngine;
#endif
namespace MacacaGames.EffectSystem.Model
{

    // [Serializable]
    [MessagePack.MessagePackObject(true)]
    public partial struct EffectInfo
    {
        public string id;
        public enum EffectTaxonomy { Info = 0, Domain = 101, Kingdom = 102, }
        public string GetTitle()
        {
            return this.type.ToString();
        }
        public string type;
        public string inputType;
        public EffectTaxonomy taxonomy;
        public float inputBase;
        public string activeCondition;
        public List<List<ConditionRequirement>> activeRequirementLists;
        public string deactiveCondition;
        public List<List<ConditionRequirement>> deactiveRequirementLists;
        public string triggerTransType;
        public float activeProbability; // Active、Deacitve機率，0-100
        public float deactiveProbability;
        public int activeMaintainActions;
        public int activeMaintainRounds;
        public int cooldownTime;
        public string logic;
        public string colliderType;
        public List<string> subInfoIds;
        public List<int> parameters;
        public List<string> viewInfoIds;
        public List<string> tags;
        public bool HasError()
        {

            // TODO why
            // (int rangeMin, int rangeMax) = (100, 10000);
            // if (activeCondition == deactiveCondition &&
            //     activeCondition > rangeMin && activeCondition < rangeMax)
            //     return true;

            if (inputBase == 0)
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

            if (inputBase == 0)
            {
                return "Effect的Level不該等於0。";
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
                activeMaintainActions = activeMaintainActions,
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

        // public void DoEachSubInfos(Action<EffectInfo> act)
        // {
        //     act.Invoke(this);
        //     foreach (var subInfo in subInfos)
        //     {
        //         subInfo.DoEachSubInfos(act);
        //     }
        // }
        
    }


    // [Serializable]
    public struct EffectViewInfo
    {
        [HideInInspector]
        public string id;
        [HorizontalGroup("ViewInfo"), HideLabel]
        public string viewRootType;
        [HorizontalGroup("ViewInfo"), HideLabel]
        [HideInInspector]
        public string prefabAddress;
        
        [HorizontalGroup("ViewInfo"), HideLabel, Newtonsoft.Json.JsonIgnore, MessagePack.IgnoreMember]
        public GameObject prefab;
        [MessagePack.IgnoreMember]
        public static Func<string, GameObject> GetPrefab;
        public string prefabName;
    }
    // [Serializable] 
    [MessagePack.MessagePackObject(true)]
    public struct ConditionRequirement
    {
        public string inputType;
        public string requirementLogic;
        public bool isCheckOwner;
        public int conditionValue;

    }
   
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MacacaGames.EffectSystem.Model
{
    public partial struct EffectInfo
    {
        [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
        List<EffectInfo> _subInfos;
        [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
        public List<EffectInfo> subInfos
        {
            get
            {
                if (_subInfos == null)
                {
                    if (EffectDataProvider.GetEffectInfo == null)
                    {
                        throw new Exception("Please use EffectDataProvider.SetEffectInfoDelegate to assign the impl");
                    }
                    _subInfos = EffectDataProvider.GetEffectInfo?.Invoke(subInfoIds);
                }
                return _subInfos;
            }
        }

        [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
        List<EffectViewInfo> _viewInfos;
        [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
        public List<EffectViewInfo> viewInfos
        {
            get
            {
                if (_viewInfos == null)
                {
                    if (EffectDataProvider.GetEffectViewInfo == null)
                    {
                        throw new Exception("Please use EffectDataProvider.SeEffectViewInfoDelegate to assign the impl");
                    }
                    _viewInfos = EffectDataProvider.GetEffectViewInfo?.Invoke(viewInfoIds);
                }
                return _viewInfos;
            }
        }

        public static Func<List<string>, List<ConditionRequirement>> GetActiveRequirementLists;

        [MessagePack.IgnoreMember]
        public List<ConditionRequirement> activeRequirementLists
        {
            get
            {
                if (GetActiveRequirementLists == null)
                {
                    Console.WriteLine("No registe the GetActiveRequirementLists");
                }
                return GetActiveRequirementLists?.Invoke(activeRequirement); ;
            }
        }

        public static Func<List<string>, List<ConditionRequirement>> GetDeactiveRequirementLists;
        [MessagePack.IgnoreMember]
        public List<ConditionRequirement> deactiveRequirementLists
        {
            get
            {
                if (GetDeactiveRequirementLists == null)
                {
                    Console.WriteLine("No registe the GetActiveRequirementLists");
                }
                return GetDeactiveRequirementLists?.Invoke(deactiveRequirement); ;
            }
        }
    }
    public struct EffectViewInfo
    {
        [HideInInspector]
        public string id;
        public string viewRootType;
        [HideInInspector]
        public string prefabAddress;

        [Newtonsoft.Json.JsonIgnore, MessagePack.IgnoreMember]
        public GameObject prefab;
        [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
        public static Func<string, GameObject> GetPrefab;
        public string prefabName;
    }


}
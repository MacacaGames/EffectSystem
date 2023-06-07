using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MacacaGames.EffectSystem.Model
{
    public partial struct EffectInfo
    {
        /// <summary>
        /// 需要依賴外部回傳值，server、client皆須註冊。
        /// </summary>
        public static Func<List<string>, List<EffectViewInfo>> getEffectViewInfo;
        [MessagePack.IgnoreMember]
        List<EffectInfo> _subInfos;
        [MessagePack.IgnoreMember]
        public List<EffectInfo> subInfos
        {
            get
            {
                return _subInfos != null ? _subInfos : GetSubInfo();
            }
        }

        [MessagePack.IgnoreMember]
        List<EffectViewInfo> _viewInfos;
        [MessagePack.IgnoreMember]
        public List<EffectViewInfo> viewInfos
        {
            get
            {
                if (_viewInfos == null)
                {
                    if (getEffectViewInfo == null)
                    {
                        throw new Exception("Please assign allEffect impl");
                    }
                    _viewInfos = getEffectViewInfo?.Invoke(viewInfoIds);
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


        public List<EffectInfo> GetSubInfo()
        {
            if (getEffectInfo == null)
            {
                throw new Exception("Please assign allEffect impl");
            }

            List<EffectInfo> result = new List<EffectInfo>();
            if (subInfoIds != null && subInfoIds.Count > 0)
            {
                foreach (string effectid in subInfoIds)
                {
                    EffectInfo? effect = getEffectInfo?.Invoke(effectid);
                    if (effect != null)
                    {
                        result.Add((EffectInfo)effect);
                    }
                }
            }
            _subInfos = result;
            return _subInfos;
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
        [MessagePack.IgnoreMember]
        public static Func<string, GameObject> GetPrefab;
        public string prefabName;
    }


}
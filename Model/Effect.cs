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
        public ColliderType colliderType;
        public List<String> subInfoIds;
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
    public enum ColliderType
    {
        None = 0,
        Explosion_Fire = 1,
        Explosion_Frozen = 2,
        Explosion_Poison = 3,
        Explosion_Storm = 4,
        Zone_Fire = 5,
        Zone_Frozen = 6,
        Zone_Poison = 7,
        Zone_Storm = 8,
        Projectile_Fire = 9,
        Projectile_Forzen = 10,
        Projectile_Poison = 11,
        Projectile_Storm = 12,
        Around_Fire = 13,
        Around_Frozen = 14,
        Around_Poison = 15,
        Around_Storm = 16,
        Projectile_Bat = 17,
        Side_Pumpkin = 18,
        Projectile_Assault_Boss = 19,
        Projectile_MagicBall_Boss = 20,
        Projectile_Spike_Boss = 21,
        BigExplosion_Fire = 22,
        BigExplosion_Frozen = 23,
        BigExplosion_Poison = 24,
        BigExplosion_Storm = 25,
        BounceProjectile_Fire = 26,
        BounceProjectile_Forzen = 27,
        BounceProjectile_Poison = 28,
        BounceProjectile_Storm = 29,
        BounceProjectile_C03_Damage = 30,
        BounceProjectile_C03_Heal = 31,
        Zone_C04_Blade = 32,

        FireHit_Circle_M = 9001,
        Frozen_Circle_M = 9002,
        Storm_Circle_M = 9003,
        WeakAtk_Circle_M = 9004,
        Frozen_Wave_M = 9005,
        Poison_Wave_M = 9006,
        Storm_Wave_M = 9007,
        Bat_WaveL_S = 9008,
        Bat_WaveR_S = 9009,
        Pumpkin_Side_S = 9010,
        MagicBall_Wave_S_Boss = 9011,
        MagicBall_Wave_L_Boss = 9012,
        Spike_Wave_M_Boss = 9013,
        Assault_Wave_M_Boss = 9014,
        FrozenZone_Circle_L = 9015,
    }
}
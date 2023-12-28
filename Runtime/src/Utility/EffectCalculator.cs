using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    public class EffectCalculator
    {
        private EffectSystem effectSystem;
        public EffectCalculator(EffectSystem EffectSystem)
        {
            this.effectSystem = EffectSystem;
        }
        /// <summary>取得指定EffectType的上下限。</summary>
        public (float sumLimitMin, float sumLimitMax) GetLimit(string effectType)
        {
            var effect = effectSystem.CreateEffect(new EffectInfo { type = effectType, value = 1 });
            return effect.sumLimit;
        }


        /// <summary>加總合併同Kindom的Effect值。</summary>
        public Dictionary<EffectInfo, float> GetEffectTypeKingdomValues(params IEnumerable<EffectInfo>[] effectInfoGroups)
        {
            List<EffectInfo> effectInfos = effectInfoGroups.SelectMany(_ => _).ToList();

            Dictionary<EffectInfo, float> typeGroup = new Dictionary<EffectInfo, float>();

            foreach (var info in effectInfos)
            {
                int typeKingdomHash = info.GetTypeKingdom().GetHashCode();
                if (typeGroup.Keys.Select(_ => _.GetTypeKingdom().GetHashCode()).Contains(typeKingdomHash) == false)
                {
                    typeGroup.Add(info.GetTypeKingdom(), 0);
                }

                var key = typeGroup.Keys.Single(_ => _.GetTypeKingdom().GetHashCode() == typeKingdomHash);
                typeGroup[key] += effectSystem.CreateEffect(info).GetValue();
            }

            return typeGroup;
        }

        /// <summary>加總合併同Kindom的Effect值，用EffectList加總。</summary>
        public Dictionary<EffectInfo, EffectInstanceList> GetEffectTypeKingdomValues_EffectList(params IEnumerable<EffectInfo>[] effectInfoGroups)
        {
            List<EffectInfo> effectInfos = effectInfoGroups.SelectMany(_ => _).ToList();

            Dictionary<EffectInfo, EffectInstanceList> typeGroup = new Dictionary<EffectInfo, EffectInstanceList>();

            foreach (var info in effectInfos)
            {
                int typeKingdomHash = info.GetTypeKingdom().GetHashCode();
                if (typeGroup.Keys.Select(_ => _.GetTypeKingdom().GetHashCode()).Contains(typeKingdomHash) == false)
                {
                    typeGroup.Add(info.GetTypeKingdom(), new EffectInstanceList(info.type,effectSystem));
                }

                var key = typeGroup.Keys.Single(_ => _.GetTypeKingdom().GetHashCode() == typeKingdomHash);
                typeGroup[key].Add(effectSystem.CreateEffect(info));
            }

            return typeGroup;
        }
    }
}
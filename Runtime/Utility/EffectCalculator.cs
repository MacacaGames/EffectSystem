using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    public class EffectCalculator
    {
        /// <summary>取得指定EffectType的上下限。</summary>
        public (float sumLimitMin, float sumLimitMax) GetLimit(string effectType)
        {
            var effect = EffectSystem.CreateEffect(new EffectInfo { type = effectType, value = 1 });
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
                typeGroup[key] += EffectSystem.CreateEffect(info).GetValue();
            }

            return typeGroup;
        }

        /// <summary>加總合併同Kindom的Effect值，用EffectList加總。</summary>
        public Dictionary<EffectInfo, EffectSystem.EffectInstanceList> GetEffectTypeKingdomValues_EffectList(params IEnumerable<EffectInfo>[] effectInfoGroups)
        {
            List<EffectInfo> effectInfos = effectInfoGroups.SelectMany(_ => _).ToList();

            Dictionary<EffectInfo, EffectSystem.EffectInstanceList> typeGroup = new Dictionary<EffectInfo, EffectSystem.EffectInstanceList>();

            foreach (var info in effectInfos)
            {
                int typeKingdomHash = info.GetTypeKingdom().GetHashCode();
                if (typeGroup.Keys.Select(_ => _.GetTypeKingdom().GetHashCode()).Contains(typeKingdomHash) == false)
                {
                    typeGroup.Add(info.GetTypeKingdom(), new EffectSystem.EffectInstanceList(info.type));
                }

                var key = typeGroup.Keys.Single(_ => _.GetTypeKingdom().GetHashCode() == typeKingdomHash);
                typeGroup[key].Add(EffectSystem.CreateEffect(info));
            }

            return typeGroup;
        }
    }
}
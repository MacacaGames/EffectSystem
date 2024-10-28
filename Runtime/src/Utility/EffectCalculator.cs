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
        /// <summary>取得指定EffectType的總值上下限。</summary>
        public (float sumLimitMin, float sumLimitMax) GetSumLimit(string effectType)
        {
            var effect = effectSystem.CreateEffect(new EffectInfo { type = effectType, value = 1 });
            return effect.sumLimit;
        }
        /// <summary>取得指定EffectType的層數上下限。</summary>
        public int GetCountLimit(string effectType)
        {
            var effect = effectSystem.CreateEffect(new EffectInfo { type = effectType, value = 1 });
            return effect.countLimit;
        }
    }
}
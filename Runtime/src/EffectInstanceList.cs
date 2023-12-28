

using System.Collections;
using System.Collections.Generic;
using MacacaGames.EffectSystem;
using UnityEngine;

public class EffectInstanceList : ICollection<EffectInstanceBase>
{
    private EffectSystem effectSystem;
    
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

        public EffectInstanceList(string effectType, EffectSystem effectSystem)
        {
            this.effectSystem = effectSystem;
            var effect = effectSystem.calculator.GetLimit(effectType);

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

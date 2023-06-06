using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public interface IEffectableObject
    {
        void DestoryEffectableObject();

        Transform GetEffectViewParent(string viewRoot);

        bool ApprovedAddEffect(EffectInfo info);

        void OnEffectActive(EffectInfo info);

        void OnEffectDeactive(EffectInfo info);

        bool IsAlive();
        
        /// <summary>
        /// Due to the real runtime value is maintain by the IffectableObject, so you should use this to get the acctual value
        /// e.g. Current_ATK = ATK_Constant * ATK_Ratio
        /// So only using EffectSystem.GetEffectSum() is not enough
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        float GetRuntimeValue(string inputType);

    }
}
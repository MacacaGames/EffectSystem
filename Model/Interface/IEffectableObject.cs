using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public interface IEffectableObject
    {
        void DestoryEffectableObject();

        Transform GetEffectViewParent(string viewRoot);

        /// <summary>
        /// Detect if a Effect can be request or not
        /// </summary>
        /// <param name="info"></param>
        /// <returns>If false, then system automatically reject a add effect request</returns>
        bool ApprovedAddEffect(EffectInfo info);

        void OnEffectActive(EffectInfo info);

        void OnEffectDeactive(EffectInfo info);

        bool IsAlive();

        /// <summary>
        /// Due to the real runtime value is maintain by the IffectableObject, so you should use this to get the acctual value
        /// e.g. Current_ATK = ATK_Constant * ATK_Ratio
        /// So only using EffectSystem.GetEffectSum() is not enough
        /// </summary>
        /// <param name="parameterKey"></param>
        /// <returns></returns>
        float GetRuntimeValue(string parameterKey);

    }
}
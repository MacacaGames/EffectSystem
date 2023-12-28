using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public interface IEffectableObject
    {
        /// <summary>
        /// Get the display name of an IEffectableObject
        /// Not really required, but very helpful when debugging
        /// </summary>
        /// <returns></returns>
        string GetDisplayName();
        Transform GetEffectViewParent(string viewRoot);

        /// <summary>
        /// Detect if a Effect can be request or not
        /// </summary>
        /// <param name="info"></param>
        /// <returns>If false, then system automatically reject a add effect request</returns>
        bool ApprovedAddEffect(EffectInfo info);

        /// <summary>
        /// Fire once when an Effect Instance is Active
        /// </summary>
        /// <param name="info"></param>
        void OnEffectActive(EffectInfo info);

        /// <summary>
        /// Fire once when an Effect Instance is DeActive
        /// </summary>
        /// <param name="info"></param>
        void OnEffectDeactive(EffectInfo info);

        // void OnEffectConditionActive(string condition, EffectTriggerConditionInfo info);

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
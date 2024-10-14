using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public interface IEffectableObject
    {
        /// <summary>
    /// Gets the display name of the IEffectableObject.
    /// Not strictly required, but very helpful for debugging.
    /// </summary>
    /// <returns></returns>
    string GetDisplayName();

    /// <summary>
    /// Retrieves the parent Transform for the effect view based on the viewRoot.
    /// </summary>
    /// <param name="viewRoot"></param>
    /// <returns></returns>
    Transform GetEffectViewParent(string viewRoot);

    /// <summary>
    /// Determines whether this IEffectableObject accepts the application of the 
    /// provided info. This method should only include checks and should not perform 
    /// any operations on the effect.
    /// </summary>
    /// <param name="info"></param>
    /// <returns>Returns false if this IEffectableObject reject the effect request.</returns>
    bool ApprovedAddEffect(EffectInfo info);

    /// <summary>
    /// Triggered when an Effect Instance becomes active.
    /// </summary>
    /// <param name="info"></param>
    void OnEffectActive(EffectInfo info);

    /// <summary>
    /// Triggered when an Effect Instance becomes inactive.
    /// </summary>
    /// <param name="info"></param>
    void OnEffectDeactive(EffectInfo info);

    /// <summary>
    /// Determines if the object is still "alive".
    /// </summary>
    /// <returns></returns>
    bool IsAlive();

    /// <summary>
    /// Since the actual runtime values are maintained by the IEffectableObject, 
    /// implement the values needed for your game.
    /// For example, ATK_Current = ATK_Constant * ATK_Ratio.
    /// </summary>
    /// <param name="parameterKey"></param>
    /// <returns></returns>
    float GetRuntimeValue(string parameterKey);

    /// <summary>
    /// Destroys the IEffectableObject.
    /// </summary>
    void DestoryEffectableObject();

    }
}
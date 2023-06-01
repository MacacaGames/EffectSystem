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
        float GetInputTypeValue(string inputType);

    }
}
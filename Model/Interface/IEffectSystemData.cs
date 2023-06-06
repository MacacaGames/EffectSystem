using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public interface IEffectSystemData
    {

        public List<EffectInfo> effects { get; set; }
        public List<EffectViewInfo> effectViewInfos { get; set; }

        /// <summary>
        /// Same as ApplicationController Init.
        /// </summary>
        Task Init();

        /// <summary>
        /// Usually used by EffectInfo.subInfo.
        /// </summary>
        /// <returns></returns>
        List<EffectInfo> GetEffects();
        
        /// <summary>
        /// Init EffectInfo List
        /// </summary>
        /// <returns></returns>
        Task FetchEffects();

        /// <summary>
        /// Init EffectViewInfo List
        /// </summary>
        /// <returns></returns>
        Task FetchEffectViewInfo();

        void InitEffectTypes();
    }
}
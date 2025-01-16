using System.Collections;
using System.Collections.Generic;
using MacacaGames.EffectSystem.Model;
using UnityEngine;
using MacacaGames.EffectSystem;

    public abstract class EffectViewBase : MonoBehaviour
    {
        protected EffectInstanceBase effectInstance;
        public EffectViewInfo viewInfo { get; protected set; }


        public virtual void Init(EffectInstanceBase effect, EffectViewInfo viewInfo)
        {
            this.effectInstance = effect;
            this.viewInfo = viewInfo;
        }

        public virtual void OnStart()
        {
            
        }

        public virtual void OnActive()
        {
            
        }

        public virtual void OnDeactive()
        {
            
        }

        public virtual void OnEnd()
        {
            
        }

        public virtual void OnCooldownEnd()
        {
            
        }

        public virtual void OnTick()
        {
            
        }

        public virtual void OnEffectApply()
        {
            
        }
    }

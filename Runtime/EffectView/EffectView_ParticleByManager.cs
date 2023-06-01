using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class EffectView_ParticleByManager : EffectViewBase
    {
        [Header("Particle")]
        [SerializeField]
        string[] onStartParticle = new string[0];

        [SerializeField]
        string[] onActiveParticle = new string[0];

        [SerializeField]
        string[] onDeactiveParticle = new string[0];

        [SerializeField]
        string[] onEndParticle = new string[0];

        [SerializeField]
        string[] onColdDownEndParticle = new string[0];



        public override void OnStart()
        {
            base.OnStart();

            foreach (var p in onStartParticle)
            {
                Play(p);
            }
        }

        public override void OnActive()
        {
            base.OnActive();

            foreach (var p in onActiveParticle)
            {
                Play(p);
            }
        }

        public override void OnDeactive()
        {
            base.OnDeactive();

            foreach (var p in onDeactiveParticle)
            {
                Play(p);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();

            foreach (var p in onEndParticle)
            {
                Play(p);
            }
        }

        public override void OnColdDownEnd()
        {
            base.OnColdDownEnd();

            foreach (var p in onColdDownEndParticle)
            {
                Play(p);
            }
        }


        void Play(string p)
        {
            EffectParticleManager.Instance.PlayOneShot(p, transform.position, transform.lossyScale);
        }
    }
}
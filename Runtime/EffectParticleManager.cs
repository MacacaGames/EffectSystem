using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class EffectParticleManager : UnitySingleton<EffectParticleManager>
    {
        public enum ParticleType
        {
            None = 0,
            Poison = 1,
            BossDie = 2,
            BossDust = 3,
            BossDieBG = 4,
            Explosion = 5,
            BurnOnce = 6,
            FirstStrike = 7,
            WeakStrike = 8,
            Boost_Crit = 9,
            Boost_Defend = 10,
            Boost_Power = 11,
            Boost_Recover = 12,
            Boost_Speed = 13,
            Boost_Invincible = 14,
            HeroDie = 15,
            HeroWakeUp = 16,
            HeroWakeUpBG = 17,
            Pumpkin_Disappear_Left = 18,
            Pumpkin_Disappear_Right = 19,
            HeroHeal = 20,
            BossMagicBallExplosion = 21,
            Particle_Slot01 = 22,
            Particle_Slot02 = 23,
            Particle_Slot03 = 24,
            Particle_Slot04 = 25,
            Particle_Slot05 = 26,
            Particle_Slot06 = 27,
            Particle_Slot07 = 28,
            Particle_Slot08 = 29,
            Particle_Slot09 = 30,
            Particle_Slot10 = 31,
        }

        [System.Serializable]
        public struct ParticleStruct
        {
            public string type;
            public ParticleSystem particle;
        }

        [Sirenix.OdinInspector.TableList]
        [SerializeField]
        ParticleStruct[] particleQuery;

        Dictionary<string, ParticleSystem> poolDict = new Dictionary<string, ParticleSystem>();

        [SerializeField]
        Transform poolsContainer;

        public void Init()
        {
            //Create pools
            foreach (var p in particleQuery)
            {                
                var ins = Instantiate(p.particle.gameObject, poolsContainer);
                poolDict.Add(p.type, ins.GetComponent<ParticleSystem>());
            }
        }


        public void PlayOneShot(string type, Vector3 pos)
        {
            PlayOneShot(type, pos, Vector3.one);
        }

        public void PlayOneShot(string type, Vector3 pos, Vector3 scale)
        {
            if (string.IsNullOrEmpty(type))
                return;

            ParticleSystem ps = poolDict[type];
            ps.transform.position = pos;
            ps.transform.localScale = scale;
            ps.Play();
        }

        // public void PlayBossDustParticle()
        // {
        //     PlayLayerViewParticle(poolDict[ParticleManager.ParticleType.BossDust], 10);
        // }
        // public void PlayHeroDieWindParticle()
        // {
        //     PlayLayerViewParticle(poolDict[ParticleManager.ParticleType.HeroDie], 12);
        //     PlayLayerViewParticle(poolDict[ParticleManager.ParticleType.HeroWakeUp], 12, true);
        // }
        // public void PlayHeroWakeUpParticle()
        // {
        //     // PlayLayerViewParticle(poolDict[ParticleManager.ParticleType.HeroWakeUp], 12);
        //     PlayOneShot(ParticleType.HeroWakeUp, Vector3.zero);
        // }

        // void PlayLayerViewParticle(ParticleSystem ps, int targetLayer, bool onlyCatchTex = false)
        // {
        //     ps.transform.position = Vector3.zero;
        //     var shape = ps.shape;
        //     shape.texture = LayerViewCapture.Instance.ResultTexture(targetLayer);
        //     ps.GetComponent<ParticleSyncShape>().SetShapeTexture(shape.texture);

        //     if (!onlyCatchTex) ps.Play();
        // }

    }
}
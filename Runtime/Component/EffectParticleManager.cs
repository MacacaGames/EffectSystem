using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class EffectParticleManager : UnitySingleton<EffectParticleManager>
    {
        [System.Serializable]
        public struct EffectParticleData
        {
            public string type;
            public ParticleSystem particle;
        }

        [SerializeField]
        EffectParticleData[] particleQuery;

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
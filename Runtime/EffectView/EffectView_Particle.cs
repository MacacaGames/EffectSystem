using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem{

public class EffectView_Particle : EffectViewBase
{
    [Header("Particle")]
    [SerializeField]
    ParticleSystem[] onStartParticle = new ParticleSystem[0];

    [SerializeField]
    ParticleSystem[] onActiveParticle = new ParticleSystem[0];

    [SerializeField]
    ParticleSystem[] onDeactiveParticle = new ParticleSystem[0];

    [MacacaGames.ViewSystem.ReadOnly]
    [SerializeField]
    ParticleSystem[] onEndParticle = new ParticleSystem[0];
    
    [SerializeField]
    ParticleSystem[] onColdDownEndParticle = new ParticleSystem[0];

      [SerializeField]
    ParticleSystem[] OnEffectApplyParticle = new ParticleSystem[0];



    public override void OnStart()
    {
        base.OnStart();

        foreach (var p in onStartParticle)
        {
            p.Play();
        }
    }

    public override void OnActive()
    {
        base.OnActive();
        Debug.Log("Active");

        foreach (var p in onActiveParticle)
        {
            p.Play();
        }
    }

    public override void OnDeactive()
    {
        base.OnDeactive();

        foreach (var p in onDeactiveParticle)
        {
            p.Play();
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        foreach (var p in onEndParticle)
        {
            p.Play();
        }
    }

    public override void OnColdDownEnd()
    {
        base.OnColdDownEnd();

        foreach (var p in onColdDownEndParticle)
        {
            p.Play();
        }
    }

    public override void OnEffectApply()
    {
        base.OnEffectApply();

        foreach (var p in OnEffectApplyParticle)
        {
            p.Play();
        }
    }
}
}
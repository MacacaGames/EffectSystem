using System;
using System.Collections.Generic;
using MacacaGames.EffectSystem;
using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class EffectViewController : MonoBehaviour
    {
        public static EffectViewController Instance;

        public Dictionary<GameObject, Queue<EffectViewBase>> effectViewPool =
            new Dictionary<GameObject, Queue<EffectViewBase>>();

        public Action<EffectViewBase> onEffectViewCreated;

        private List<IEffectViewFactory> _viewFactories = new List<IEffectViewFactory>();

        private EffectViewResource _resource;

        public void Init(IEffectViewFactory factory, EffectViewResource viewResource)
        {
            EffectSystem.Instance.OnEffectAdded += SpawnEffectView;
            _viewFactories.Add(factory);
            factory.Initialize(viewResource);

            _resource = viewResource;
            Instance = this;
        }

        void SpawnEffectView(EffectInstanceBase effect)
        {
            foreach (var factory in _viewFactories)
            {
                EffectViewBase view = factory.CreateEffectView(effect);
                if (view == null) continue;
                effect.OnEffectActive += view.OnActive;
                effect.OnEffectStart += view.OnStart;
                effect.OnEffectEnd += view.OnEnd;
                effect.OnEffectDeactive += view.OnDeactive;
                effect.OnEffectTick += view.OnTick;
                effect.OnCEffectooldownEnd += view.OnCooldownEnd;
                view.Init(effect, new EffectViewInfo());

                onEffectViewCreated?.Invoke(view);
            }
        }
    }
}

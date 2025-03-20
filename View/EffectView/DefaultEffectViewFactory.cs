using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class DefaultEffectViewFactory : IEffectViewFactory
    {
        EffectViewResource _effectViewResource;

        public void Initialize(EffectViewResource effectViewResource)
        {
            this._effectViewResource = effectViewResource;
        }
        public EffectViewBase CreateEffectView(EffectInstanceBase effectInstance)
        {
            var prefab = _effectViewResource.GetEffectViewPrefab(effectInstance.info.id);
            if (prefab == null) return null;
            Object.Instantiate(prefab).TryGetComponent<EffectViewBase>(out EffectViewBase effectView);
            return effectView;
        }
    }
}
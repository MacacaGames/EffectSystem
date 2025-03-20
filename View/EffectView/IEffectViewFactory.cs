namespace MacacaGames.EffectSystem
{
    public interface IEffectViewFactory
    {
        public void Initialize(EffectViewResource viewResource);
        EffectViewBase CreateEffectView(EffectInstanceBase effectInstance);
    }
}
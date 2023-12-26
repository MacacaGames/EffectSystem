namespace MacacaGames.EffectSystem
{
    /// <summary>
    /// Mark a property or field in EffectInstanceBase that can be inject value which is managed by EffectSystem.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = true)]
    public class EffectInstanceBaseInjectAttribute : System.Attribute
    {
        public EffectInstanceBaseInjectAttribute()
        {
        }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EffectTypeGroupAttribute : System.Attribute
    {
        public EffectTypeGroupAttribute(string group)
        {
            this.group = group;
        }

        public string group;
    }
}

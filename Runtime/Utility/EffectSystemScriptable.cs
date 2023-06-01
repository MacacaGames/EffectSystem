using UnityEngine;

namespace MacacaGames.EffectSystem
{
	public partial struct EffectSystemScriptable
	{

		public partial struct EffectType
		{
			public const string CCPrevention = "CCPrevention";	
		}
		public partial struct InputType
		{
		    public const string  Fixed = "Fixed";

		    public const string  AtkBase = "AtkBase";

		    public const string  HpBase = "HpBase";

		}
		public partial struct SkillAttackType
		{
		    public const string  Melee = "Melee";

		    public const string  Range = "Range";

		    public const string  Support = "Support";

		}
		public partial struct ActiveCondition
		{
		    public const string  OnEffectStart = "OnEffectStart";
		}
		public partial struct DeactiveCondition
		{			
		    public const string  None = "None";
			public const string OnOwnerAfterDoAttack = "OnOwnerAfterDoAttack";
			public const string AfterActive = "AfterActive";		
		}
		public partial struct TriggerTransType
		{
		    public const string  SkipNewOne = "SkipNewOne";

		    public const string  CutOldOne = "CutOldOne";

		}
		public partial struct EffectInfoLogic
		{
		    public const string  OnlyActiveOnce = "OnlyActiveOnce";

		    public const string  ReactiveAfterCooldownEnd = "ReactiveAfterCooldownEnd";

		    public const string  DestroyAfterMaintainTimeEnd = "DestroyAfterMaintainTimeEnd";

		}
		public partial struct EffectViewRoot
		{
		    
		}
		public partial struct ConditionLogic
		{
		    public const string  None = "None";

		    public const string  Greater = "Greater";

		    public const string  GreaterEqual = "GreaterEqual";

		    public const string  Equal = "Equal";

		    public const string  LessEqual = "LessEqual";

		    public const string  Less = "Less";

		}
		public partial struct CastTarget
		{

		}
		public partial struct DamageType
		{
		    public const string  Physical = "Physical";

		    public const string  Magical = "Magical";

		    public const string  True = "True";

		}
		public partial struct SkillTargetVfxType
		{
		    
		}
		public partial struct SkillCastVfxType
		{
		    public const string  None = "None";
		}

	}
}

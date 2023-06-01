using UnityEngine;

namespace MacacaGames.EffectSystem
{
	public struct EffectSystemScriptable
	{

		public struct EffectType
		{
		    public const string  AllStatsRatio = "AllStatsRatio";

		    public const string  Atk_Constant = "Atk_Constant";

		    public const string  Atk_Ratio = "Atk_Ratio";

		    public const string  AttackRangeBlock = "AttackRangeBlock";

		    public const string  Bleed = "Bleed";

		    public const string  BuffPrevention = "BuffPrevention";

		    public const string  Burning = "Burning";

		    public const string  CCPrevention = "CCPrevention";

		    public const string  CritMultiplier_Constant = "CritMultiplier_Constant";

		    public const string  CritRate_Constant = "CritRate_Constant";

		    public const string  DamageReductionRate = "DamageReductionRate";

		    public const string  DamageTaken = "DamageTaken";

		    public const string  DamgeRatioAgainstLastRow = "DamgeRatioAgainstLastRow";

		    public const string  DebuffPrevention = "DebuffPrevention";

		    public const string  DodgeRate_Constant = "DodgeRate_Constant";

		    public const string  Freeze = "Freeze";

		    public const string  IncreasedHealRatio = "IncreasedHealRatio";

		    public const string  Hp_Constant = "Hp_Constant";

		    public const string  Hp_Ratio = "Hp_Ratio";

		    public const string  IgnoreReflect = "IgnoreReflect";

		    public const string  MagicalDamageReductionRate = "MagicalDamageReductionRate";

		    public const string  MagicalDamageTaken = "MagicalDamageTaken";

		    public const string  MysterySkill = "MysterySkill";

		    public const string  PhysicalDamageReductionRate = "PhysicalDamageReductionRate";

		    public const string  PhysicalDamageTaken = "PhysicalDamageTaken";

		    public const string  Sleep = "Sleep";

		    public const string  Taunt = "Taunt";

		    public const string  Trigger_Attach = "Trigger_Attach";

		    public const string  Trigger_Attach_Self = "Trigger_Attach_Self";

		    public const string  Trigger_AttachAllies = "Trigger_AttachAllies";

		    public const string  Trigger_ExplodeTargetBuff = "Trigger_ExplodeTargetBuff";

		    public const string  Trigger_Heal_Constant = "Trigger_Heal_Constant";

		    public const string  Trigger_Heal_Ratio = "Trigger_Heal_Ratio";

		    public const string  Trigger_HitSelf_Constant = "Trigger_HitSelf_Constant";

		    public const string  Trigger_HitSelf_Ratio = "Trigger_HitSelf_Ratio";

		    public const string  Trigger_KillSelf = "Trigger_KillSelf";

		    public const string  Trigger_RemoveCC = "Trigger_RemoveCC";

		    public const string  Trigger_RemoveTargetBuff = "Trigger_RemoveTargetBuff";

		    public const string  Trigger_RemoveTargetDebuff = "Trigger_RemoveTargetDebuff";

		    public const string  AttackDamageRatio = "AttackDamageRatio";

		    public const string  Trigger_AttachWithFixedInput = "Trigger_AttachWithFixedInput";

		    public const string  ReducedHealRatio = "ReducedHealRatio";

		    public const string  IncreasedHealConstant = "IncreasedHealConstant";

		    public const string  ReducedHealConstant = "ReducedHealConstant";

		    public const string  Trigger_HitTarget_Ratio = "Trigger_HitTarget_Ratio";

		    public const string  Trigger_HitTarget_Constant = "Trigger_HitTarget_Constant";

		}
		public struct InputType
		{
		    public const string  Fixed = "Fixed";

		    public const string  AtkBase = "AtkBase";

		    public const string  HpBase = "HpBase";

		    public const string  SpeedBase = "SpeedBase";

		    public const string  CritRateBase = "CritRateBase";

		    public const string  CritMultiplierBase = "CritMultiplierBase";

		    public const string  DodgeBase = "DodgeBase";

		    public const string  AtkCurrent = "AtkCurrent";

		    public const string  HpCurrent = "HpCurrent";

		    public const string  SpeedCurrent = "SpeedCurrent";

		    public const string  CritRateCurrent = "CritRateCurrent";

		    public const string  CritMultiplierCurrent = "CritMultiplierCurrent";

		    public const string  DodgeCurrent = "DodgeCurrent";

		    public const string  ChargingStack = "ChargingStack";

		    public const string  HpRatioCurrent = "HpRatioCurrent";

		    public const string  Taunt = "Taunt";

		    public const string  Buffs = "Buffs";

		    public const string  Debuffs = "Debuffs";

		}
		public struct SkillAttackType
		{
		    public const string  Melee = "Melee";

		    public const string  Range = "Range";

		    public const string  Support = "Support";

		}
		public struct ActiveCondition
		{
		    public const string  OnEffectStart = "OnEffectStart";

		    public const string  OnOwnerBeforeGetAttack = "OnOwnerBeforeGetAttack";

		    public const string  OnOwnerAfterGetAttack = "OnOwnerAfterGetAttack";

		    public const string  OnOwnerBeforeDoAttack = "OnOwnerBeforeDoAttack";

		    public const string  OnOwnerAfterDoAttack = "OnOwnerAfterDoAttack";

		    public const string  OnOwnerBeforeGetHit = "OnOwnerBeforeGetHit";

		    public const string  OnOwnerAfterGetHit = "OnOwnerAfterGetHit";

		    public const string  OnOwnerKillOther = "OnOwnerKillOther";

		    public const string  OnOwnerCritAndBeforeDoAttack = "OnOwnerCritAndBeforeDoAttack";

		    public const string  OnOwnerGetAttackMiss = "OnOwnerGetAttackMiss";

		    public const string  OnOwnerTriggerNearDeath = "OnOwnerTriggerNearDeath";

		    public const string  OnOwnerBeforeHeal = "OnOwnerBeforeHeal";

		    public const string  OnOwnerAfterHeal = "OnOwnerAfterHeal";

		    public const string  OnOwnerEnterAttackLine = "OnOwnerEnterAttackLine";

		    public const string  OnOwnerLeaveAttackLine = "OnOwnerLeaveAttackLine";

		    public const string  OnOwnerAfterDoHit = "OnOwnerAfterDoHit";

		    public const string  OnOwnerFallingEnd = "OnOwnerFallingEnd";

		    public const string  OnHeroUseProactiveSkill = "OnHeroUseProactiveSkill";

		    public const string  OnOwnerCritAndAfterDoHit = "OnOwnerCritAndAfterDoHit";

		    public const string  OnOwnerBePicked = "OnOwnerBePicked";

		    public const string  OnOwnerAttackTargetPoisonStackMax = "OnOwnerAttackTargetPoisonStackMax";

		    public const string  OnOwnerBeforeAttackLastRow = "OnOwnerBeforeAttackLastRow";

		    public const string  OnOwnerBeforeApproveEffect = "OnOwnerBeforeApproveEffect";

		    public const string  OnOwnerAfterGetPhysicalAttack = "OnOwnerAfterGetPhysicalAttack";

		    public const string  OnOwnerAfterGetMagicalAttack = "OnOwnerAfterGetMagicalAttack";

		    public const string  OnOwnerAfterDoSupport = "OnOwnerAfterDoSupport";

		    public const string  OnOwnerAfterGetSupport = "OnOwnerAfterGetSupport";

		    public const string  OnOwnerBeforeDoSupport = "OnOwnerBeforeDoSupport";

		    public const string  OnOwnerBeforeGetSupport = "OnOwnerBeforeGetSupport";

		    public const string  OnOwnerAfterDied = "OnOwnerAfterDied";

		    public const string  OnOwnerHpGreaterEqual20 = "OnOwnerHpGreaterEqual20";

		    public const string  OnOwnerHpGreaterEqual40 = "OnOwnerHpGreaterEqual40";

		    public const string  OnOwnerHpGreaterEqual60 = "OnOwnerHpGreaterEqual60";

		    public const string  OnOwnerHpGreaterEqual80 = "OnOwnerHpGreaterEqual80";

		    public const string  OnOwnerHpGreaterEqual100 = "OnOwnerHpGreaterEqual100";

		    public const string  OnOwnerHpLess20 = "OnOwnerHpLess20";

		    public const string  OnOwnerHpLess40 = "OnOwnerHpLess40";

		    public const string  OnOwnerHpLess60 = "OnOwnerHpLess60";

		    public const string  OnOwnerHpLess80 = "OnOwnerHpLess80";

		    public const string  OnOwnerHpLess100 = "OnOwnerHpLess100";

		    public const string  OnOwnerAttackTargetHpGreaterEqual0 = "OnOwnerAttackTargetHpGreaterEqual0";

		    public const string  OnOwnerAttackTargetHpGreaterEqual60 = "OnOwnerAttackTargetHpGreaterEqual60";

		    public const string  OnOwnerAttackTargetHpGreaterEqual80 = "OnOwnerAttackTargetHpGreaterEqual80";

		    public const string  OnOwnerAttackTargetHpGreaterEqual100 = "OnOwnerAttackTargetHpGreaterEqual100";

		    public const string  OnOwnerAttackTargetHpLess0 = "OnOwnerAttackTargetHpLess0";

		    public const string  OnOwnerAttackTargetHpLess50 = "OnOwnerAttackTargetHpLess50";

		}
		public struct DeactiveCondition
		{
		    public const string  None = "None";

		    public const string  AfterActive = "AfterActive";

		    public const string  OnOwnerBeforeGetAttack = "OnOwnerBeforeGetAttack";

		    public const string  OnOwnerAfterGetAttack = "OnOwnerAfterGetAttack";

		    public const string  OnOwnerBeforeDoAttack = "OnOwnerBeforeDoAttack";

		    public const string  OnOwnerAfterDoAttack = "OnOwnerAfterDoAttack";

		    public const string  OnOwnerBeforeGetHit = "OnOwnerBeforeGetHit";

		    public const string  OnOwnerAfterGetHit = "OnOwnerAfterGetHit";

		    public const string  OnOwnerKillOther = "OnOwnerKillOther";

		    public const string  OnOwnerCritAndBeforeDoAttack = "OnOwnerCritAndBeforeDoAttack";

		    public const string  OnOwnerGetAttackMiss = "OnOwnerGetAttackMiss";

		    public const string  OnOwnerTriggerNearDeath = "OnOwnerTriggerNearDeath";

		    public const string  OnOwnerBeforeHeal = "OnOwnerBeforeHeal";

		    public const string  OnOwnerAfterHeal = "OnOwnerAfterHeal";

		    public const string  OnOwnerEnterAttackLine = "OnOwnerEnterAttackLine";

		    public const string  OnOwnerLeaveAttackLine = "OnOwnerLeaveAttackLine";

		    public const string  OnOwnerAfterDoHit = "OnOwnerAfterDoHit";

		    public const string  OnOwnerFallingEnd = "OnOwnerFallingEnd";

		    public const string  OnHeroUseProactiveSkill = "OnHeroUseProactiveSkill";

		    public const string  OnOwnerCritAndAfterDoHit = "OnOwnerCritAndAfterDoHit";

		    public const string  OnOwnerBePicked = "OnOwnerBePicked";

		    public const string  OnOwnerAttackTargetPoisonStackMax = "OnOwnerAttackTargetPoisonStackMax";

		    public const string  OnOwnerBeforeAttackLastRow = "OnOwnerBeforeAttackLastRow";

		    public const string  OnOwnerBeforeApproveEffect = "OnOwnerBeforeApproveEffect";

		    public const string  OnOwnerAfterGetPhysicalAttack = "OnOwnerAfterGetPhysicalAttack";

		    public const string  OnOwnerAfterGetMagicalAttack = "OnOwnerAfterGetMagicalAttack";

		    public const string  OnOwnerAfterDoSupport = "OnOwnerAfterDoSupport";

		    public const string  OnOwnerAfterGetSupport = "OnOwnerAfterGetSupport";

		    public const string  OnOwnerBeforeDoSupport = "OnOwnerBeforeDoSupport";

		    public const string  OnOwnerBeforeGetSupport = "OnOwnerBeforeGetSupport";

		    public const string  OnOwnerAfterDied = "OnOwnerAfterDied";

		    public const string  OnOwnerHpGreaterEqual20 = "OnOwnerHpGreaterEqual20";

		    public const string  OnOwnerHpGreaterEqual40 = "OnOwnerHpGreaterEqual40";

		    public const string  OnOwnerHpGreaterEqual60 = "OnOwnerHpGreaterEqual60";

		    public const string  OnOwnerHpGreaterEqual80 = "OnOwnerHpGreaterEqual80";

		    public const string  OnOwnerHpGreaterEqual100 = "OnOwnerHpGreaterEqual100";

		    public const string  OnOwnerHpLess20 = "OnOwnerHpLess20";

		    public const string  OnOwnerHpLess40 = "OnOwnerHpLess40";

		    public const string  OnOwnerHpLess60 = "OnOwnerHpLess60";

		    public const string  OnOwnerHpLess80 = "OnOwnerHpLess80";

		    public const string  OnOwnerHpLess100 = "OnOwnerHpLess100";

		    public const string  OnOwnerAttackTargetHpGreaterEqual0 = "OnOwnerAttackTargetHpGreaterEqual0";

		    public const string  OnOwnerAttackTargetHpGreaterEqual100 = "OnOwnerAttackTargetHpGreaterEqual100";

		    public const string  OnOwnerAttackTargetHpLess0 = "OnOwnerAttackTargetHpLess0";

		    public const string  OnOwnerAttackTargetHpLess50 = "OnOwnerAttackTargetHpLess50";

		}
		public struct TriggerTransType
		{
		    public const string  SkipNewOne = "SkipNewOne";

		    public const string  CutOldOne = "CutOldOne";

		}
		public struct EffectInfoLogic
		{
		    public const string  None = "None";

		    public const string  OnlyActiveOnce = "OnlyActiveOnce";

		    public const string  ReactiveAfterCooldownEnd = "ReactiveAfterCooldownEnd";

		    public const string  DestroyAfterMaintainTimeEnd = "DestroyAfterMaintainTimeEnd";

		}
		public struct EffectViewRoot
		{
		    public const string  Self = "Self";

		    public const string  UI = "UI";

		    public const string  Head = "Head";

		}
		public struct ConditionLogic
		{
		    public const string  None = "None";

		    public const string  Greater = "Greater";

		    public const string  GreaterEqual = "GreaterEqual";

		    public const string  Equal = "Equal";

		    public const string  LessEqual = "LessEqual";

		    public const string  Less = "Less";

		}
		public struct CastTarget
		{
		    public const string  CastToOtherPlayer = "CastToOtherPlayer";

		    public const string  CastToSelf = "CastToSelf";

		    public const string  CastToNextAlly = "CastToNextAlly";

		}
		public struct DamageType
		{
		    public const string  Physical = "Physical";

		    public const string  Magical = "Magical";

		    public const string  True = "True";

		}
		public struct SkillTargetVfxType
		{
		    public const string  Physic = "Physic";

		    public const string  GenericMagic = "GenericMagic";

		    public const string  IceMagic = "IceMagic";

		    public const string  FireMagic = "FireMagic";

		    public const string  DarkMagic = "DarkMagic";

		    public const string  WaterMagic = "WaterMagic";

		    public const string  Buff = "Buff";

		}
		public struct SkillCastVfxType
		{
		    public const string  None = "None";

		    public const string  Water = "Water";

		    public const string  Fire = "Fire";

		    public const string  Dark = "Dark";

		    public const string  Buff = "Buff";

		    public const string  GeneralMagic = "GeneralMagic";

		}

	}
}

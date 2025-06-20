using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MacacaGames.EffectSystem.Model
{
    public enum TriggerTransType
    {
        // 跳過新觸發器
        SkipNewOne = 0,
        // Deactive舊觸發器
        CutOldOne = 1,
        // 復用舊觸發器，timer不重新start
        KeepOldOneWithoutTimerReset = 2,
    }

    public enum EffectLifeCycleLogic
    {
        None = 0,
        OnlyActiveOnce = 1,
        ReactiveAfterCooldownEnd = 2,
    }

    public enum ConditionLogic
    {
        None = 0,
        Greater = 1,
        GreaterEqual = 2,
        Equal = 3,
        LessEqual = 4,
        Less = 5
    }

    public enum EffectTaxonomy
    {
        Info = 0, Domain = 101, Kingdom = 102,
    }

}
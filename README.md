See [Document](https://macacagames.github.io/EffectSystemDocs/) for more detail.

> [中文版 (README_Zh.md)](README_Zh.md)

# EffectSystem

EffectSystem is a **data-driven** Buff / Debuff / Trigger framework for Unity and .NET. It models all gameplay effects as `EffectInfo` data, driven by a condition system for activation and deactivation, with automatic type resolution via reflection.

Whether you're building a turn-based RPG, a real-time action game, or a roguelike, EffectSystem provides a unified way to manage numerical modifiers, timed buffs, triggered abilities, and complex effect compositions — all configurable through data without code changes.

### What It Can Do

- Increase ATK by 50 points.
- Increase HP by 10%.
- Increase DEF by 5% for 50 seconds.
- Reduce a specified enemy's ATK by 100 points, usable every 30 seconds.
- Reduce the opponent's DEF by 50% when a successful block occurs.
- Apply a DOT effect that ticks every round.
- Trigger a heal with 40% probability on each attack.
- Attach sub-effects to targets when a skill activates.

### Key Features

- **Data-Logic Separation** — All effect configuration lives in `EffectInfo` structs (serializable via MessagePack, JSON, or ScriptableObject). Adjust values without changing code.
- **Condition-Driven** — Effects don't activate automatically. They respond to named conditions (`activeCondition` / `deactiveCondition`) triggered by game logic.
- **Reflection-Based Type Resolution** — Name your class `Effect_{type}` and the framework finds it automatically. No manual registration needed.
- **Object Pooling** — All effect instances are pooled to avoid GC pressure.
- **Flexible Timer System** — Support seconds, rounds, actions, or any custom time unit via pluggable `TimerTicker`.
- **Excel / Data-Table Friendly** — Design effects in spreadsheets, export as data, and load at runtime.
- **Server Compatible** — Core logic runs outside Unity with `#if !Server` guards for view-only code.

---

# Installation

### Option 1: Unity Package Manager

Add the following to your project's `manifest.json`:
```json
{
    "dependencies": {
        "com.macacagames.utility": "https://github.com/MacacaGames/MacacaUtility.git",
        "com.macacagames.effectsystem.editor": "git@github.com:MacacaGames/EffectSystem.git?path=Editor/src",
        "com.macacagames.effectsystem.model": "git@github.com:MacacaGames/EffectSystem.git?path=Model/src",
        "com.macacagames.effectsystem.runtime": "git@github.com:MacacaGames/EffectSystem.git?path=Runtime/src",
        "com.macacagames.effectsystem.view": "git@github.com:MacacaGames/EffectSystem.git?path=View"
    }
}
```

### Option 2: Git SubModule

```bash
git submodule add https://github.com/MacacaGames/EffectSystem.git MyPackages
```

> EffectSystem depends on MacacaUtility. Add it as well:

```json
{
    "dependencies": {
        "com.macacagames.utility": "https://github.com/MacacaGames/MacacaUtility.git",
        "com.macacagames.effectsystem.editor": "file:../MyPackages/EffectSystem/Editor/src",
        "com.macacagames.effectsystem.model": "file:../MyPackages/EffectSystem/Model/src",
        "com.macacagames.effectsystem.runtime": "file:../MyPackages/EffectSystem/Runtime/src",
        "com.macacagames.effectsystem.view": "file:../MyPackages/EffectSystem/View"
    }
}
```

---

# Server-Side .csproj Setup

If your project uses EffectSystem on the server side (outside Unity), create your own `.csproj` files since they are project-specific and not included in this repository.

Create the following two files **outside** the EffectSystem submodule directory (e.g., in the same parent folder):

### EffectSystem.Model.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <DefineConstants>Server</DefineConstants>
        <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    </PropertyGroup>

    <ItemGroup>
        <Compile Include="EffectSystem/Model/src/**/*.cs" Exclude="**/*.meta" />
    </ItemGroup>

    <ItemGroup>
        <!-- Add your project-specific references here -->
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
        <PackageReference Include="MessagePack.AspNetCoreMvcFormatter" Version="3.1.4" />
        <PackageReference Include="MessagePack.UnityShims" Version="3.1.4" />
        <PackageReference Include="MessagePack" Version="3.1.4" />
    </ItemGroup>
</Project>
```

### EffectSystem.Runtime.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <DefineConstants>Server</DefineConstants>
        <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    </PropertyGroup>

    <ItemGroup>
        <Compile Include="EffectSystem/Runtime/src/**/*.cs" Exclude="**/*.meta" />
    </ItemGroup>

    <ItemGroup>
        <Reference Include="Sirenix.OdinInspector.Attributes">
            <HintPath>EffectSystem/Runtime/Dlls/Sirenix.OdinInspector.Attributes.dll</HintPath>
        </Reference>
        <Reference Include="Macaca.Utility">
            <HintPath>EffectSystem/Runtime/Dlls/Macaca.Utility.dll</HintPath>
        </Reference>
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="EffectSystem.Model.csproj" />
    </ItemGroup>
</Project>
```

> **Note:** Adjust `HintPath`, `ProjectReference`, and `Compile Include` paths based on where you place these files relative to the EffectSystem submodule.

---

# Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                   Model Layer (Data)                │
│  EffectInfo, Enums, EffectDataProvider,             │
│  IEffectableObject, IEffectTimer                    │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│               Runtime Layer (Core Engine)           │
│  EffectSystem (Singleton)                           │
│  ├─ Object Pool (RequestEffect / RecoveryEffect)    │
│  ├─ Condition Dispatch (EffectTriggerCondition)     │
│  ├─ Timer Management (TimerTicker)                  │
│  ├─ EffectInstanceBase (Effect instance base)       │
│  ├─ EffectTriggerBase (Trigger instance base)       │
│  ├─ EffectCondition (Activation/deactivation logic) │
│  └─ DefaultTimerBase (Countdown timer)              │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│              Custom Layer (Your Project)            │
│  Effect_IncreasedAtkConstant  (Element)             │
│  Effect_Trigger_HitConstant   (Trigger)             │
│  Effect_Stunned               (Element)             │
│  ...your custom Effect implementations              │
└─────────────────────────────────────────────────────┘
```

| Layer | Content | Modifiable? |
|-------|---------|-------------|
| **Model** | Pure data structures, enums, interface definitions | Not recommended |
| **Runtime** | EffectSystem core engine | Not recommended |
| **Custom** | Project-specific Effect implementations | Freely extend |

---

# Fundamentals

### Initializing EffectSystem

Before using the Effect System, create an `EffectSystem` instance and call `Init()`:

```csharp
var effectSystem = new EffectSystem();
effectSystem.Init();
```

In Unity, EffectSystem uses the singleton pattern by default. After `Init()`, access it via `EffectSystem.Instance`.

Outside Unity (e.g., on a server), manage the instance yourself with your own dependency injection solution.

### IEffectableObject

Any C# object that can receive effects must implement `IEffectableObject`. Characters, enemies, cards, weapons, or even global singletons can be effect carriers.

```csharp
public interface IEffectableObject
{
    /// Display name for debugging.
    string GetDisplayName();

    /// Returns the parent Transform for visual effect attachment.
    Transform GetEffectViewParent(string viewRoot);

    /// Gate check — return false to reject (e.g., immunity).
    /// Should only include checks, no side effects.
    bool ApprovedAddEffect(EffectInfo info);

    /// Called when an effect instance becomes active.
    void OnEffectActive(EffectInfo info);

    /// Called when an effect instance becomes inactive.
    void OnEffectDeactive(EffectInfo info);

    /// Whether this object is still alive. Effects can only attach to alive objects.
    bool IsAlive();

    /// Project-defined runtime value lookup.
    /// e.g., ATK_Current = ATK_Constant * ATK_Ratio.
    float GetRuntimeValue(string parameterKey);

    /// Destroys this IEffectableObject.
    void DestoryEffectableObject();
}
```

---

# EffectInfo — Core Data Structure

`EffectInfo` is the data core of the entire system. Every effect is fully described by one `EffectInfo` struct:

```csharp
[MessagePack.MessagePackObject(true)]
[Serializable]
public partial struct EffectInfo
{
    public string id;                  // Optional unique ID (used by Description templates)
    public string type;                // Effect type name (maps to Effect_{type} class)
    public float value;                // Base value
    public string activeCondition;     // Activation condition name
    public List<string> activeRequirement;    // Pre-activation checks
    public float activeProbability;    // Activation probability (0~1)
    public string deactiveCondition;   // Deactivation condition name
    public List<string> deactiveRequirement;  // Pre-deactivation checks
    public float deactiveProbability;  // Deactivation probability (0~1)
    public float maintainTime;         // Duration
    public float cooldownTime;         // Cooldown time
    public EffectLifeCycleLogic logic; // Lifecycle logic preset
    public TriggerTransType triggerTransType; // Behavior on repeated triggers
    public List<string> tags;          // Tags (Buff, Debuff, CC, etc.)
    public List<string> subInfoIds;    // Sub-effect ID list
    public List<string> viewInfoIds;   // Visual effect config IDs
    public Dictionary<string, string> parameters; // Custom key-value parameters
}
```

### Field Reference

#### Basic Fields

| Field | Description | Example |
|-------|-------------|---------|
| `id` | Optional unique ID, used by Description template references | `"#myBuff"` |
| `type` | Effect type name — system looks for `Effect_{type}` class | `"ATK_Constant"` |
| `value` | Numeric value — meaning defined by the concrete implementation | `100` |

#### Condition Fields

| Field | Description | Example |
|-------|-------------|---------|
| `activeCondition` | Activation event name, triggered externally via `EffectTriggerCondition()` | `"OnEffectStart"`, `"OnBeforeAttack"` |
| `deactiveCondition` | Deactivation event name | `"AfterActive"`, `"OnRoundEnd"` |
| `activeProbability` | Activation probability (1 = 100%, 0 = skip probability check) | `0.5` |
| `deactiveProbability` | Deactivation probability | `1` |

#### Time Fields

| Field | Description | Example |
|-------|-------------|---------|
| `maintainTime` | Duration after activation (auto-deactivate when elapsed). 0 = no time-based lifecycle | `3` (3 tick units) |
| `cooldownTime` | Cooldown after deactivation before re-activation is allowed | `2` |

#### Behavior Fields

| Field | Description |
|-------|-------------|
| `logic` | Lifecycle preset: `None` (sleep after deactivation), `OnlyActiveOnce` (remove after one activation), `ReactiveAfterCooldownEnd` (auto-reactivate after cooldown) |
| `triggerTransType` | Repeated trigger behavior: `SkipNewOne` (ignore), `CutOldOne` (deactivate old then activate new), `KeepOldOneWithoutTimerReset` (keep old, no timer reset) |

#### Composition Fields

| Field | Description |
|-------|-------------|
| `tags` | Tag list for categorization (Buff/Debuff/CC) and batch operations |
| `subInfoIds` | Sub-effect ID list, lazily loaded via `EffectDataProvider.GetEffectInfo` |
| `parameters` | Arbitrary key-value string pairs for flexible extension |

---

# Effect Lifecycle

An effect goes through the following lifecycle from attachment to removal:

```
                     AddRequestedEffect()
                           │
                           ▼
              ┌─── RequestEffect() (from pool or create new)
              │            │
              │            ▼
              │      Reset(info) — reset state
              │            │
              │            ▼
              │    ┌── Start() ────────────────────────────┐
              │    │   ├─ Create EffectCondition           │
              │    │   ├─ RegistEffectTriggerCondition()   │
              │    │   │   (register OnActive/OnDeactive   │
              │    │   │    to owner's condition dict)     │
              │    │   ├─ Add Timer to TimerTicker         │
              │    │   ├─ condition.Start()                │
              │    │   │   (if activeCondition is          │
              │    │   │    "OnEffectStart" → immediate)   │
              │    │   └─ OnStart() callback               │
              │    └───────────────────────────────────────┘
              │                    │
              │    ┌───────────────▼────────────────────┐
              │    │  Waiting for ActiveCondition       │
              │    │  (external EffectTriggerCondition) │
              │    └───────────────┬────────────────────┘
              │                    │
              │    ┌───────────────▼────────────────────┐
              │    │  OnActive() ─── effect activates   │
              │    │   ├─ Model Injection (InjectModels)│
              │    │   ├─ ExecuteActive()               │
              │    │   │   ├─ SetDirty(true)            │
              │    │   │   ├─ owner.OnEffectActive()    │
              │    │   │   └─ EffectViewOnActive()      │
              │    │   └─ [Trigger] OnTrigger()         │
              │    └───────────────┬────────────────────┘
              │                    │
              │     ┌──────────────▼───────────────────┐
              │     │ OnDeactive() ─── effect ends     │
              │     │  ├─ ExecuteDeactive()            │
              │     │  │   ├─ owner.OnEffectDeactive() │
              │     │  │   └─ EffectViewOnDeactive()   │
              │     │  └─ LifeCycleLogic check:        │
              │     │     ├─ None / OnlyActiveOnce     │
              │     │     │   → SetSleep()             │
              │     │     └─ ReactiveAfterCooldownEnd  │
              │     │         → cooldownTimer.Start()  │
              │     └──────────────────────────────────┘
              │                    │
              │    (may loop back to Active)
              │                    │
              │     ┌──────────────▼───────────────────┐
              │     │  End() ─── remove and recycle    │
              │     │   ├─ Remove from TimerTicker     │
              │     │   ├─ UnregistEffectTriggerCond.  │
              │     │   ├─ OnEnd() callback            │
              │     │   └─ RemoveEffectView()          │
              │     └──────────────┬───────────────────┘
              │                    │
              └── RecoveryEffectBase() (return to pool)
```

### Key Concepts

1. **Start ≠ Active**: `Start()` means the effect is attached to the owner but not necessarily in effect. It only truly activates when its `activeCondition` is triggered.
2. **Sleep Mechanism**: When `LifeCycleLogic` is `None` or `OnlyActiveOnce`, the effect enters Sleep after deactivation. A sleeping effect provides no value (returns 0) and does not participate in calculations.
3. **isActive affects GetValue()**: `GetValue()` checks `condition.isActive` — if false, it returns 0. This is the core mechanism for Element-type effects.

---

# Effect Instance Implementation

### Type Resolution

The system uses reflection to find the class matching an `EffectInfo`'s type. The naming convention is `Effect_{type}`:

```csharp
// EffectInfo.type = "ATK_Constant"  →  system looks for class "Effect_ATK_Constant"
// EffectInfo.type = "IncreasedAtkConstant"  →  "Effect_IncreasedAtkConstant"
```

Results are cached after the first lookup. If no matching class is found, the system falls back to `EffectInstanceBase`.

### Basic Implementation (Element)

A basic effect that simply provides a numeric value:

```csharp
public class Effect_ATK_Constant : EffectInstanceBase
{
    public Effect_ATK_Constant(EffectSystem effectSystem) : base(effectSystem) { }
}
```

That's it. The `value` from `EffectInfo` is automatically accumulated via `GetEffectSum()`.

### Advanced Implementation

Override methods from `EffectInstanceBase` to customize behavior:

```csharp
public class Effect_MyEffect : EffectInstanceBase
{
    public Effect_MyEffect(EffectSystem effectSystem) : base(effectSystem) { }

    /// If the sum of this type exceeds maxEffectValue, new effects won't be added.
    public override float maxEffectValue => 100;

    /// Upper and lower bounds for GetEffectSum().
    public override (float min, float max) sumLimit => (0f, 100f);

    /// Max stack count for this type on a single owner.
    public override int countLimit => 5;

    protected override void OnStart() { /* Called when attached to owner */ }
    public override void OnActive(EffectTriggerConditionInfo triggerConditionInfo) { /* Effect activates */ }
    public override void OnDeactive(EffectTriggerConditionInfo triggerConditionInfo) { /* Effect deactivates */ }
    public override void OnCooldownEnd() { /* Cooldown finished */ }
    protected override void OnEnd() { /* About to be recycled */ }
}
```

### Trigger Implementation

Triggers execute logic on activation (rather than passively providing values). Inherit from `EffectTriggerBase`:

```csharp
public class Effect_MyTrigger : EffectTriggerBase
{
    public Effect_MyTrigger(EffectSystem effectSystem) : base(effectSystem) { }

    // Executed immediately after OnActive().
    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        // Your trigger logic here
    }
}
```

---

# Element vs. Trigger — Two Effect Categories

EffectSystem effects fall into two fundamental categories:

### Element (Passive Value Modifier)

Elements **passively provide numeric values**. Their `value` is accumulated via `GetEffectSum()` and read by game logic:

```csharp
// No custom logic needed — value is auto-accumulated
public class Effect_IncreasedAtkConstant : EffectInstanceBase
{
    public Effect_IncreasedAtkConstant(EffectSystem effectSystem) : base(effectSystem) { }
}

// Game logic reads the accumulated value
float atkBonus = effectSystem.GetEffectSum(character, "IncreasedAtkConstant");
int finalAtk = (int)(baseAtk * (1 + atkRatio) + atkBonus);
```

### Trigger (Active Behavior Executor)

Triggers **execute one-shot logic** when activated. They typically use `deactiveCondition: "AfterActive"` to deactivate immediately after firing:

```csharp
public class Effect_Trigger_Heal : EffectTriggerBase
{
    public Effect_Trigger_Heal(EffectSystem effectSystem) : base(effectSystem) { }

    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        // Execute healing logic
    }
}
```

### Comparison

| Aspect | Element | Trigger |
|--------|---------|---------|
| Inherits | `EffectInstanceBase` | `EffectTriggerBase` |
| After activation | Continues providing value | Executes logic once |
| Typical `deactiveCondition` | Different from `activeCondition` (e.g., `"OnRoundEnd"`) | `"AfterActive"` |
| Typical `activeCondition` | `"OnEffectStart"` | Event-based (e.g., `"OnBeforeAttack"`) |
| `value` meaning | Accumulation amount (+100 ATK) | Logic parameter (damage multiplier, heal amount) |

---

# Condition System

### Active & Deactive Conditions

Conditions determine when effects activate and deactivate. They are string-based event names triggered by game logic:

```csharp
var effectInfo = new EffectInfo {
    type = "ATK_Constant",
    activeCondition = "OnBeforeAttack",  // activate when this condition fires
    deactiveCondition = "OnRoundEnd",     // deactivate when this condition fires
};

// In your game logic — trigger the condition for a specific owner
effectSystem.EffectTriggerCondition("OnBeforeAttack", character, conditionInfo);
```

When `EffectTriggerCondition()` is called, the system:
1. Finds all effects on the owner registered to that condition name
2. Executes their `OnActive` or `OnDeactive` callbacks via delegate multicast

### Built-in Conditions

| Condition | Description |
|-----------|-------------|
| `OnEffectStart` | Activates immediately when the effect is attached to an owner |
| `AfterActive` | Deactivates immediately after `OnActive()` executes (for one-shot triggers) |
| `OnEffectCooldownEnd` | Activates automatically when cooldown ends |

You can define any string as a condition name. Common patterns include:
- Combat: `OnBeforeAttack`, `OnAfterAttack`, `OnBeforeHit`, `OnAfterHit`
- Turn-based: `OnRoundStart`, `OnRoundEnd`, `OnActionStart`, `OnActionEnd`
- Real-time: `OnDash`, `OnReload`, `OnKill`, `OnHpFull`

### Internal Activation Flow

When `OnActive` is triggered:

```
OnActive(info)
  ├─ Check Sleep → if sleeping, remove Effect
  ├─ Check activeRequirementLists → all must be satisfied
  ├─ Check activeProbability → random roll
  ├─ Check cooldownTimer → skip if cooling down
  └─ ForceActive(info)
       ├─ Handle TriggerTransType
       │    ├─ CutOldOne → Deactive old, then activate new
       │    ├─ SkipNewOne → return immediately
       │    └─ KeepOldOneWithoutTimerReset → keep as-is
       ├─ isActive = true
       ├─ effectInstance.OnActive(info)
       ├─ Reset cooldownTimer
       └─ Check deactiveCondition
            ├─ "AfterActive" → immediate ForceDeactive
            └─ Otherwise → maintainTimeTimer.Start()
```

---

# Requirement Lists

`activeRequirement` and `deactiveRequirement` add constraints to condition triggers. The condition only fires if all requirements are met.

A requirement is defined by `ConditionRequirement`:

```csharp
var requirement = new ConditionRequirement {
    id = "isLowHealth",
    conditionParameter = "HP_Ratio",       // key for GetRuntimeValue()
    requirementLogic = ConditionLogic.Less, // comparison operator
    conditionValue = 30,                    // threshold
    isCheckOwner = true,                    // true: check owner, false: check anchor
};
```

Register a lookup delegate so the system can resolve requirement IDs:

```csharp
EffectInfo.GetActiveRequirementLists = (ids) =>
    activeRequirements.Where(m => ids.Contains(m.id)).ToList();

EffectInfo.GetDeactiveRequirementLists = (ids) =>
    deactiveRequirements.Where(m => ids.Contains(m.id)).ToList();
```

---

# Probability

`activeProbability` and `deactiveProbability` make activation/deactivation chance-based.

| Field | Description |
|-------|-------------|
| `activeProbability` | Probability (0~1) of activation succeeding. `0.4` = 40% chance. |
| `deactiveProbability` | Probability (0~1) of deactivation succeeding. |

When set to `0`, the probability check is skipped entirely (equivalent to 100%).

---

# Timer System

### Architecture

```
EffectSystem
  └─ Dictionary<string, TimerTicker> timerTickers
       ├─ "Default" (TimerTicker)      ← real-time (seconds)
       │    └─ List<IEffectTimer> timers
       ├─ "Action" (TimerTicker)       ← per-action tick
       │    └─ List<IEffectTimer> timers
       ├─ "Round" (TimerTicker)        ← per-round tick
       │    └─ List<IEffectTimer> timers
       └─ custom tickers as needed
```

### Time Units

Different games use different time units. EffectSystem doesn't assume any — you control when and how timers tick.

**Real-time games** (seconds):
```csharp
void Update()
{
    EffectSystem.Instance.TickEffectTimer(
        EffectSystemScriptableBuiltIn.TimerTickerId.Default, Time.deltaTime);
}
```

**Turn-based games** (rounds/actions):
```csharp
void OnRoundEnd()
{
    EffectSystem.Instance.TickEffectTimer("Round", 1);
}
void OnActionEnd()
{
    EffectSystem.Instance.TickEffectTimer("Action", 1);
}
```

### MaintainTime

If `maintainTime > 0`, the effect follows a time-based lifecycle. After activation, a countdown begins. When it reaches 0, the effect auto-deactivates.

```csharp
var buff = new EffectInfo {
    type = "ATK_Constant",
    value = 200,
    maintainTime = 10, // deactivates after 10 tick units
};

effectSystem.AddRequestedEffect(target, buff);
var sum = effectSystem.GetEffectSum(target, "ATK_Constant"); // 200

// After 10 ticks...
sum = effectSystem.GetEffectSum(target, "ATK_Constant"); // 0
```

### CooldownTime

After deactivation, `cooldownTime` prevents re-activation for a period:

```csharp
var healOnAttack = new EffectInfo {
    type = "HealSelf",
    value = 100,
    activeCondition = "OnAttack",
    deactiveCondition = "AfterActive",
    cooldownTime = 10,  // 10 ticks before it can trigger again
    logic = EffectLifeCycleLogic.ReactiveAfterCooldownEnd,
};
```

### Custom Timer Override

Override `maintainTimeTimerId` or `cooldownTimeTimerId` to route effects to different tickers:

```csharp
public class EffectInstanceCustom : EffectInstanceBase
{
    public override string maintainTimeTimerId =>
        info.GetParameterByKey("maintainTimeTimerType") != ""
            ? info.GetParameterByKey("maintainTimeTimerType")
            : "Action"; // default to per-action tick
}
```

This lets data configuration (via `parameters`) control which timer an effect uses.

---

# Pre-defined Enums

### TriggerTransType

Controls behavior when `OnActive()` is triggered while the effect is already active:

| Value | Description |
|-------|-------------|
| `SkipNewOne` | Ignore the repeated trigger |
| `CutOldOne` | Deactivate the current one, then immediately activate again (reset timer) |
| `KeepOldOneWithoutTimerReset` | Keep the current one, don't reset timer |

### EffectLifeCycleLogic

Preset lifecycle behaviors to reduce boilerplate:

| Value | Description |
|-------|-------------|
| `None` | After deactivation, enter Sleep state (no value, awaiting next condition) |
| `OnlyActiveOnce` | Activate once, then remove entirely |
| `ReactiveAfterCooldownEnd` | Automatically re-activate when cooldown ends |

---

# Manipulating Effects

### Adding Effects

```csharp
IEffectableObject target;
EffectSystem effectSystem;

// Add a single effect
effectSystem.AddRequestedEffect(target, effectInfo);

// Add multiple effects
effectSystem.AddRequestedEffects(target, new[] { effectInfo1, effectInfo2 });
```

The system checks `owner.IsAlive()` and `owner.ApprovedAddEffect()` before adding. It also enforces `maxEffectValue` and `countLimit`.

### Querying Effects

| Method | Description |
|--------|-------------|
| `GetEffectSum(owner, type)` | Sum of all active effect values of the given type |
| `GetEffectsByType(owner, type)` | List of effect instances of the given type |
| `GetEffectsByTag(owner, tag)` | List of effect instances with the given tag |

```csharp
var sumValue = effectSystem.GetEffectSum(player, "ATK_Constant");
var effects = effectSystem.GetEffectsByType(player, "ATK_Constant");
```

### Removing Effects

The system does **not** automatically recycle effect instances. Always clean up when effects are no longer needed.

| Method | Description |
|--------|-------------|
| `CleanEffectableObject(owner)` | Remove all effects from an owner |
| `RemoveEffectByTag(owner, tag)` | Remove all effects with a specific tag |
| `RemoveEffectsByType(owner, type)` | Remove all effects of a specific type |

```csharp
void OnGameEnd()
{
    foreach (var obj in GetEffectablesOnField())
    {
        effectSystem.CleanEffectableObject(obj);
    }
}
```

---

# RuntimeValue

Runtime values bridge EffectSystem and game logic. When effects need to reference live game state (e.g., current ATK for damage calculation), they use `IEffectableObject.GetRuntimeValue()`:

```csharp
public class MyCharacter : IEffectableObject
{
    public float GetRuntimeValue(string parameterKey)
    {
        return parameterKey switch
        {
            "CurrentATK" =>
                effectSystem.GetEffectSum(this, "ATK_Constant")
                * (1f + effectSystem.GetEffectSum(this, "ATK_Ratio")),
            "HP_Current" => currentHp,
            "DEF" => GetDef(),
            _ => 0,
        };
    }
}
```

This decouples Triggers from specific attribute calculation logic — they only need to know the key name.

---

# SubInfos — Effect Composition

`EffectInfo.subInfoIds` lets one effect reference and dynamically attach other effects:

```
┌──────────────────────┐
│   Trigger_Attach     │  ← Parent trigger
│   subInfoIds:        │
│    ├─ "IncreasedAtk" │  ← Sub-effect 1
│    └─ "Stunned"      │  ← Sub-effect 2
└──────────────────────┘
```

Register a lookup delegate for sub-effect resolution:

```csharp
EffectDataProvider.SetEffectInfoDelegate((ids) =>
    allEffects.Where(m => ids.Contains(m.id)).ToList());
```

Access sub-effects in your implementation:

```csharp
public class Effect_Trigger_Attach : EffectTriggerBase
{
    public Effect_Trigger_Attach(EffectSystem effectSystem) : base(effectSystem) { }

    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        effectSystem.AddRequestedEffects(owner, info.subInfos);
    }
}
```

SubInfos are lazily loaded via `EffectDataProvider.GetEffectInfo`, so sub-effect data can be maintained externally (ScriptableObject, database, JSON).

---

# Parameters

`EffectInfo.parameters` is a `Dictionary<string, string>` that provides maximum flexibility:

```csharp
string value = info.GetParameterByKey("damageType");
info.SetParameterByKey("triggerTarget", "Enemies");
```

Parameters let the same Effect class change behavior through data configuration. Common uses:

| Use Case | Example Key | Example Value |
|----------|-------------|---------------|
| Timer type selection | `maintainTimeTimerType` | `"Round"`, `"Action"` |
| Target selection | `triggerTarget` | `"Owner"`, `"Anchor"`, `"Targets"`, `"Enemies"` |
| Value source | `inputType` | `"ATK"`, `"HP_Max"` |
| Damage type | `damageType` | `"Physical"`, `"Magical"` |
| Hit count | `hit` | `"3"` |
| Effect type reference | `effectType` | `"Stunned"` |

---

# Tag System

Tags enable categorization and batch operations on effects.

### Tag Sources
1. **`EffectInfo.tags`** — configured in data
2. **`AddRequestedEffect` tags parameter** — added programmatically at attachment time

### Core API

```csharp
// Query by tag
var debuffs = effectSystem.GetEffectsByTag(owner, "Debuff");

// Batch removal by tag
effectSystem.RemoveEffectByTag(owner, "Debuff");

// Immunity via ApprovedAddEffect
public bool ApprovedAddEffect(EffectInfo info)
{
    if (hasCCImmunity && info.tags.Contains("CC"))
        return false;
    return true;
}
```

### Common Tag Patterns

| Tag | Purpose |
|-----|---------|
| `Buff` | Positive effects — can be removed by purge abilities |
| `Debuff` | Negative effects — can be cleansed |
| `CC` | Crowd control (stun, taunt) — can be immunized against |
| `Passive` | Passive abilities — typically not removable |
| `Unremovable` | Protected from removal abilities |

---

# Model Injection

EffectSystem provides a reflection-based model injection mechanism, allowing Triggers to access contextual information from the triggering event.

### Usage

Declare fields with `[EffectInstanceBaseInject]` in your Trigger class:

```csharp
public class Effect_Trigger_MyTrigger : EffectTriggerBase
{
    [EffectInstanceBaseInject]
    Skill currentSkill;  // Auto-injected from conditionInfo.models

    public Effect_Trigger_MyTrigger(EffectSystem effectSystem) : base(effectSystem) { }

    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        // currentSkill is already injected
        Debug.Log($"Skill: {currentSkill.name}");
    }
}
```

Pass models when triggering:

```csharp
effectSystem.EffectTriggerCondition("OnBeforeAttack", owner,
    new EffectTriggerConditionInfo(owner, anchor, targets, new object[] { skill }));
```

The system scans for `[EffectInstanceBaseInject]` fields/properties and injects matching types from the `models` array.

---

# Description Template System

EffectSystem includes a built-in description template system for generating UI text:

### Template Syntax

```
Deal {Effect_Trigger_HitConstant.value} damage
Lasts {Effect_Trigger_HitConstant.time} rounds
```

### Supported Paths

| Path | Description |
|------|-------------|
| `{Effect_XXX}` or `{Effect_XXX.value}` | Effect's value |
| `{Effect_XXX.time}` | maintainTime |
| `{Effect_XXX.cd}` | cooldownTime |
| `{Effect_XXX.activeProb}` | activeProbability |
| `{#myId}` | Reference by id (instead of type name) |
| `{Effect_XXX>subInfos>Effect_YYY.value}` | Deep reference to sub-effect value |
| `{Effect_XXX:%}` | Display as percentage (× 100 + %) |

---

# Advanced Patterns

### Multiple IEffectableObject Types

You can have different types of effect carriers in the same game:

```csharp
// Player-level effects (global buffs)
public class PlayerEffectData : MonoBehaviour, IEffectableObject { ... }

// Per-follower effects (local + inherits player effects)
public class FollowerEffectData : MonoBehaviour, IEffectableObject
{
    // Override GetEffectSum to combine local + global
    protected override float GetEffectSum(string type)
    {
        float local = EffectSystem.Instance.GetEffectSum(this, type);
        float global = EffectSystem.Instance.GetEffectSum(playerData, type);
        return local + global;
    }
}

// World-level effects (environmental modifiers)
public class WorldEffectData : MonoBehaviour, IEffectableObject { ... }
```

### Collider-Based Effect Application

Apply effects through Unity physics collisions (useful for traps, pickups, AoE):

```csharp
public class EffectApplyCollider : MonoBehaviour
{
    public ColliderUpdateType colliderUpdateType; // OnTriggerEnter or OnTriggerStay
    public float retriggerInterval = 1f;

    void CheckCollision(Collider collision)
    {
        if (collision.transform.parent.TryGetComponent(out IEffectableObject target))
        {
            List<EffectInfo> effectInfos = EffectDataProvider.GetEffectInfo(effectIds);
            EffectSystem.Instance.AddRequestedEffects(target, effectInfos);
        }
    }
}
```

### Conditional Sub-Effect Synchronization

Attach sub-effects when a condition is met, remove them when it's not:

```csharp
public class Effect_ConditionalAttach : EffectTriggerBase
{
    List<EffectInstanceBase> attachedEffects = new();

    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        var effects = EffectSystem.Instance.AddRequestedEffects(owner, info.subInfos);
        attachedEffects.AddRange(effects);
    }

    public override void OnDeactive(EffectTriggerConditionInfo info)
    {
        base.OnDeactive(info);
        foreach (var effect in attachedEffects)
            EffectSystem.Instance.RemoveEffect(effect.owner, effect);
        attachedEffects.Clear();
    }
}
```

Configuration: `activeCondition: "OnHpFull"`, `deactiveCondition: "HpNotFull"` — automatically toggles sub-effects based on HP state.

### Effect Code Auto-Generation

For projects with many simple (shell) effects, auto-generate effect classes from type definitions:

```csharp
public static void GenerateEffectFiles(string targetDirectory)
{
    var fields = typeof(EffectSystemScriptable.EffectType)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string));

    foreach (var field in fields)
    {
        string effectName = field.Name;
        string filePath = Path.Combine(targetDirectory, $"Effect_{effectName}.cs");
        if (File.Exists(filePath)) continue; // don't overwrite

        string content = $@"
using MacacaGames.EffectSystem;

public class Effect_{effectName} : EffectInstanceBase
{{
    public Effect_{effectName}(EffectSystem effectSystem) : base(effectSystem) {{ }}
}}";
        File.WriteAllText(filePath, content);
    }
}
```

### Server / Client Separation

Core EffectSystem code uses `#if !Server` to isolate client-only logic:

```csharp
public virtual void AddEffectView(EffectInstanceBase effect)
{
#if !Server
    // Only create visual effects on the client
    foreach (var viewInfo in effect.info.viewInfos)
    {
        EffectViewBase effectView = EffectSystem.Instance.RequestEffectView(...);
        effectViewList.Add(effectView);
    }
#endif
}
```

On the server, implement empty wrappers to satisfy interface requirements.

---

# Quick Start: Adding a New Effect

### Adding an Element (Passive Value)

1. Define the type name constant (e.g., `"MyNewStat"`)
2. Create the class:
   ```csharp
   public class Effect_MyNewStat : EffectInstanceBase
   {
       public Effect_MyNewStat(EffectSystem effectSystem) : base(effectSystem) { }
   }
   ```
3. Read the value in your game logic:
   ```csharp
   float bonus = effectSystem.GetEffectSum(character, "MyNewStat");
   ```
4. Optionally expose it via `GetRuntimeValue()` for other effects to reference

### Adding a Trigger (Active Behavior)

1. Create the class:
   ```csharp
   public class Effect_Trigger_MyAction : EffectTriggerBase
   {
       public Effect_Trigger_MyAction(EffectSystem effectSystem) : base(effectSystem) { }

       protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
       {
           // Your logic here
       }
   }
   ```
2. Configure `EffectInfo` with appropriate conditions and parameters

---

# Common Usage Scenarios

### Buff lasting 3 actions

```
EffectInfo:
  type: "IncreasedAtkConstant"
  value: 100
  activeCondition: "OnEffectStart"
  maintainTime: 3
  parameters:
    maintainTimeTimerType: "Action"
  tags: ["Buff"]
```

### 50% chance to deal extra damage on attack

```
EffectInfo:
  type: "Trigger_HitConstant"
  value: 50
  activeCondition: "OnBeforeAttack"
  deactiveCondition: "AfterActive"
  activeProbability: 0.5
  logic: ReactiveAfterCooldownEnd
  cooldownTime: 0
  parameters:
    triggerTarget: "Anchor"
    damageType: "Physical"
    hit: "1"
```

### DOT effect every round

```
EffectInfo:
  type: "Trigger_Burn"
  value: 20
  activeCondition: "OnRoundStart"
  deactiveCondition: "AfterActive"
  logic: ReactiveAfterCooldownEnd
  parameters:
    triggerTarget: "Enemies"
```

---

# FAQ

### Q: Effect attached but nothing happens?
Check:
- Is `activeCondition` correct? Is `EffectTriggerCondition()` being called externally?
- Is `activeProbability` set to 0? (Should be 1 for 100%, or 0 to skip the check)
- Does `ApprovedAddEffect()` return false? (Owner might have immunity)
- Is the effect in Sleep state?

### Q: Timer not counting down?
Check:
- Is the correct `TimerTicker` created via `AddTimerTicker()`?
- Is `TickEffectTimer()` being called at the right time?
- If using custom timer IDs via `parameters`, is the key name correct?

### Q: New Effect class not found by the system?
The system looks for `Effect_{type}` via reflection. Verify:
- Class name matches `Effect_{EffectInfo.type}` exactly
- Class inherits from `EffectInstanceBase` (or a subclass)
- Constructor accepts an `EffectSystem` parameter

### Q: How does Element value get read?
1. `GetEffectSum(owner, "TypeName")` iterates all same-type effects on the owner
2. For each effect, calls `GetValue()`
3. `GetValue()` checks `condition.isActive` — only returns value if active
4. All values are summed and returned

### Q: Difference between TriggerTransType values?
- `SkipNewOne`: Already active + new trigger → ignore new trigger
- `CutOldOne`: Deactivate old → activate new (timer resets)
- `KeepOldOneWithoutTimerReset`: Keep old, timer continues

### Q: How to make an effect provide value immediately without external trigger?
Set `activeCondition: "OnEffectStart"`. The effect activates as soon as it's attached, `isActive = true`, and `GetValue()` returns the value.

### Q: SubInfos vs. attaching multiple effects directly?
SubInfos are "dynamically attached by a Trigger at execution time." Advantages:
- Trigger logic can modify sub-effect values (e.g., apply buff/debuff modifiers)
- Creates a parent-child data relationship for management
- Description templates can reference sub-effect values

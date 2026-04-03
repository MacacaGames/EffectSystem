有關詳細資訊，請參閱[文檔](https://macacagames.github.io/EffectSystemDocs/)。

> [English version (README.md)](README.md)

# EffectSystem

EffectSystem 是一套適用於 Unity 與 .NET 的**資料驅動** Buff / Debuff / Trigger 框架。它將所有遊戲效果建模為 `EffectInfo` 資料，透過條件系統驅動啟動與失效，並以反射自動解析對應的實作類別。

無論你正在開發回合制 RPG、即時動作遊戲或 Roguelike，EffectSystem 都能提供統一的方式來管理數值修飾、計時 Buff、觸發型能力以及複合效果組合 — 全部透過資料配置，無需修改程式碼。

### 功能範例

- 增加 ATK 50 點。
- 增加 HP 10%。
- 增加 DEF 5%，持續 50 秒。
- 減少指定敵人的 ATK 100 點，每 30 秒可使用一次。
- 成功格擋時，減少對手 DEF 50%。
- 每回合觸發一次 DOT 效果。
- 每次攻擊有 40% 機率觸發治療。
- 技能啟動時對目標附加子效果。

### 核心特色

- **資料與邏輯分離** — 所有效果配置存放在 `EffectInfo` 結構體中（可透過 MessagePack、JSON 或 ScriptableObject 序列化）。調整數值不需改程式碼。
- **條件驅動** — Effect 不會自動生效，而是透過遊戲邏輯觸發命名條件（`activeCondition` / `deactiveCondition`）來驅動。
- **反射型別解析** — 將類別命名為 `Effect_{type}`，框架會自動找到對應類別，不需手動註冊。
- **物件池管理** — 所有 Effect 實例透過物件池管理，避免頻繁 GC。
- **彈性計時系統** — 透過可插拔的 `TimerTicker` 支援秒、回合、行動或任何自訂時間單位。
- **Excel / 資料表友善** — 在試算表中設計效果，匯出為資料，於執行時載入。
- **伺服器相容** — 核心邏輯可在 Unity 外執行，視覺相關程式碼以 `#if !Server` 隔離。

---

# 安裝

### 選項 1：Unity Package Manager

在專案的 `manifest.json` 中加入：
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

### 選項 2：Git SubModule

```bash
git submodule add https://github.com/MacacaGames/EffectSystem.git MyPackages
```

> EffectSystem 依賴 MacacaUtility，請一併加入：

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

# 伺服器端 .csproj 設定

若專案在伺服器端（Unity 外）使用 EffectSystem，需自行建立 `.csproj` 檔案（因為這些是專案特定的，不包含在本 Repository 中）。

在 EffectSystem submodule 目錄**外部**建立以下兩個檔案（例如同層父目錄）：

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
        <!-- 在此加入專案特定的參考 -->
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

> **注意：** 請根據這些檔案與 EffectSystem submodule 的相對位置調整 `HintPath`、`ProjectReference` 和 `Compile Include` 路徑。

---

# 架構總覽

```
┌─────────────────────────────────────────────────────┐
│                   Model 層（資料）                    │
│  EffectInfo, Enums, EffectDataProvider,              │
│  IEffectableObject, IEffectTimer                     │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│               Runtime 層（核心引擎）                   │
│  EffectSystem (Singleton)                            │
│  ├─ 物件池管理 (RequestEffect / RecoveryEffect)       │
│  ├─ 條件派發 (EffectTriggerCondition)                 │
│  ├─ 計時器管理 (TimerTicker)                          │
│  ├─ EffectInstanceBase (Effect 實例基底)               │
│  ├─ EffectTriggerBase (Trigger 實例基底)               │
│  ├─ EffectCondition (啟動/失效邏輯)                    │
│  └─ DefaultTimerBase (倒數計時器)                      │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│              Custom 層（專案實作）                      │
│  Effect_IncreasedAtkConstant  (Element)               │
│  Effect_Trigger_HitConstant   (Trigger)               │
│  Effect_Stunned               (Element)               │
│  ...你的自訂 Effect 實作                               │
└─────────────────────────────────────────────────────┘
```

| 層級 | 內容 | 可否修改 |
|------|------|---------|
| **Model** | 純資料結構、Enum、介面定義 | 不建議修改 |
| **Runtime** | EffectSystem 核心引擎 | 不建議修改 |
| **Custom** | 專案特定的 Effect 實作 | 自由擴充 |

---

# 基礎概念

### 初始化 EffectSystem

使用前需建立 `EffectSystem` 實例並呼叫 `Init()`：

```csharp
var effectSystem = new EffectSystem();
effectSystem.Init();
```

在 Unity 環境中，EffectSystem 預設使用單例模式。呼叫 `Init()` 後可透過 `EffectSystem.Instance` 取得實例。

在 Unity 外（如伺服器端），請使用自己的依賴注入方案管理 `EffectSystem` 實例。

### IEffectableObject

任何想要承載 Effect 的 C# 物件都必須實作 `IEffectableObject`。角色、敵人、卡牌、武器，甚至全域單例都可以作為效果承載者。

```csharp
public interface IEffectableObject
{
    /// 顯示名稱（除錯用）
    string GetDisplayName();

    /// 取得視覺效果的掛載 Transform
    Transform GetEffectViewParent(string viewRoot);

    /// 門禁檢查 — 回傳 false 以拒絕（如免疫）
    /// 應只包含檢查邏輯，不應有副作用
    bool ApprovedAddEffect(EffectInfo info);

    /// 當 Effect 實例被啟動時呼叫
    void OnEffectActive(EffectInfo info);

    /// 當 Effect 實例被失效時呼叫
    void OnEffectDeactive(EffectInfo info);

    /// 此物件是否仍然存活。Effect 只能附加到存活的物件上
    bool IsAlive();

    /// 專案自定義的即時數值查詢
    /// 例如 ATK_Current = ATK_Constant * ATK_Ratio
    float GetRuntimeValue(string parameterKey);

    /// 銷毀此 IEffectableObject
    void DestoryEffectableObject();
}
```

---

# EffectInfo — 核心資料結構

`EffectInfo` 是整個系統的資料核心。每個 Effect 由一個 `EffectInfo` 結構體完整描述：

```csharp
[MessagePack.MessagePackObject(true)]
[Serializable]
public partial struct EffectInfo
{
    public string id;                  // 選填唯一 ID（用於 Description 模板引用）
    public string type;                // Effect 類型名稱（對應 Effect_{type} 類別）
    public float value;                // 基礎數值
    public string activeCondition;     // 啟動條件名稱
    public List<string> activeRequirement;    // 啟動前額外條件
    public float activeProbability;    // 啟動機率（0~1）
    public string deactiveCondition;   // 失效條件名稱
    public List<string> deactiveRequirement;  // 失效前額外條件
    public float deactiveProbability;  // 失效機率（0~1）
    public float maintainTime;         // 持續時間
    public float cooldownTime;         // 冷卻時間
    public EffectLifeCycleLogic logic; // 生命週期邏輯預設
    public TriggerTransType triggerTransType; // 重複觸發行為
    public List<string> tags;          // 標籤（Buff、Debuff、CC 等）
    public List<string> subInfoIds;    // 子 Effect ID 列表
    public List<string> viewInfoIds;   // 視覺效果配置 ID 列表
    public Dictionary<string, string> parameters; // 自訂 key-value 參數
}
```

### 欄位說明

#### 基礎欄位

| 欄位 | 說明 | 範例 |
|------|------|------|
| `id` | 選填唯一 ID，用於 Description 模板引用 | `"#myBuff"` |
| `type` | Effect 類型名稱 — 系統尋找 `Effect_{type}` 類別 | `"ATK_Constant"` |
| `value` | 數值 — 意義由具體實作決定 | `100` |

#### 條件欄位

| 欄位 | 說明 | 範例 |
|------|------|------|
| `activeCondition` | 啟動事件名稱，由外部呼叫 `EffectTriggerCondition()` 觸發 | `"OnEffectStart"`、`"OnBeforeAttack"` |
| `deactiveCondition` | 失效事件名稱 | `"AfterActive"`、`"OnRoundEnd"` |
| `activeProbability` | 啟動機率（1 = 100%，0 = 跳過機率檢查） | `0.5` |
| `deactiveProbability` | 失效機率 | `1` |

#### 時間欄位

| 欄位 | 說明 | 範例 |
|------|------|------|
| `maintainTime` | 啟動後的持續時間（到時自動失效）。0 = 無時間生命週期 | `3`（3 個 tick 單位） |
| `cooldownTime` | 失效後的冷卻時間，冷卻期間無法重新啟動 | `2` |

#### 行為欄位

| 欄位 | 說明 |
|------|------|
| `logic` | 生命週期預設：`None`（失效後進入 Sleep）、`OnlyActiveOnce`（啟動一次後移除）、`ReactiveAfterCooldownEnd`（冷卻後自動重新啟動） |
| `triggerTransType` | 重複觸發行為：`SkipNewOne`（忽略）、`CutOldOne`（中斷舊的再啟動新的）、`KeepOldOneWithoutTimerReset`（保留舊的，不重置計時器） |

#### 組合欄位

| 欄位 | 說明 |
|------|------|
| `tags` | 標籤列表，用於分類（Buff/Debuff/CC）和批量操作 |
| `subInfoIds` | 子 Effect ID 列表，透過 `EffectDataProvider.GetEffectInfo` 延遲載入 |
| `parameters` | 任意 key-value 字串參數，提供彈性擴充 |

---

# Effect 生命週期

一個 Effect 從附加到移除的完整生命週期：

```
                     AddRequestedEffect()
                           │
                           ▼
              ┌─── RequestEffect()（從池中取出或新建）
              │            │
              │            ▼
              │      Reset(info) — 重設狀態
              │            │
              │            ▼
              │    ┌── Start() ──────────────────────────┐
              │    │   ├─ 建立 EffectCondition             │
              │    │   ├─ RegistEffectTriggerCondition()   │
              │    │   │   （將 OnActive/OnDeactive 註冊    │
              │    │   │    到 owner 的條件字典中）          │
              │    │   ├─ 加入 Timer 到 TimerTicker         │
              │    │   ├─ condition.Start()                │
              │    │   │   （若 activeCondition 為           │
              │    │   │    "OnEffectStart" 則立即啟動）     │
              │    │   └─ OnStart() callback               │
              │    └──────────────────────────────────────┘
              │                    │
              │    ┌───────────────▼───────────────────┐
              │    │  等待 ActiveCondition 被觸發         │
              │    │  （外部呼叫 EffectTriggerCondition） │
              │    └───────────────┬───────────────────┘
              │                    │
              │    ┌───────────────▼───────────────────┐
              │    │  OnActive() ─── 效果生效            │
              │    │   ├─ Model 注入（InjectModels）      │
              │    │   ├─ ExecuteActive()                │
              │    │   │   ├─ SetDirty(true)             │
              │    │   │   ├─ owner.OnEffectActive()     │
              │    │   │   └─ EffectViewOnActive()       │
              │    │   └─ [Trigger] OnTrigger() 執行效果  │
              │    └───────────────┬───────────────────┘
              │                    │
              │     ┌──────────────▼──────────────────┐
              │     │ OnDeactive() ─── 效果失效         │
              │     │  ├─ ExecuteDeactive()             │
              │     │  │   ├─ owner.OnEffectDeactive()  │
              │     │  │   └─ EffectViewOnDeactive()    │
              │     │  └─ LifeCycleLogic 判斷：          │
              │     │     ├─ None / OnlyActiveOnce      │
              │     │     │   → SetSleep()              │
              │     │     └─ ReactiveAfterCooldownEnd   │
              │     │         → cooldownTimer.Start()   │
              │     └─────────────────────────────────┘
              │                    │
              │    （可能循環回 Active）
              │                    │
              │     ┌──────────────▼──────────────────┐
              │     │  End() ─── 移除並回收             │
              │     │   ├─ 從 TimerTicker 移除計時器     │
              │     │   ├─ UnregistEffectTriggerCond.   │
              │     │   ├─ OnEnd() callback             │
              │     │   └─ RemoveEffectView()           │
              │     └──────────────┬──────────────────┘
              │                    │
              └── RecoveryEffectBase()（回收到池中）
```

### 關鍵概念

1. **Start ≠ Active**：`Start()` 表示 Effect 已附加到 owner 上，但不一定已生效。只有在 `activeCondition` 被觸發後才真正啟動。
2. **Sleep 機制**：當 `LifeCycleLogic` 為 `None` 或 `OnlyActiveOnce` 時，失效後進入 Sleep 狀態。Sleep 的 Effect 不提供數值（回傳 0），不參與計算。
3. **isActive 影響 GetValue()**：`GetValue()` 會檢查 `condition.isActive`，若為 false 則回傳 0。這是 Element 類 Effect 的核心機制。

---

# Effect 實例實作

### 型別解析

系統透過反射尋找與 `EffectInfo` type 匹配的類別。命名規則為 `Effect_{type}`：

```csharp
// EffectInfo.type = "ATK_Constant"  →  系統尋找 "Effect_ATK_Constant"
// EffectInfo.type = "IncreasedAtkConstant"  →  "Effect_IncreasedAtkConstant"
```

首次查找後結果會被快取。若找不到匹配的類別，系統會回退到 `EffectInstanceBase`。

### 基礎實作（Element）

一個僅提供數值的基本 Effect：

```csharp
public class Effect_ATK_Constant : EffectInstanceBase
{
    public Effect_ATK_Constant(EffectSystem effectSystem) : base(effectSystem) { }
}
```

就這樣。`EffectInfo` 中的 `value` 會自動透過 `GetEffectSum()` 累加。

### 進階實作

覆寫 `EffectInstanceBase` 的方法和屬性以自訂行為：

```csharp
public class Effect_MyEffect : EffectInstanceBase
{
    public Effect_MyEffect(EffectSystem effectSystem) : base(effectSystem) { }

    /// 當此類型的效果總值超過 maxEffectValue，新效果不會被加入
    public override float maxEffectValue => 100;

    /// GetEffectSum() 的上下限
    public override (float min, float max) sumLimit => (0f, 100f);

    /// 同一 owner 上此類型的最大堆疊數
    public override int countLimit => 5;

    protected override void OnStart() { /* 附加到 owner 時呼叫 */ }
    public override void OnActive(EffectTriggerConditionInfo triggerConditionInfo) { /* 效果啟動 */ }
    public override void OnDeactive(EffectTriggerConditionInfo triggerConditionInfo) { /* 效果失效 */ }
    public override void OnCooldownEnd() { /* 冷卻結束 */ }
    protected override void OnEnd() { /* 即將被回收 */ }
}
```

### Trigger 實作

Trigger 在啟動時執行邏輯（而非被動提供數值）。繼承 `EffectTriggerBase`：

```csharp
public class Effect_MyTrigger : EffectTriggerBase
{
    public Effect_MyTrigger(EffectSystem effectSystem) : base(effectSystem) { }

    // 在 OnActive() 之後立即執行
    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        // 你的觸發邏輯
    }
}
```

---

# Element 與 Trigger — 兩大 Effect 分類

EffectSystem 中的 Effect 分為兩大基本類別：

### Element（被動數值修飾）

Element **被動提供數值**。其 `value` 透過 `GetEffectSum()` 累加，由遊戲邏輯讀取：

```csharp
// 不需要自訂邏輯 — value 自動累加
public class Effect_IncreasedAtkConstant : EffectInstanceBase
{
    public Effect_IncreasedAtkConstant(EffectSystem effectSystem) : base(effectSystem) { }
}

// 遊戲邏輯讀取累加值
float atkBonus = effectSystem.GetEffectSum(character, "IncreasedAtkConstant");
int finalAtk = (int)(baseAtk * (1 + atkRatio) + atkBonus);
```

### Trigger（主動行為觸發）

Trigger 在啟動時**執行一次性邏輯**。通常搭配 `deactiveCondition: "AfterActive"` 在觸發後立即失效：

```csharp
public class Effect_Trigger_Heal : EffectTriggerBase
{
    public Effect_Trigger_Heal(EffectSystem effectSystem) : base(effectSystem) { }

    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        // 執行治療邏輯
    }
}
```

### 比較

| 面向 | Element | Trigger |
|------|---------|---------|
| 繼承 | `EffectInstanceBase` | `EffectTriggerBase` |
| 啟動後行為 | 持續提供數值 | 執行一次性邏輯 |
| 典型 `deactiveCondition` | 與 `activeCondition` 不同（如 `"OnRoundEnd"`） | `"AfterActive"` |
| 典型 `activeCondition` | `"OnEffectStart"` | 事件型（如 `"OnBeforeAttack"`） |
| `value` 意義 | 累加量（+100 攻擊力） | 邏輯參數（傷害倍率、治療量） |

---

# 條件系統

### Active 與 Deactive 條件

條件決定 Effect 何時啟動、何時失效。它們是字串型事件名稱，由遊戲邏輯觸發：

```csharp
var effectInfo = new EffectInfo {
    type = "ATK_Constant",
    activeCondition = "OnBeforeAttack",  // 此條件觸發時啟動
    deactiveCondition = "OnRoundEnd",     // 此條件觸發時失效
};

// 在遊戲邏輯中 — 對特定 owner 觸發條件
effectSystem.EffectTriggerCondition("OnBeforeAttack", character, conditionInfo);
```

當 `EffectTriggerCondition()` 被呼叫時，系統會：
1. 找到該 owner 上所有註冊到該條件名稱的 Effect
2. 透過 delegate 多播執行它們的 `OnActive` 或 `OnDeactive` 回調

### 內建條件

| 條件名稱 | 說明 |
|----------|------|
| `OnEffectStart` | Effect 附加時立即啟動 |
| `AfterActive` | `OnActive()` 執行後立即失效（用於一次性 Trigger） |
| `OnEffectCooldownEnd` | 冷卻結束時自動啟動 |

你可以定義任何字串作為條件名稱。常見模式包括：
- 戰鬥：`OnBeforeAttack`、`OnAfterAttack`、`OnBeforeHit`、`OnAfterHit`
- 回合制：`OnRoundStart`、`OnRoundEnd`、`OnActionStart`、`OnActionEnd`
- 即時：`OnDash`、`OnReload`、`OnKill`、`OnHpFull`

### 內部啟動流程

當 `OnActive` 被觸發時：

```
OnActive(info)
  ├─ 檢查 Sleep → 若為 Sleep 則移除 Effect
  ├─ 檢查 activeRequirementLists → 所有條件都必須滿足
  ├─ 檢查 activeProbability → 隨機機率判定
  ├─ 檢查 cooldownTimer → 若在冷卻中則跳過
  └─ ForceActive(info)
       ├─ 處理 TriggerTransType
       │    ├─ CutOldOne → 失效舊的，再啟動新的
       │    ├─ SkipNewOne → 直接返回
       │    └─ KeepOldOneWithoutTimerReset → 保留不變
       ├─ isActive = true
       ├─ effectInstance.OnActive(info)
       ├─ 重置 cooldownTimer
       └─ 判斷 deactiveCondition
            ├─ "AfterActive" → 立即 ForceDeactive
            └─ 其他 → maintainTimeTimer.Start()
```

---

# Requirement 條件檢查

`activeRequirement` 和 `deactiveRequirement` 為條件觸發增加額外約束。只有當所有 Requirement 都滿足時，條件才會生效。

Requirement 由 `ConditionRequirement` 定義：

```csharp
var requirement = new ConditionRequirement {
    id = "isLowHealth",
    conditionParameter = "HP_Ratio",       // GetRuntimeValue() 的 key
    requirementLogic = ConditionLogic.Less, // 比較運算子
    conditionValue = 30,                    // 門檻值
    isCheckOwner = true,                    // true: 檢查 owner, false: 檢查 anchor
};
```

註冊查詢委派讓系統能解析 Requirement ID：

```csharp
EffectInfo.GetActiveRequirementLists = (ids) =>
    activeRequirements.Where(m => ids.Contains(m.id)).ToList();

EffectInfo.GetDeactiveRequirementLists = (ids) =>
    deactiveRequirements.Where(m => ids.Contains(m.id)).ToList();
```

---

# 機率

`activeProbability` 和 `deactiveProbability` 使啟動/失效變為機率性。

| 欄位 | 說明 |
|------|------|
| `activeProbability` | 啟動成功的機率（0~1）。`0.4` = 40% 機率 |
| `deactiveProbability` | 失效成功的機率（0~1） |

當設為 `0` 時，機率檢查會被完全跳過（等同 100%）。

---

# 計時系統

### 架構

```
EffectSystem
  └─ Dictionary<string, TimerTicker> timerTickers
       ├─ "Default" (TimerTicker)      ← 即時（秒）
       │    └─ List<IEffectTimer> timers
       ├─ "Action" (TimerTicker)       ← 每次行動 tick
       │    └─ List<IEffectTimer> timers
       ├─ "Round" (TimerTicker)        ← 每回合 tick
       │    └─ List<IEffectTimer> timers
       └─ 依需求自訂更多 ticker
```

### 時間單位

不同遊戲使用不同的時間單位。EffectSystem 不假設任何單位 — 由你控制何時以及如何 tick 計時器。

**即時遊戲**（秒）：
```csharp
void Update()
{
    EffectSystem.Instance.TickEffectTimer(
        EffectSystemScriptableBuiltIn.TimerTickerId.Default, Time.deltaTime);
}
```

**回合制遊戲**（回合/行動）：
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

若 `maintainTime > 0`，Effect 會遵循時間生命週期。啟動後開始倒數，歸零時自動失效。

```csharp
var buff = new EffectInfo {
    type = "ATK_Constant",
    value = 200,
    maintainTime = 10, // 10 個 tick 單位後失效
};

effectSystem.AddRequestedEffect(target, buff);
var sum = effectSystem.GetEffectSum(target, "ATK_Constant"); // 200

// 10 個 tick 後...
sum = effectSystem.GetEffectSum(target, "ATK_Constant"); // 0
```

### CooldownTime

失效後，`cooldownTime` 會阻止重新啟動一段時間：

```csharp
var healOnAttack = new EffectInfo {
    type = "HealSelf",
    value = 100,
    activeCondition = "OnAttack",
    deactiveCondition = "AfterActive",
    cooldownTime = 10,  // 10 個 tick 後才能再次觸發
    logic = EffectLifeCycleLogic.ReactiveAfterCooldownEnd,
};
```

### 自訂 Timer 覆寫

覆寫 `maintainTimeTimerId` 或 `cooldownTimeTimerId` 以將 Effect 路由到不同的 ticker：

```csharp
public class EffectInstanceCustom : EffectInstanceBase
{
    public override string maintainTimeTimerId =>
        info.GetParameterByKey("maintainTimeTimerType") != ""
            ? info.GetParameterByKey("maintainTimeTimerType")
            : "Action"; // 預設為每行動 tick
}
```

這讓資料配置（透過 `parameters`）就能控制 Effect 使用哪種計時器。

---

# 預定義列舉

### TriggerTransType

控制 Effect 已啟動時，再次觸發 `OnActive()` 的行為：

| 值 | 說明 |
|----|------|
| `SkipNewOne` | 忽略重複觸發 |
| `CutOldOne` | 失效目前的，立即重新啟動（重置計時器） |
| `KeepOldOneWithoutTimerReset` | 保留目前的，不重置計時器 |

### EffectLifeCycleLogic

預設生命週期行為，減少樣板程式碼：

| 值 | 說明 |
|----|------|
| `None` | 失效後進入 Sleep 狀態（不提供數值，等待下次條件） |
| `OnlyActiveOnce` | 啟動一次後完全移除 |
| `ReactiveAfterCooldownEnd` | 冷卻結束後自動重新啟動 |

---

# 操作 Effect

### 新增 Effect

```csharp
IEffectableObject target;
EffectSystem effectSystem;

// 新增單一 Effect
effectSystem.AddRequestedEffect(target, effectInfo);

// 新增多個 Effect
effectSystem.AddRequestedEffects(target, new[] { effectInfo1, effectInfo2 });
```

系統會在新增前檢查 `owner.IsAlive()` 和 `owner.ApprovedAddEffect()`，並強制 `maxEffectValue` 和 `countLimit` 限制。

### 查詢 Effect

| 方法 | 說明 |
|------|------|
| `GetEffectSum(owner, type)` | 指定類型所有啟動中 Effect 的數值總和 |
| `GetEffectsByType(owner, type)` | 指定類型的 Effect 實例列表 |
| `GetEffectsByTag(owner, tag)` | 指定標籤的 Effect 實例列表 |

```csharp
var sumValue = effectSystem.GetEffectSum(player, "ATK_Constant");
var effects = effectSystem.GetEffectsByType(player, "ATK_Constant");
```

### 移除 Effect

系統**不會**自動回收 Effect 實例。請在不再需要時主動清理。

| 方法 | 說明 |
|------|------|
| `CleanEffectableObject(owner)` | 移除 owner 上的所有 Effect |
| `RemoveEffectByTag(owner, tag)` | 移除指定標籤的所有 Effect |
| `RemoveEffectsByType(owner, type)` | 移除指定類型的所有 Effect |

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

RuntimeValue 是 EffectSystem 與遊戲邏輯之間的橋樑。當 Effect 需要引用即時遊戲狀態（如當前 ATK 用於傷害計算）時，使用 `IEffectableObject.GetRuntimeValue()`：

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

這使 Trigger 完全不需要知道具體的屬性計算邏輯 — 只需要知道 key 名稱。

---

# SubInfos — 效果組合

`EffectInfo.subInfoIds` 讓一個 Effect 引用並動態附加其他 Effect：

```
┌──────────────────────┐
│   Trigger_Attach     │  ← 父 Trigger
│   subInfoIds:        │
│    ├─ "IncreasedAtk" │  ← 子 Effect 1
│    └─ "Stunned"      │  ← 子 Effect 2
└──────────────────────┘
```

註冊查詢委派以解析子 Effect：

```csharp
EffectDataProvider.SetEffectInfoDelegate((ids) =>
    allEffects.Where(m => ids.Contains(m.id)).ToList());
```

在實作中存取子 Effect：

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

SubInfos 透過 `EffectDataProvider.GetEffectInfo` 延遲載入，因此子 Effect 資料可以在外部維護（ScriptableObject、資料庫、JSON）。

---

# Parameters 參數包

`EffectInfo.parameters` 是一個 `Dictionary<string, string>`，提供最大彈性：

```csharp
string value = info.GetParameterByKey("damageType");
info.SetParameterByKey("triggerTarget", "Enemies");
```

Parameters 讓同一個 Effect 類別可以透過資料配置改變行為。常見用途：

| 用途 | 範例 Key | 範例 Value |
|------|----------|-----------|
| 計時器類型選擇 | `maintainTimeTimerType` | `"Round"`、`"Action"` |
| 目標選取 | `triggerTarget` | `"Owner"`、`"Anchor"`、`"Targets"`、`"Enemies"` |
| 數值來源 | `inputType` | `"ATK"`、`"HP_Max"` |
| 傷害類型 | `damageType` | `"Physical"`、`"Magical"` |
| 攻擊次數 | `hit` | `"3"` |
| Effect 類型引用 | `effectType` | `"Stunned"` |

---

# 標籤系統

標籤用於 Effect 的分類和批量操作。

### 標籤來源
1. **`EffectInfo.tags`** — 資料配置時設定
2. **`AddRequestedEffect` 的 tags 參數** — 附加時以程式碼額外添加

### 核心 API

```csharp
// 依標籤查詢
var debuffs = effectSystem.GetEffectsByTag(owner, "Debuff");

// 依標籤批量移除
effectSystem.RemoveEffectByTag(owner, "Debuff");

// 在 ApprovedAddEffect 中實現免疫
public bool ApprovedAddEffect(EffectInfo info)
{
    if (hasCCImmunity && info.tags.Contains("CC"))
        return false;
    return true;
}
```

### 常見標籤設計

| 標籤 | 用途 |
|------|------|
| `Buff` | 正面效果 — 可被淨化技能移除 |
| `Debuff` | 負面效果 — 可被清除 |
| `CC` | 控制效果（暈眩、嘲諷）— 可被免疫 |
| `Passive` | 被動能力 — 通常不可移除 |
| `Unremovable` | 受保護不被移除技能影響 |

---

# Model 注入系統

EffectSystem 提供一個基於反射的 Model 注入機制，讓 Trigger 可以存取觸發事件的上下文資訊。

### 使用方式

在 Trigger 類別中宣告帶有 `[EffectInstanceBaseInject]` 的欄位：

```csharp
public class Effect_Trigger_MyTrigger : EffectTriggerBase
{
    [EffectInstanceBaseInject]
    Skill currentSkill;  // 從 conditionInfo.models 自動注入

    public Effect_Trigger_MyTrigger(EffectSystem effectSystem) : base(effectSystem) { }

    protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
    {
        // currentSkill 已被自動注入
        Debug.Log($"Skill: {currentSkill.name}");
    }
}
```

觸發時傳入 models：

```csharp
effectSystem.EffectTriggerCondition("OnBeforeAttack", owner,
    new EffectTriggerConditionInfo(owner, anchor, targets, new object[] { skill }));
```

系統會掃描所有帶有 `[EffectInstanceBaseInject]` 的欄位/屬性，從 `models` 陣列中找到匹配型別的物件並注入。

---

# Description 描述模板系統

EffectSystem 內建描述文字模板系統，用於生成 UI 說明文字。

### 模板語法

```
造成 {Effect_Trigger_HitConstant.value} 點傷害
持續 {Effect_Trigger_HitConstant.time} 回合
```

### 支援的路徑

| 路徑 | 說明 |
|------|------|
| `{Effect_XXX}` 或 `{Effect_XXX.value}` | Effect 的 value |
| `{Effect_XXX.time}` | maintainTime |
| `{Effect_XXX.cd}` | cooldownTime |
| `{Effect_XXX.activeProb}` | activeProbability |
| `{#myId}` | 以 id 引用（而非類型名） |
| `{Effect_XXX>subInfos>Effect_YYY.value}` | 深層引用子 Effect 的數值 |
| `{Effect_XXX:%}` | 以百分比顯示（乘 100 加 %） |

---

# 進階模式

### 多種 IEffectableObject 類型

同一個遊戲中可以有不同類型的效果承載者：

```csharp
// 玩家層級效果（全域 Buff）
public class PlayerEffectData : MonoBehaviour, IEffectableObject { ... }

// 個別隨從效果（自身 + 繼承玩家效果）
public class FollowerEffectData : MonoBehaviour, IEffectableObject
{
    // 覆寫 GetEffectSum 以合併自身 + 全域效果
    protected override float GetEffectSum(string type)
    {
        float local = EffectSystem.Instance.GetEffectSum(this, type);
        float global = EffectSystem.Instance.GetEffectSum(playerData, type);
        return local + global;
    }
}

// 世界層級效果（環境修飾）
public class WorldEffectData : MonoBehaviour, IEffectableObject { ... }
```

### 碰撞器觸發效果

透過 Unity 物理碰撞施加效果（適用於陷阱、道具拾取、AoE）：

```csharp
public class EffectApplyCollider : MonoBehaviour
{
    public ColliderUpdateType colliderUpdateType; // OnTriggerEnter 或 OnTriggerStay
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

### 條件式子效果同步

條件成立時附加子效果，條件不成立時移除：

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

配置：`activeCondition: "OnHpFull"`、`deactiveCondition: "HpNotFull"` — 根據 HP 狀態自動切換子效果。

### Effect 程式碼自動生成

對於大量簡單（空殼）Effect 的專案，可從類型定義自動生成 Effect 類別：

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
        if (File.Exists(filePath)) continue; // 不覆蓋已存在的

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

### Server / Client 分離

EffectSystem 核心程式碼使用 `#if !Server` 隔離純客戶端邏輯：

```csharp
public virtual void AddEffectView(EffectInstanceBase effect)
{
#if !Server
    // 只在客戶端建立視覺效果
    foreach (var viewInfo in effect.info.viewInfos)
    {
        EffectViewBase effectView = EffectSystem.Instance.RequestEffectView(...);
        effectViewList.Add(effectView);
    }
#endif
}
```

伺服器端透過空實作滿足介面需求。

---

# 快速上手：新增 Effect

### 新增 Element（被動數值）

1. 定義類型名稱常數（如 `"MyNewStat"`）
2. 建立類別：
   ```csharp
   public class Effect_MyNewStat : EffectInstanceBase
   {
       public Effect_MyNewStat(EffectSystem effectSystem) : base(effectSystem) { }
   }
   ```
3. 在遊戲邏輯中讀取數值：
   ```csharp
   float bonus = effectSystem.GetEffectSum(character, "MyNewStat");
   ```
4. 選擇性地透過 `GetRuntimeValue()` 公開，供其他 Effect 引用

### 新增 Trigger（主動行為）

1. 建立類別：
   ```csharp
   public class Effect_Trigger_MyAction : EffectTriggerBase
   {
       public Effect_Trigger_MyAction(EffectSystem effectSystem) : base(effectSystem) { }

       protected override void OnTrigger(EffectTriggerConditionInfo conditionInfo)
       {
           // 你的邏輯
       }
   }
   ```
2. 配置 `EffectInfo`，設定適當的 Condition 和 Parameters

---

# 常見使用情境

### 持續 3 行動的 Buff

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

### 50% 機率在攻擊時造成額外傷害

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

### 每回合觸發 DOT 效果

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

# 常見問題 FAQ

### Q: Effect 附加了但看不到效果？
檢查：
- `activeCondition` 是否正確？是否有外部呼叫 `EffectTriggerCondition()` 觸發它？
- `activeProbability` 是否設為 0？（應為 1 表示 100%，或 0 以跳過檢查）
- `ApprovedAddEffect()` 是否回傳 false？（owner 可能有免疫）
- Effect 是否已進入 Sleep 狀態？

### Q: Timer 沒有倒數？
確認：
- 對應的 `TimerTicker` 是否已透過 `AddTimerTicker()` 建立？
- 是否在適當時機呼叫了 `TickEffectTimer()`？
- 若透過 `parameters` 使用自訂 timer ID，key 名稱是否正確？

### Q: 新建的 Effect 類別沒被系統找到？
系統透過反射搜尋 `Effect_{type}`。確認：
- 類別名稱完全匹配 `Effect_{EffectInfo.type}`
- 類別繼承自 `EffectInstanceBase`（或其子類別）
- 建構子接受 `EffectSystem` 參數

### Q: Element 的 value 如何被讀取？
1. `GetEffectSum(owner, "TypeName")` 遍歷該 owner 的所有同類型 Effect
2. 對每個 Effect 呼叫 `GetValue()`
3. `GetValue()` 檢查 `condition.isActive`，Active 才回傳值
4. 所有值累加後回傳

### Q: TriggerTransType 各值的差異？
- `SkipNewOne`：已啟動 + 新觸發 → 忽略新觸發
- `CutOldOne`：失效舊的 → 啟動新的（計時器重置）
- `KeepOldOneWithoutTimerReset`：保留舊的，計時器繼續

### Q: 如何讓 Effect 附加後立即提供數值，不需外部觸發？
設定 `activeCondition: "OnEffectStart"`。Effect 附加後立即啟動，`isActive = true`，`GetValue()` 就會回傳值。

### Q: SubInfos 和直接附加多個 Effect 有什麼差異？
SubInfos 是「由 Trigger 在執行時動態附加」的子 Effect。優勢：
- Trigger 邏輯可以修改子 Effect 的值（如套用 Buff/Debuff 修正）
- 資料上形成父子關係，便於管理
- Description 模板可以引用子 Effect 的數值

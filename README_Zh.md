有關詳細信息，請參閱[文檔](https://macacagames.github.io/EffectSystem/)。

# 概述
在表格上以EffectInfo為基本單位組合拼貼出各種增益、減益或技能效果，並且方便在不同專案間使用。工程師只需實作EffectType並註冊生效與失效的時機點。

---------
# 特點
- 透過 Excel 新增或調整技能
- 透過 EffectSubInfo 組合不同技能
---

# 安裝
### 選項 1 : 通過OpenUPM安裝 (推薦)

```sh
openupm add com.macacagames.effectsystem
```

### 選項 2 : Unity Package file
添加進編輯器的manifest.json:
```json
{
    "dependencies": {
        "com.macacagames.utility": "https://github.com/MacacaGames/MacacaUtility.git",
        "com.macacagames.effectsystem": "https://github.com/MacacaGames/EffectSystem.git"
    }
}
```

### 選項 3: Git SubModule
```bash
git submodule add https://github.com/MacacaGames/EffectSystem.git Assets/MacacaEffectSystem
```
Note: EffectSystem 依賴於 MacacaUtility，所以也要在 git submodule 添加 MacacaUtility。
```bash
git submodule add https://github.com/MacacaGames/MacacaUtility.git Assets/MacacaUtility
```
---
# 設置
- #### Excel 資料
    - [Effect 相關資料範例](https://docs.google.com/spreadsheets/d/1zYKiOlThAqTMVuUPHcxeQGX7rBRLp5E-49ci-GCZBa8/edit?usp=drive_link)
    - 依照不同需求自行增減各欄位的 Enum 內容
- #### 添加 SkillResource
    - AssetMenu 路徑 : _"GameResource/SkillResource"_
    - 添加技能圖示
    - 在 Bake All Effect Enum 貼上 Enums backe的Json資料，並按下Invoke
- #### 添加 EffectResource
    - AssetMenu 路徑 : _"GameResource/EffectResource"_
    - 貼上EffectView Json
    - 新增特效存放路徑，按下Get All View Prefab按鈕
---
# 用法
- #### 如果 Server 需要傳遞 EffectInfo 資料
```csharp

        StaticCompositeResolver.Instance.Register(
            MacacaGames.EffectSystem.Resolvers.EffectSystemResolver.Instance,
        );
```
- #### 實作 EffectType
    - 繼承 EffectBase 並實作所需要的效果

- #### 繼承並實作 IEffectSystemData
    - Effect 表格資料初始化
    - EffectInfo 查詢
    - 將實作的所有 EffectType 加入字典

- #### 繼承並實作 IEffectableObject
    - 用來添加技能的實體

- #### EffectManager
    - 附加 Effect 到 IEffectableObject 上或移除
    - 查詢 IEffectableObject 上的 Effect 實體
    - 搭配I2取得技能敘述
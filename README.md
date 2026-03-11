# Olden Ring - Unity + NGO 多人动作游戏

一款基于 Unity 和 Netcode for GameObjects (NGO) 开发的类魂系多人动作游戏项目。

## 项目简介

本项目是一个类似《艾尔登法环》风格的多人动作游戏原型，使用 Unity 引擎开发，采用 Netcode for GameObjects (NGO) 实现多人联机功能。项目包含完整的角色控制系统、战斗系统、动画系统、AI系统和存档系统。

## 技术栈

- **游戏引擎**: Unity 6000+
- **网络框架**: Netcode for GameObjects (NGO)
- **输入系统**: Unity Input System
- **渲染管线**: URP (Universal Render Pipeline)
- **AI导航**: Unity NavMesh

## 核心功能

### 角色系统

- 完整的角色移动控制
  - 行走、奔跑、冲刺
  - 跳跃与闪避翻滚
  - 基于方向的角色受击动画（前、后、左、右四个方向）
- 角色动画管理（移动、攻击、受击、死亡）
- 体力系统（耐力消耗与恢复）
- 属性系统（生命力、耐力等）

### 战斗系统

- 近战武器系统
  - 武器碰撞检测
  - 轻攻击与重攻击动作
  - 武器装备管理
- 伤害计算与效果系统
- 多种伤害类型（物理、火焰、魔法、闪电、神圣）
- 姿态（Poise）系统
- 锁定目标系统

### AI系统

- 基于状态机的AI行为控制
  - 空闲状态 (Idle State)
  - 追踪目标状态 (Pursue Target State)
- AI角色网络同步
- NavMesh导航集成

### 网络功能

- 基于 NGO 的多人联机
- 玩家数据同步
  - 位置与旋转插值同步
  - 移动输入同步
  - 动画状态同步
- 网络变量 (NetworkVariable) 数据管理
- 客户端连接管理

### 存档系统

- 角色数据保存/加载
- 多存档槽位支持（10个角色槽位）
- 场景状态管理

### UI系统

- HUD界面管理
- 状态条显示（生命值、耐力条）
- 弹窗管理系统
- 主菜单与角色选择界面

## 项目结构

```
Assets/
├── Scripts/
│   ├── Character/              # 角色相关脚本
│   │   ├── CharacterManager.cs        # 角色基础管理类
│   │   ├── CharacterNetworkManager.cs # 角色网络同步
│   │   ├── CharacterLocomotionManager.cs # 角色移动管理
│   │   ├── CharacterAnimatorManager.cs # 动画状态管理
│   │   ├── CharacterCombatManager.cs   # 战斗管理
│   │   ├── CharacterEffectsManager.cs  # 效果管理
│   │   ├── CharacterStatsManager.cs    # 属性管理
│   │   ├── Player/                     # 玩家专属脚本
│   │   │   ├── PlayerManager.cs        # 玩家角色管理
│   │   │   ├── PlayerLocomotionManager.cs # 玩家移动控制
│   │   │   ├── PlayerCombatManager.cs  # 玩家战斗管理
│   │   │   ├── PlayerInputManager.cs   # 玩家输入管理
│   │   │   ├── PlayerCamera.cs         # 玩家摄像机控制
│   │   │   ├── PlayerNetworkManager.cs # 玩家网络同步
│   │   │   ├── PlayerStatsManager.cs   # 玩家属性管理
│   │   │   ├── PlayerInventoryManager.cs # 玩家背包管理
│   │   │   ├── PlayerEquipmentManager.cs # 玩家装备管理
│   │   │   └── PlayerUI/               # 玩家UI管理
│   │   │       ├── PlayerUIManager.cs
│   │   │       ├── PlayerUIHudManager.cs
│   │   │       └── PlayerUIPopUpManager.cs
│   │   └── AI Character/              # AI角色脚本
│   │       ├── AICharacterManager.cs   # AI角色管理
│   │       ├── AICharacterCombatManager.cs # AI战斗管理
│   │       ├── AICharacterLocomotionManager.cs # AI移动管理
│   │       ├── AIIdleState.cs          # 空闲状态
│   │       ├── AIPursueTargetState.cs  # 追踪状态
│   │       └── AIState.cs              # 状态基类
│   ├── Items/                  # 物品系统
│   │   ├── Item.cs                    # 物品基类
│   │   ├── WeaponItem.cs              # 武器物品
│   │   ├── MeleeWeaponItem.cs         # 近战武器
│   │   ├── WeaponManager.cs           # 武器管理器
│   │   ├── WeaponModelInstantiationSlot.cs # 武器模型槽位
│   │   ├── WorldItemDatabase.cs       # 世界物品数据库
│   │   └── WeaponActions/              # 武器动作
│   │       ├── WeaponItemAction.cs    # 武器动作基类
│   │       ├── LightAttackWeaponItemAction.cs # 轻攻击
│   │       └── HeavyAttackWeaponItemAction.cs # 重攻击
│   ├── Colliders/              # 碰撞器
│   │   ├── DamageCollider.cs         # 伤害碰撞器
│   │   └── MeleeWeaponDamageCollider.cs # 近战武器伤害碰撞
│   ├── Effects/                # 效果系统
│   │   ├── InstantCharacterEffect.cs  # 即时效果基类
│   │   ├── TakeDamageEffect.cs        # 受击效果
│   │   └── TakeStaminaDamageEffect.cs # 体力伤害效果
│   ├── Animator/               # 动画相关
│   │   ├── ResetActionFlag.cs        # 重置动作标记
│   │   ├── ResetisJumping.cs         # 重置跳跃状态
│   │   └── ToggleAttackType.cs       # 切换攻击类型
│   ├── WorldManagers/          # 世界管理器
│   │   ├── WorldSaveGameManager.cs    # 存档管理
│   │   ├── WorldCharacterEffectsManager.cs # 效果管理
│   │   ├── WorldSoundFXManager.cs     # 音效管理
│   │   ├── WorldAIManager.cs          # AI管理
│   │   ├── WorldActionManager.cs      # 动作管理
│   │   ├── WorldGameSessionManager.cs # 游戏会话管理
│   │   └── WorldUtilityManager.cs     # 工具管理
│   ├── MenuScene/              # 主菜单场景
│   │   ├── TitleScreenManager.cs      # 标题画面管理
│   │   └── TitleScreenLoadmanager.cs # 场景加载管理
│   ├── Save & Load/            # 存档相关
│   │   ├── CharacterSaveData.cs       # 角色存档数据
│   │   └── SaveFileDataWriter.cs      # 存档文件写入
│   ├── UI/                     # UI相关
│   │   ├── UI_Character_Save_Slot.cs  # 存档槽位UI
│   │   └── UI_Match_Scroll_Wheel_To_Selected_Button.cs # UI滚动匹配
│   └── Enums.cs               # 枚举定义
├── Prefabs/                    # 预制件
│   ├── Player.prefab                  # 玩家预制件
│   ├── Player Camera.prefab           # 玩家摄像机
│   ├── Player Input Manager.prefab    # 输入管理器
│   ├── Player UI Manager.prefab       # UI管理器
│   ├── Items/Weapons/                 # 武器预制件
│   │   ├── Weapons_Broad_Sword_Blue_01.prefab
│   │   ├── Weapons_Straight_Sword_Red_01.prefab
│   │   ├── Weapons_Straight_Sword_Small_01.prefab
│   │   ├── Weapons_Straight_Sword_Standard_01.prefab
│   │   └── Weapons_Unarmed_01.prefab
│   ├── VFX/                           # 特效预制件
│   │   ├── FX_BloodSplatter_Red_01.prefab
│   │   └── FX_BloodSplatter_Red_02.prefab
│   ├── UI/                            # UI预制件
│   └── WorldManagers/                 # 世界管理器预制件
│       ├── World Actions Manager.prefab
│       ├── World AI Manager.prefab
│       ├── World Character Effects Manager.prefab
│       ├── World Item Database.prefab
│       ├── World Network Manager.prefab
│       ├── World Save Game Manager.prefab
│       ├── World Sound FX Manager.prefab
│       └── World Utility Manager.prefab
├── Resources/                  # 资源文件
│   ├── Animation/             # 动画资源
│   │   └── Humanoid/         # 人形动画
│   │       ├── _Core/Locomotion/  # 基础移动动画
│   │       │   ├── idle, walk, run, sprint 各方向
│   │       └── Damage/       # 受击动画
│   ├── Audio/                 # 音频资源
│   ├── Materials/             # 材质
│   ├── Models/                # 模型
│   ├── Textures/              # 纹理
│   └── UI/                    # UI资源
├── Data/                       # 配置数据
│   ├── AnimatorControllers/    # 动画控制器
│   │   ├── AI_UnDead.controller
│   │   └── Humanoid.controller
│   ├── Effects/                # 效果配置
│   │   └── Instant Effects/
│   │       ├── Take Damage Effect.asset
│   │       └── Take Stamina Damage Effect.asset
│   ├── AI States/              # AI状态配置
│   │   ├── Idle State.asset
│   │   └── Pursue Target State.asset
│   └── Items/Weapons/          # 武器配置
│       └── Melee Weapons/
│           ├── Broad Sword.asset
│           ├── Straight Sword (Test).asset
│           └── Unarmed.asset
└── ParrelSync/                 # ParrelSync 多开工具
```

## 开发工具

### ParrelSync

项目集成了 ParrelSync，支持多开 Unity 编辑器进行多人游戏测试，无需构建即可测试网络功能。

## 主要脚本说明

### 角色管理

| 脚本 | 说明 |
|------|------|
| [`CharacterManager.cs`](Assets/Scripts/Character/CharacterManager.cs) | 角色基础管理类，处理网络同步、状态管理 |
| [`PlayerManager.cs`](Assets/Scripts/Character/Player/PlayerManager.cs) | 玩家角色管理，继承自 CharacterManager |
| [`CharacterNetworkManager.cs`](Assets/Scripts/Character/CharacterNetworkManager.cs) | 角色网络同步，处理位置、旋转、动画状态同步 |
| [`CharacterLocomotionManager.cs`](Assets/Scripts/Character/CharacterLocomotionManager.cs) | 角色移动基础逻辑 |

### 战斗系统

| 脚本 | 说明 |
|------|------|
| [`MeleeWeaponDamageCollider.cs`](Assets/Scripts/Colliders/MeleeWeaponDamageCollider.cs) | 近战武器伤害碰撞检测 |
| [`TakeDamageEffect.cs`](Assets/Scripts/Effects/TakeDamageEffect.cs) | 受击效果处理 |
| [`WeaponItemAction.cs`](Assets/Scripts/Items/WeaponActions/WeaponItemAction.cs) | 武器动作基类 |
| [`LightAttackWeaponItemAction.cs`](Assets/Scripts/Items/WeaponActions/LightAttackWeaponItemAction.cs) | 轻攻击动作 |
| [`HeavyAttackWeaponItemAction.cs`](Assets/Scripts/Items/WeaponActions/HeavyAttackWeaponItemAction.cs) | 重攻击动作 |

### AI系统

| 脚本 | 说明 |
|------|------|
| [`AICharacterManager.cs`](Assets/Scripts/Character/AI Character/AICharacterManager.cs) | AI角色管理，状态机控制 |
| [`AIState.cs`](Assets/Scripts/Character/AI Character/AIState.cs) | AI状态基类 |
| [`AIIdleState.cs`](Assets/Scripts/Character/AI Character/AIIdleState.cs) | 空闲状态 |
| [`AIPursueTargetState.cs`](Assets/Scripts/Character/AI Character/AIPursueTargetState.cs) | 追踪目标状态 |

### 输入控制

| 脚本 | 说明 |
|------|------|
| [`PlayerInputManager.cs`](Assets/Scripts/Character/Player/PlayerInputManager.cs) | 玩家输入管理，处理键盘手柄输入 |
| [`PlayerControls.cs`](Assets/PlayerControls.cs) | Unity Input System 输入映射 |
| [`PlayerCamera.cs`](Assets/Scripts/Character/Player/PlayerCamera.cs) | 玩家摄像机控制 |

### 存档系统

| 脚本 | 说明 |
|------|------|
| [`WorldSaveGameManager.cs`](Assets/Scripts/WorldManagers/WorldSaveGameManager.cs) | 世界存档管理器 |
| [`CharacterSaveData.cs`](Assets/Scripts/Save & Load/CharacterSaveData.cs) | 角色存档数据结构 |
| [`SaveFileDataWriter.cs`](Assets/Scripts/Save & Load/SaveFileDataWriter.cs) | 存档文件读写 |

## 如何运行

### 环境要求

- Unity 6000+
- URP 渲染管线

### 运行步骤

1. 克隆项目到本地
   ```bash
   git clone <repository-url>
   ```
2. 使用 Unity 6000+ 打开项目
3. 打开主场景
4. 运行游戏

### 多人测试

使用 ParrelSync 创建克隆项目：
1. 打开菜单 `ParrelSync > Clones Manager`
2. 点击 `Create new clone`
3. 打开克隆项目
4. 两个编辑器同时运行进行多人联机测试

## 网络架构

项目使用 Unity Netcode for GameObjects (NGO) 实现多人联机：

- **网络变量 (NetworkVariable)**: 用于同步角色位置、旋转、动画状态等
- **RPC**: 用于客户端与服务器之间的通信
- **网络行为 (NetworkBehaviour)**: 所有网络对象的基础类
- **对象池**: 用于网络对象的实例化与回收

## 动画系统

使用 Animator Controller 配合动画状态机：

- 移动动画混合树（8方向移动）
- 攻击动画序列
- 受击动画（4方向）
- 跳跃与翻滚动画
- 动画事件触发

## 开发状态

项目处于开发阶段，持续更新中。

## 许可证

本项目仅供学习交流使用。

项目用到的所有素材仅供学习使用。

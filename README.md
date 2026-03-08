# Olden Ring - Unity + NGO 多人动作游戏

一款基于 Unity 和 Netcode for GameObjects (NGO) 开发的类魂系多人动作游戏项目。

## 项目简介

本项目是一个类似《艾尔登法环》风格的多人动作游戏原型，使用 Unity 引擎开发，采用 Netcode for GameObjects (NGO) 实现多人联机功能。项目包含完整的角色控制系统、战斗系统、动画系统和存档系统。

## 技术栈

- **游戏引擎**: Unity 6000+
- **网络框架**: Netcode for GameObjects (NGO)
- **输入系统**: Unity Input System
- **渲染管线**: URP (Universal Render Pipeline)

## 核心功能

### 角色系统
- 完整的角色移动控制（行走、奔跑、冲刺、翻滚）
- 角色动画管理（移动、攻击、受击、死亡）
- 基于方向的角色受击动画（前、后、左、右四个方向）

### 战斗系统
- 近战武器系统
- 武器碰撞检测
- 伤害计算与效果系统
- 多种伤害类型（物理、火焰、魔法、闪电、神圣）
- 姿态（Poise）系统

### 网络功能
- 基于 NGO 的多人联机
- 玩家数据同步
- 网络动画同步

### 存档系统
- 角色数据保存/加载
- 多存档槽位支持

## 项目结构

```
Assets/
├── Scripts/
│   ├── Character/              # 角色相关脚本
│   │   ├── Player/             # 玩家专属脚本
│   │   │   ├── PlayerManager.cs
│   │   │   ├── PlayerLocomotionManager.cs
│   │   │   ├── PlayerCombatManager.cs
│   │   │   ├── PlayerInputManager.cs
│   │   │   └── ...
│   │   ├── CharacterManager.cs
│   │   ├── CharacterAnimatorManager.cs
│   │   ├── CharacterCombatManager.cs
│   │   └── ...
│   ├── Items/                  # 物品系统
│   │   ├── WeaponItem.cs
│   │   ├── MeleeWeaponItem.cs
│   │   ├── WeaponActions/      # 武器动作
│   │   └── ...
│   ├── Colliders/              # 碰撞器
│   │   ├── DamageCollider.cs
│   │   └── MeleeWeaponDamageCollider.cs
│   ├── Effects/                # 效果系统
│   │   ├── TakeDamageEffect.cs
│   │   └── TakeStaminaDamageEffect.cs
│   ├── WorldManagers/          # 世界管理器
│   │   ├── WorldSaveGameManager.cs
│   │   ├── WorldCharacterEffectsManager.cs
│   │   └── WorldSoundFXManager.cs
│   └── MenuScene/             # 主菜单场景
├── Prefabs/                    # 预制件
│   ├── Player.prefab
│   ├── Items/Weapons/
│   ├── VFX/
│   └── WorldManagers/
├── Resources/                  # 资源文件
│   ├── Animation/             # 动画资源
│   ├── Audio/                 # 音频资源
│   └── ...
└── Data/                       # 配置数据
    ├── AnimatorControllers/
    ├── Effects/
    └── Items/Weapons/
```

## 开发工具

### ParrelSync
项目集成了 ParrelSync，支持多开 Unity 编辑器进行多人游戏测试，无需构建即可测试网络功能。

## 主要脚本说明

### 角色管理
- [`CharacterManager.cs`](Assets/Scripts/Character/CharacterManager.cs) - 角色基础管理类
- [`PlayerManager.cs`](Assets/Scripts/Character/Player/PlayerManager.cs) - 玩家角色管理
- [`CharacterAnimatorManager.cs`](Assets/Scripts/Character/CharacterAnimatorManager.cs) - 动画状态管理

### 战斗系统
- [`MeleeWeaponDamageCollider.cs`](Assets/Scripts/Colliders/MeleeWeaponDamageCollider.cs) - 近战武器伤害碰撞检测
- [`TakeDamageEffect.cs`](Assets/Scripts/Effects/TakeDamageEffect.cs) - 受击效果处理
- [`WeaponItemAction.cs`](Assets/Scripts/Items/WeaponActions/WeaponItemAction.cs) - 武器动作基类

### 输入控制
- [`PlayerInputManager.cs`](Assets/Scripts/Character/Player/PlayerInputManager.cs) - 玩家输入管理
- [`PlayerControls.cs`](Assets/PlayerControls.cs) - Unity Input System 输入映射

## 如何运行

1. 克隆项目到本地
2. 使用 Unity 6000+ 打开项目
3. 打开主场景
4. 运行游戏

### 多人测试
使用 ParrelSync 创建克隆项目，可同时运行多个编辑器实例进行多人联机测试。

## 开发状态

项目处于开发阶段，持续更新中。

## 许可证

本项目仅供学习交流使用。
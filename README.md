# 🧟‍♂️ Zombie Survival FPS Prototype

> 一个基于 Unity 3D 的第一人称僵尸生存射击游戏原型。
> 实现了完整的游戏循环 (Game Loop)、武器系统框架与本地数据持久化功能。

![Gameplay Demo](这里放你的GIF动图链接_必须放.gif)
*(建议：放一个由射击、换弹、被追击、结算界面组成的 15秒 快速剪辑动图)*

## 🎮 项目简介 (Introduction)
本项目旨在验证基础 FPS 游戏的核心架构实现。不同于单纯的 Demo，本项目构建了一个包含**主菜单 -> 战斗循环 -> 胜负判定 -> 数据结算**的完整闭环。
重点展示了基于 **Raycast (射线检测)** 的射击系统、**NavMesh** 驱动的僵尸 AI 以及基于 **JSON** 的排行榜系统。

## 🛠️ 技术亮点 (Technical Highlights)

### 1. 🔫 模块化武器系统 (Weapon System)
* **核心类**: `WeaponBase.cs`, `Pistol.cs`, `Sniper.cs`
* **面向对象设计**: 使用 **继承与多态** 设计武器基类，统一管理 `Fire()`, `Reload()` 接口，方便快速扩展新武器（如霰弹枪、步枪）。
* **射击机制**: 实现了基于 **Raycast** 的即时命中判定，并配合协程 (Coroutine) 实现了后坐力反馈与换弹冷却逻辑。

### 2. 🧠 僵尸 AI 状态机 (Zombie AI FSM)
* **核心类**: `ZombieController.cs`, `ZombieManager.cs`
* **有限状态机 (FSM)**: 实现了 `Patrol` (巡逻), `Chase` (追击), `Attack` (攻击) 三种状态的自动切换。
* **感知系统**: 结合 **NavMeshAgent** 寻路与距离判定，僵尸能够听声辨位并绕过障碍物追击玩家。

### 3. 💾 数据持久化 (Data Persistence)
* **核心类**: `LeaderboardManager.cs`
* **序列化**: 使用 `JsonUtility` (或 Newtonsoft.Json) 将玩家的得分数据序列化为 JSON 格式保存至本地。
* **排行榜**: 实现了数据的读取、排序 (LINQ) 与 UI 动态生成，展示 Top 10 高分记录。

### 4. 🕹️ 游戏流程控制 (Game Loop)
* 实现了 `GameManager` 单例，管理游戏倒计时、生成波次 (Wave System) 以及 UI 状态的广播。

## 🕹️ 操作说明 (Controls)
* **WASD**: 移动 (Movement)
* **Mouse**: 视角旋转 (Look)
* **LMB (左键)**: 射击 (Fire)
* **R (按键)**: 换弹 (Reload)
* **Shift**: 冲刺 (Sprint)

## 📂 目录结构 (Directory Structure)
```text
Scripts/
├── Weapon/          # 武器基类与派生类 (体现 OOP)
├── Zombie/          # 僵尸行为逻辑与 FSM
├── Leaderboard/     # 数据读写与排行榜逻辑
├── Manager/         # 全局游戏管理器
└── UI/              # 界面交互逻辑

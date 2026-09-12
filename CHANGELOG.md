# Changelog

All notable changes to **ROGUELIKE FPS SURVIVAL** will be documented in this file.

---

## [Alpha 0.4.0] - 2026-09-11

### Added
- **Active Capsule Loadout System**:
  - Main Menu Loadout panel (`LoadoutPanelUI`) allowing players to configure and equip active ability capsules before starting a match.
  - Dedicated `LoadoutManager` integrating equipped capsule skills directly with `PlayerSkillsManager`.
  - UI components for loadout management: `CapsuleCardUI`, `CapsuleListItemUI`, `CapsuleSlotUI`, and `CapsuleStatsTooltipUI`.
  - Dedicated "Capsule" input action bound to **Q** key in `PlayerInputActions`.
  - In-game dynamic cooldown HUD visual feedback for equipped capsules.
- **Flying Enemy Archetype**:
  - Added new aerial enemy type (`FlyingEnemy.prefab` and `FlyingEnemy.asset`) with custom pink material.
  - Custom flight locomotion (`EnemyFlyingFollow`) featuring hover elevation, vertical sine-wave bobbing, and player tracking.
  - Scaling spawn curve weights for flying enemies over time.
- **Enemy Status Effect Mutations**:
  - Procedural enemy mutations spawning dynamically after 60s of elapsed survival time, with scaling chance.
  - 4 distinct enemy mutation variants:
    - *Damage*: High damage variant with red visual tint.
    - *Slowness*: Inflicts movement slow on contact (`ISlowable`) with cyan visual tint.
    - *Weakness*: Inflicts vulnerability / increased damage taken on contact (`IVulnerable`) with purple visual tint.
    - *Stun*: Inflicts temporary stun on contact (`IStuneable`) with gold/yellow visual tint.
  - `EnemyEffectVisuals` providing distinct dynamic color feedback for mutated enemies.
  - Player controller support for slow, stun, and damage vulnerability effects.
- **Items & Stats**:
  - **Magic Aura Amulet**: Added to `AmuletsDatabaseSO`, granting cooldown reduction on abilities (`CooldownMultiplier`).
- **UI & HUD**:
  - Added numerical level label ("Lvl. {0}") and XP progress text ("current/max") to the player experience bar (`PlayerExperienceUI`).
  - Added visual selection state and green highlight feedback for upgrade cards in `UpgradeCardUI` and `UpgradeManagerUI`.

### Changed
- **Shooting Quality of Life**: Triggering fire while weapon magazine is empty now automatically initiates weapon reload (`PlayerWeaponManager`).
- **Combat Feedback**: Floating damage popups for Critical Hits now render with distinct vivid red text.
- **Capsule Balancing**: Standardized all capsule abilities (Blind, Damage, Health, Pull, Push, Silence, Slow, Stun, Vulnerability) to 5-second base cooldowns and updated in-game descriptions.
- **Enemy Spawner**: Enhanced spawn rates, weight curves, and cluster scaling over match progression.

---

## [Alpha 0.3.0] - 2026-09-02

### Added
- **Dynamic Amulet & Item System**:
  - Centralized `AmuletsDatabaseSO` and `AmuletSO` definitions.
  - Runtime `AmuletCreator` generator supporting weighted rarities and procedural stat modifications.
  - `PlayerAmuletManager` component handling dynamic inventory, passive bonuses, and live stat recalculation.
  - **14 Unique Collectable Amulets / Items**:
    - *Berserker Amulet*
    - *Guardian Amulet*
    - *Swiftness Amulet*
    - *Vampire Amulet*
    - *Vitality Amulet*
    - *Bigger Grenades*
    - *Bionic Eye*
    - *Faster Grenade Bag*
    - *Grenade Maniac*
    - *Rocket Boots*
    - *Stone Armor*
    - *Tactic Gloves*
    - *Demon Drainer*
    - *Amulet of Luck*
- **Interactive Chest & Interaction System**:
  - `IInteractable` interface for world object interactions.
  - World interaction input action (E key) bound in `PlayerInputActions`.
  - Interactive `ChestBehaviour` object with in-game prompt.
  - `ChestCreator` manager spawning chests dynamically upon enemy defeats.
  - `AmuletCreatorUI` 3-card draft modal displayed upon opening chests.
- **Combat & Stats Mechanics**:
  - **Critical Strike System**: Added Critical Strike Chance (`CritChance`) and Critical Damage (`CritDamage` / `CritMultiplier`) calculations with floating damage feedback.
  - **Luck Stat**: Added `Luck` stat in `StatsContainerSO`, directly scaling chest spawn chances.

### Changed
- Updated `PlayerStatVisualizerUI` to display Critical Chance, Critical Damage, and Luck stats.
- Extended New Input System configuration with world interaction binding.

### Fixed
- Fixed weapon fire rate stat calculation having inverted properties.

---

## [Alpha v0.2.0] - 2026-08-26

### Added
- New Input System integration (`PlayerInputActions`) across player mechanics and UI.
- Enemy cluster spawning system with on-screen warnings every minute.
- Scaling enemy health and damage over elapsed time.
- Floating damage popup indicators and hit indicator pool.
- Fast Enemy and Tank Enemy data configurations.
- Death screen logic and flow.
- Armor, Life Steal, and Experience Multiplier stats.
- Auto-fill shield upon unlocking for the first time.
- Time freeze while selecting level-up upgrades.
- Generic on-screen notification system.

### Changed
- Release balance adjustments.
- Buffed orb pickup speed and attraction radius.
- Improved enemy spawner variable configuration logic.
- Merged separate walk/run speeds into a single movement speed stat.

### Fixed
- Fixed enemy first attack timing bug.
- Fixed shield orb spawning before shield unlock.
- Fixed upgrade generator offering percentage upgrades on zero-value stats.

### Removed
- Legacy Unity Input Manager polling.
- Crouch speed stat.
- Separate walk/run speed stats.

---

## [Alpha v0.1] - 2026-08-20

### Added
- Initial release with core FPS shooting, movement, basic enemy spawning, weapons, and upgrade systems.

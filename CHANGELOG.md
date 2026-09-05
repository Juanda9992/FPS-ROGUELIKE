# Changelog

All notable changes to **ROGUELIKE FPS SURVIVAL** will be documented in this file.

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

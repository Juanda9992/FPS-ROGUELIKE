# Project Guidelines & Agent Rules (`AGENTS.md`)

This file contains instructions and guidelines for AI agents (like Antigravity) working on the **ROGUELIKE FPS SURVIVAL** Unity project.

---

## 1. Code Style & Standards (C# / Unity)

- **Naming Conventions**:
  - PascalCase for class names, method names, public fields, and properties (e.g., `EffectCapsule`, `CreateAoEObject`, `SpawnParams`).
  - camelCase with leading underscore for private fields (e.g., `_spawnParams`, `_hasSpawnedAoE`).
  - Explicit access modifiers on all members (`private`, `public`, `protected`, `internal`).

- **Unity Inspector**:
  - Use `[SerializeField] private` for fields exposed to the Unity Inspector rather than public fields.
  - Group inspector settings logically using `[Header("Header Title")]` and `[Tooltip("...")]`.

- **Null Safety & Component Checks**:
  - Use `TryGetComponent<T>(out var component)` instead of `GetComponent<T>() != null` where possible for better performance and readability.

- **Bracket use in simple ifs**:
  - Always add double brackets on the if states even if they have only one line of code, I want the style to be consistent

- **References on initialize**
  - Dont use GetComponent when a script is being initialized or in the start/awaking. I want the references to be assigned in the inspector when it is needed. Only use GetComponent if it is necessary and if it is not possible to assign the reference in the inspector, but this should be an exception

---

## 2. Architecture & Patterns

- **Interfaces**:
  - `ISpawneable`: Implemented by objects instantiated via spawn skills. Requires `void Initialize(SpawnParams spawnParams)`.
  - Action/Interaction Interfaces: `IDamageable`, `IPusheable`, `ISlowable`, `IStuneable`, `IBlindable`, `ISilenceable`. Always verify interface implementation before applying effects.

- **Skills & ScriptableObjects**:
  - Skills inherit from `ActiveSkillSO` or specialized sub-classes like `SpawnObjectSkill`.
  - Skill execution params are bundled inside `SpawnParams` data containers.

- **Folder Hierarchy**:
  - `Assets/Scripts/Enviroment/SkillObjects/Spawnables/`: Place all spawnable object scripts (e.g., `AoEObject`, `LandmineObject`, `EffectCapsule`).
  - `Assets/Scripts/Interfaces/`: Core interaction interfaces.
  - `Assets/Scripts/Scriptables/`: ScriptableObject definitions for skills, items, and configurations.
  - `Assets/Scripts/Player/`: Player mechanics, controllers, and UI.

---

## 3. Custom Project Notes & User Instructions

*Add any specific instructions, design notes, or workflow preferences below:*

- 

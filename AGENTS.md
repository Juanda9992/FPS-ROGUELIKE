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
  - Avoid using Tooltip on fields that are self explanatory (e.g. the name of the field is enough to understand what it does).
  - Avoid using [FormerlySerializedAs("variableName")] on fields, it is not necessary and makes the code harder to read.

- **Null Safety & Component Checks**:
  - Use `TryGetComponent<T>(out var component)` instead of `GetComponent<T>() != null` where possible for better performance and readability.
  - Avoid using null checks on components that are assigned in the inspector, these will never be null.
  - Avoid using ternary operators on components that are serialized in the inspector, it makes it harder to read.
  - Avoid using null check on Singleton pattern components, these will never be null.

- **Bracket use in simple ifs**:
  - Always add double brackets on the if states even if they have only one line of code, I want the style to be consistent

- **References on initialize**
  - Dont use GetComponent when a script is being initialized or in the start/awaking. I want the references to be assigned in the inspector when it is needed. Only use GetComponent if it is necessary and if it is not possible to assign the reference in the inspector, but this should be an exception.

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
- **Review Instructions**:
  - Always generate implementation plan and ask for confirmation before making any changes if the task is big or complex (It involves 3+ files or more than 100 lines of code).
- **Input System**
  - All the project should use the New Input System, not the old input manager, there is a class for it called PlayerInputActions, so use it. 

## 4. Agent Rules
- **Explain the code with examples**
  - When you are given a instruction to explain a certian piece of code, always explain it with examples, using mock data to track the values of the variables on the examples, only add explanation if i EXPLICITLY ASKED for it, other wise just provide the code.

## 5. Testing the code
  **ContextMenu**
  - Always add a context menu to the managers and systems, that let you tets the functionality of the script. The tests should be very simple and just call the public methods of the script, no need to overcomplicate things, just call the methods and see if they work as expected. The context menu should always test the most important functionality of the script.

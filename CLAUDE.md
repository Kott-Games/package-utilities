# Packages/Utilities

Git submodule (`Kott-Games/package-utilities`). General-purpose Unity helpers,
intentionally generic — written to be reusable across projects, not
Tower-Of-Greed-specific. If a change here is actually game-specific, it probably
belongs in `Assets/Scripts` instead, not this package.

This is our own code (not vendored third-party) — read and edit freely, but remember
edits need a commit inside this submodule's own repo, then a pointer bump in the
main project (see root `CLAUDE.md` → Submodules).

## What's here

- `AddressablesUtility.cs` — helpers around Unity Addressables loading.
- `JsonConverters/` — Newtonsoft/JSON.NET converters for common Unity value types
  (Vector2/3/4, Quaternion, Rect, Bounds, Color, ...) — used wherever Unity types
  need to round-trip through JSON (e.g. config serialization in `Packages/Configs`).
- `Progress/` — composable progress-reporting for async operations
  (`ProgressReport`, `SubProgress`, `WaitForAsyncOperationWithProgress`,
  `WaitForTaskWithProgress`, ...) — used by the Scopes package's async scene/manager
  loading to report loading-screen progress.
- `UI/` — `DynamicScrollView`, `UpdatableScrollRect`, `IView` — generic scroll-view UI
  helpers.
- `PrefabObjectPool.cs` — generic pooling.
- `SerializableDictionary.cs` — Unity-inspector-serializable dictionary.
- `LinqHelpers.cs`, `MathUtility.cs`, `RectExtensions.cs`, `RectIntExtensions.cs`,
  `TransformExtensions.cs`, `ColorExtensions.cs`, `VersionUtility.cs` — small generic
  extension/utility methods.
- `NoiseUtility.cs`, `NoiseValue.cs`, `NoiseValueGenerator.cs` — noise generation.
- `GenericListAttribute.cs` — inspector attribute for generic list drawing.
- `Tests/` — package's own editor tests.
- `Editor/UnityEditorUI`, `Editor/UnityEngineUI` — editor-only counterparts.

No project-specific gameplay logic belongs here — that's the line for what's
"in scope" for this package.

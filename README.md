# Kott Utilities

Generic runtime and editor helpers for Unity, shared by [Kott Scopes](https://github.com/Kott-Games/package-scopes)
and Kott Configs.

[![Tests](https://github.com/Kott-Games/package-utilities/actions/workflows/tests.yml/badge.svg)](https://github.com/Kott-Games/package-utilities/actions/workflows/tests.yml)
![Unity 6000.0+](https://img.shields.io/badge/Unity-6000.0%2B-black?logo=unity)
![License: MIT](https://img.shields.io/badge/License-MIT-blue)

## Contents

| Area | Types |
|---|---|
| Progress reporting | `ProgressReport`, `SubProgress` (maps a step onto a range of a parent progress), `WaitForTask`, `WaitForAsyncOperationWithProgress`, `WaitForAsyncOperationHandleWithProgress` (Addressables) |
| JSON.NET converters | Unity value types: vectors, `Quaternion`, `Color`, `Rect(Int)`, `Bounds(Int)`; `DictionaryArrayConverter` |
| Collections | `SerializableDictionary<TKey, TValue>` (with drawer), `PrefabObjectPool<T>`, `LinqHelpers` |
| UI | `DynamicScrollView`, `UpdatableScrollRect` |
| Math & noise | `MathUtility`, `NoiseUtility`, `NoiseValue`, `NoiseValueGenerator` |
| Extensions | `Transform`, `Color`, `Rect`, `RectInt` |
| Editor | `EditorTable`, `CustomGUI`, `GenericListDrawer`, `VersionUtility` + window |

## Installation

Via [OpenUPM](https://openupm.com/packages/com.kott.utilities/):

```
openupm add com.kott.utilities
```

or add the registry to `Packages/manifest.json` by hand:

```json
"scopedRegistries": [
  { "name": "OpenUPM", "url": "https://package.openupm.com", "scopes": ["com.kott"] }
],
"dependencies": {
  "com.kott.utilities": "1.0.0"
}
```

Alternatively, *Package Manager > + > Install package from git URL*:

```
https://github.com/Kott-Games/package-utilities.git#v1.0.0
```

Depends on Newtonsoft JSON (`com.unity.nuget.newtonsoft-json`) and uGUI. Addressables is optional: the
Addressables helpers compile only when it is installed.

## License

[MIT](LICENSE.md)

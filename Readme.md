# Endless Runner (Trash Dash)

Mobile endless-runner sample originally published by Unity, actively modernized for **Unity 6** and **Android** device deployment.

**Unity version:** `6000.3.27f1` (see `ProjectSettings/ProjectVersion.txt`)

**Primary development branch:** `feature/iap5-migration`

---

## Project Overview

This is a lane-based **endless runner**: the player dodges obstacles, collects currency, completes missions, and unlocks characters, themes, and consumables through an in-game shop.

### Core gameplay systems

| Area | Description |
|------|-------------|
| **GameManager / states** | Loadout, active run (`GameState`), game over, and additive **Shop** scene |
| **TrackManager** | Procedural track segments, obstacles, themes (day/night), speed and scoring |
| **Addressables** | Characters, themes, zones, obstacles, power-ups, and related content loaded at runtime |
| **PlayerData** | Binary save (`save.bin`), characters, themes, premium currency, missions |
| **Shop** | Soft-currency and premium IAP catalog entries (live purchasing intentionally disabled) |
| **Audio** | `MusicPlayer` (DontDestroyOnLoad) with layered stems; SFX on character and obstacles |

### Technology stack (current packages)

| Package | Version (manifest) |
|---------|------------------|
| Universal RP | 17.3.0 |
| Addressables | 2.10.3 |
| Unity Purchasing (IAP) | 5.4.4 |
| uGUI | 2.0.0 |
| Android Logcat | 1.4.7 |

Scenes include **Start**, **main** (gameplay + loadout), and **shop** (additive).

---

## Unity Modernization

The project was upgraded from the historical **Unity 2019.3** sample baseline toward **Unity 6000.3.27f1**.

**Completed in version control (commits on `feature/iap5-migration`):**

- Package and project settings updates for Unity 6 / URP 17
- Removal of invalid editor-only references from runtime scripts (e.g. `TrackManager`)
- URP-related project configuration (including `URP_COMPATIBILITY_MODE` scripting define on Android for transitional compatibility)

**Partially completed / still in the working tree (not fully committed):**

- Additional URP assets, scene overrides, texture import metadata, and package manifest deltas
- Removal of embedded Asset Store Tools package files
- Editor-generated Addressables artifacts (`link.xml`, profile sort settings, etc.)

Treat uncommitted files as **work in progress** until they are reviewed and committed separately.

---

## Completed Improvements

The following items are **implemented and present in Git history** unless noted.

### Performance and stability

- **WorldCurver shader updates** — Global shader float is set only when the curve strength changes (reduces redundant per-frame updates). *Commit: WorldCurver optimization.*
- **Cloud object pooling** — `TrackManager` reuses cloud instances via a stack pool instead of repeated instantiate/destroy. *Commit: cloud object pooling.*
- **TrackManager runtime fixes** — Safer theme reference handling and Android/editor compatibility fixes (null checks, invalid `UnityEditor` usage removed). *Included in pooling / modernization commits and Addressables error-handling commit.*

### Unity IAP 5 migration (live purchases **off**)

- **`com.unity.purchasing` 5.4.4** integrated; legacy initialization patterns removed from shop entry points.
- **IAP 5 handlers** compile against IAP 5 types; **`IAPPurchaseFulfillmentPolicy.FulfillmentEnabled`** remains `false` — no premium grants from live payment callbacks.
- **Secure IAP foundation** — Sidecar transaction store design, idempotent fulfillment processor, receipt validator stub, Edit Mode tests (synthetic transaction IDs). **Server-side receipt validation and production `ConfirmPurchase` are not enabled.**

### Android Addressables

- **Root cause addressed:** Editor play mode did not require built player content; Android devices need packed Addressables under `StreamingAssets/aa` (injected at player build from `Library/com.unity.addressables`).
- **`Build Addressables with Player Build`** enabled in `AddressableAssetSettings`.
- **`BundleAndBuild.cs`** — Menu and batchmode entry points to build Android player content.
- **CharacterDatabase / ThemeDatabase / TrackManager** — Explicit async load failure handling and clearer errors when runtime catalog or bundles are missing.

### Android runtime quality

- **EventSystemCoordinator** — When **shop** loads additively over **main**, only one legacy EventSystem stays active; peer is restored on unload.
- **AudioPlayUtility** — Guards `Play` / `PlayOneShot` / `PlayDelayed` when the `AudioSource` or GameObject is disabled (gameplay SFX and music stems).

---

## Android Testing

**Device:** Xiaomi **Mi 9T** (physical Android)

**Verified (reported after Addressables pipeline repair):**

- Start screen loads; entering gameplay loads **Addressables** content on device.
- **Playable character**, **track segments**, **obstacles**, and **environment/theme** assets appear correctly (previously missing due to absent `settings.json` / runtime catalog on device).

**Not fully re-verified on device in this documentation pass:**

- Latest APK including EventSystem / `AudioPlayUtility` commits only
- IAP rollback Edit Mode test (see Known Issues)
- Frame time / thermal profiling

---

## Known Issues

| Issue | Status |
|-------|--------|
| **UI button AudioSource warnings** | Some Logcat warnings may still appear on touch (`UnityEvent` → disabled UI `AudioSource` on prefabs). Gameplay paths use `AudioPlayUtility`; UI click sounds may need a follow-up. |
| **`URP_COMPATIBILITY_MODE`** | Temporary Android scripting define for URP transition; not a long-term rendering strategy. |
| **IAP rollback test** | Edit Mode persistence/rollback scenario **must be revalidated** in Unity Test Runner before enabling fulfillment. |
| **Live IAP** | Disabled by policy (`FulfillmentEnabled`, `StoreConnectionEnabled`, `ConfirmPurchasesEnabled`). Shop buy buttons not wired for real money. |
| **Performance profiling** | Full mobile FPS / memory profiling pending. |
| **Uncommitted Unity 6 changes** | Large working-tree diff (URP, scenes, packages, Addressables editor files) remains outside feature commits. |
| **Dual EventSystem / UI Toolkit** | Unity 6 may log EventSystem registration via UI Elements in some flows; additive shop case is mitigated by `EventSystemCoordinator`. |

---

## Roadmap

- Responsive mobile HUD modernization
- Main menu and Shop UI improvements
- Character and obstacle content expansion
- URP **Render Graph** migration (move off compatibility mode)
- Mobile FPS optimization and profiling
- Secure IAP production integration (server receipt validation, store wiring, controlled enablement)
- Google Play release preparation (signing, privacy, store listing)

---

## Build Instructions

### Prerequisites

- **Unity Hub** with editor **6000.3.27f1**
- **Android Build Support** (SDK/NDK as required by Unity 6)
- **Git LFS** (original sample assets may use LFS; install [git-lfs](https://git-lfs.github.com/) and run `git lfs install` before clone)

### Open the project

1. Clone the repository and open `EndlessRunnerSampleGame` in Unity **6000.3.27f1**.
2. Allow packages to resolve (Addressables, URP, Purchasing).

### Build Addressables for Android

Player builds require **packed Addressables content** for the Android target.

**Option A — Editor menu**

1. Switch platform to **Android** (`File → Build Settings → Android → Switch Platform`).
2. Run **`Trash Dash Debug → Build with Bundle → Build Addressables (Android)`**.

**Option B — Batchmode (CI / headless)**

Close other Unity instances using the same project, then:

```text
Unity.exe -batchmode -nographics ^
  -projectPath "<path-to>/EndlessRunnerSampleGame" ^
  -executeMethod BundleAndBuild.BuildAddressablesAndroidBatch ^
  -quit -logFile Logs/addressables-android-build.log
```

Built content is written under `Library/com.unity.addressables/aa/Android/` (catalog, `settings.json`, asset bundles). It is **copied into the APK** as `StreamingAssets/aa` when you build the player (enabled via **Build Addressables with Player Build** in Addressables settings).

### Android Development APK

1. Complete the Addressables step above (or rely on automatic build-on-player if unchanged).
2. **`File → Build Settings`**
3. Select **Android**, enable **Development Build** if desired.
4. **Build** or **Build And Run**.

Install on device and monitor **`Window → Analysis → Android Logcat`** (package `com.unity.mobile.android-logcat`).

### Play Mode note

The Editor can run with Addressables **Play Mode** scripts that use the Asset Database. **Device builds always require** a recent Android Addressables player content build before producing the APK.

---

## Repository Notes

This codebase derives from Unity’s [Endless Runner Sample Game](https://github.com/Unity-Technologies/EndlessRunnerSampleGame). Modernization commits live on **`feature/iap5-migration`**. Push only to a **personal or team-owned** remote — not the upstream Unity sample repository unless you intentionally maintain a fork there.

For legacy documentation, see `Assets/INSTRUCTION.txt` and the [original project wiki](https://github.com/Unity-Technologies/EndlessRunnerSampleGame/wiki) (may not reflect Unity 6 changes).

---

## License

See repository license terms inherited from the Unity sample and any additional notices in the project.

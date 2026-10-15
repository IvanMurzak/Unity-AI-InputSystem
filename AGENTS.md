# AGENTS.md — Unity-AI-InputSystem

Unity package `com.ivanmurzak.unity.mcp.inputsystem` — makes Unity's **Input System** (`com.unity.inputsystem`)
available to Unity-MCP's RQL tools (`rql-query` / `rql-mutate`; no extension MCP tools): the `input` source
(assets, layouts, interactions, processors, `CreateAsset`), and InputActionAsset maps / actions / bindings / control
schemes as writable views with add / remove methods. An extension of [AI Game Developer](https://github.com/IvanMurzak/Unity-MCP)
(`com.ivanmurzak.unity.mcp`), generated from `Unity-AI-Tools-Template`; the user-facing tool list is in
`README.md`.

## Layout

- `Unity-Package/` — Unity project hosting the package; only `Packages/com.ivanmurzak.unity.mcp.inputsystem/`
  ships.
  - `package.json` — the version source of truth and the dependency pins.
  - `Editor/Scripts/Rql/` — branch `experiment/rql`: the `InputSystemModule` (IRqlModule: `input` source, the
    InputActionAsset adapter, map / action / binding / control-scheme views, `InputAssetStore` persistence, the agent
    guide); replaces the former `inputsystem-*` tools.
  - `Runtime/` — runtime assembly (currently empty).
  - `Tests/Editor/Rql/` (EditMode, `InputSystemTestBase` + one file per adapter / view / source) and `Tests/Runtime/` (PlayMode).
- `Unity-Tests/<unity-version>/` — one test project per supported Unity version, consuming the package via
  `file:`. A new version must also be added to `test_pull_request.yml` and `release.yml` (and
  `open-all-projects-*`).
- `Installer/` — Unity project that exports the `.unitypackage` installer
  (`Assets/com.IvanMurzak/<name> Installer/`).
- `commands/` — PowerShell helpers (below) and `check-versions.py` (synced in by the release cascade — do not
  hand-edit; CI runs it as the `version-consistency` gate).
- `.github/workflows/` — `test_pull_request.yml` (PR test matrix), `test_unity_plugin.yml` (reusable test
  job), `release.yml`, `bump_version.yml`.
- `docs/` — user docs: `Deploy-{OpenUPM,GitHub,npmjs}.md`, `Manual-Package-Rename.md`, `openupm-signing.md`,
  `img/`, `promo/`.

## Commands (PowerShell, from the repo root)

```powershell
./commands/bump-version.ps1 -NewVersion "x.y.z"   # package.json + Installer.cs in lock-step; -WhatIf to preview
./commands/get-version.ps1
./commands/update-ai-game-developer.ps1           # bump the com.ivanmurzak.unity.mcp pin; -WhatIf to preview
./commands/open-all-projects-windows.ps1          # open all Unity projects (or -unix.sh)
```

Tests run inside the Unity Editor (NUnit + `[UnityTest]`); CI runs them with `game-ci/unity-test-runner` in
EditMode / PlayMode / Standalone for every Unity version. `release.yml` releases on push to `main` only when
the `package.json` version has no tag yet (installer, signed UPM tarball for OpenUPM, test matrix, tag, GitHub
Release); dispatch `bump_version.yml` to bump in CI. Under `pull_request_target`, tests need the
maintainer-applied `ci-ok` label and abort if the PR touches `.github/workflows/`.

## Critical invariants

- **RQL shape (branch `experiment/rql`).** Agents use Unity-MCP's `rql-query` / `rql-mutate`; no `inputsystem-*`
  tools remain. Assets and their parts are views addressed by id (`ActionMapView`, `ActionView`, `BindingView`;
  `ControlSchemeView` by name) because a re-import replaces the map / action objects. Validation lives in
  `InputValidation`; the guide in `InputSystemModule` must keep one runnable example per operation (a test runs them).
- **Asset-backed.** Every write goes through `InputAssetStore.Edit` (Undo record, change, JSON to the
  `.inputactions` file, re-import); Undo / Redo rewrite the file through `InputAssetStore.Resync`. Serialize with
  `InputAssetStore.ToJson`, never raw `ToJson()`: Input System 1.x throws on an asset with no action maps.
- Every C# file starts with the copyright box header and `#nullable enable`.

<h1 align="center"><a href="https://github.com/IvanMurzak/Unity-AI-InputSystem?tab=readme-ov-file#unity-ai-inputsystem">Unity AI InputSystem</a></h1>

<div align="center" width="100%">

[![MCP](https://badge.mcpx.dev 'MCP Server')](https://modelcontextprotocol.io/introduction)
[![OpenUPM](https://img.shields.io/npm/v/com.ivanmurzak.unity.mcp.inputsystem?label=OpenUPM&registry_uri=https://package.openupm.com&labelColor=333A41 'OpenUPM package')](https://openupm.com/packages/com.ivanmurzak.unity.mcp.inputsystem/)
[![Unity Editor](https://img.shields.io/badge/Editor-X?style=flat&logo=unity&labelColor=333A41&color=2A2A2A 'Unity Editor supported')](https://unity.com/releases/editor/archive)
[![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg 'Tests Passed')](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)</br>
[![Discord](https://img.shields.io/badge/Discord-Join-7289da?logo=discord&logoColor=white&labelColor=333A41 'Join')](https://discord.gg/cfbdMZX99G)
[![Stars](https://img.shields.io/github/stars/IvanMurzak/Unity-AI-InputSystem 'Stars')](https://github.com/IvanMurzak/Unity-AI-InputSystem/stargazers)
[![License](https://img.shields.io/github/license/IvanMurzak/Unity-AI-InputSystem?label=License&labelColor=333A41)](https://github.com/IvanMurzak/Unity-AI-InputSystem/blob/main/LICENSE)
[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://stand-with-ukraine.pp.ua)

</div>

AI-powered tools for the Unity [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html) authoring workflow. Create `InputActionAsset`s, add and remove action maps, actions, and bindings, build composite bindings (1D/2D axes), define control schemes, save assets, and modify any input asset field directly through natural language commands — no manual Input Actions editor navigation. Wraps Unity's **Input System** package (`com.unity.inputsystem`). Ideal for rapidly bootstrapping player controls, prototyping input layouts, and wiring gameplay actions. Built on top of the [AI Game Developer](https://github.com/IvanMurzak/Unity-MCP) platform.

### How to use

- [Instructions](https://github.com/IvanMurzak/Unity-MCP?tab=readme-ov-file#step-2-install-mcp-client)
- [Video Tutorial for Visual Studio Code](https://www.youtube.com/watch?v=ZhP7Ju91mOE)
- [Video Tutorial for Visual Studio](https://www.youtube.com/watch?v=RGdak4T69mc)

[![DOWNLOAD INSTALLER](https://github.com/IvanMurzak/Unity-MCP/blob/main/docs/img/button/button_download.svg?raw=true)](https://github.com/IvanMurzak/Unity-AI-InputSystem/releases/latest/download/AI-InputSystem-Installer.unitypackage)

### Stability status

| Unity Version | Editmode                                                                                                                                                                                                          | Playmode                                                                                                                                                                                                          | Standalone                                                                                                                                                                                                          |
| ------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2022.3.62f3   | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-2022-3-62f3-editmode)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)       | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-2022-3-62f3-playmode)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)       | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-2022-3-62f3-standalone)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)       |
| 2023.2.22f1   | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-2023-2-22f1-editmode)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)       | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-2023-2-22f1-playmode)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)       | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-2023-2-22f1-standalone)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)       |
| 6000.3.1f1    | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-6000-3-1f1-editmode)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)        | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-6000-3-1f1-playmode)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)        | [![r](https://github.com/IvanMurzak/Unity-AI-InputSystem/workflows/release/badge.svg?job=test-unity-6000-3-1f1-standalone)](https://github.com/IvanMurzak/Unity-AI-InputSystem/actions/workflows/release.yml)        |

## AI InputSystem Tools

13 tools, grouped by purpose:

### Asset lifecycle

- `inputsystem-asset-create` - Create a new `InputActionAsset` at a project path
- `inputsystem-get` - Get the structure of an `InputActionAsset` (action maps, actions, bindings, control schemes)
- `inputsystem-save` - Save the `InputActionAsset` to disk

### Action maps & actions

- `inputsystem-actionmap-add` - Add an action map to the asset
- `inputsystem-actionmap-remove` - Remove an action map by name
- `inputsystem-action-add` - Add an action (Button / Value / PassThrough) to an action map
- `inputsystem-action-remove` - Remove an action from an action map

### Bindings

- `inputsystem-binding-add` - Add a simple binding (e.g. `<Keyboard>/space`) to an action
- `inputsystem-binding-composite-add` - Add a composite binding (1D axis, 2D vector / WASD, …) to an action
- `inputsystem-binding-set` - Set / update an existing binding's path or properties
- `inputsystem-binding-remove` - Remove a binding from an action

### Control schemes & generic

- `inputsystem-controlscheme-add` - Add a control scheme (device requirements) to the asset
- `inputsystem-modify` - Generic write: apply a `SerializedMember` diff to any Input System object via ReflectorNet (escape hatch for fields not covered by the dedicated tools)

## Installation

### Option 1 - Installer

- **[Download Installer](https://github.com/IvanMurzak/Unity-AI-InputSystem/releases/latest/download/AI-InputSystem-Installer.unitypackage)**
- **Import installer into Unity project**
  > - You can double-click on the file - Unity will open it automatically
  > - OR: Open Unity Editor first, then click on `Assets/Import Package/Custom Package`, and choose the file

### Option 2 - OpenUPM-CLI

- [Install OpenUPM-CLI](https://github.com/openupm/openupm-cli#installation)
- Open the command line in your Unity project folder

```bash
openupm add com.ivanmurzak.unity.mcp.inputsystem
```

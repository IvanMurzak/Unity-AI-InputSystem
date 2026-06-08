/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
│  Licensed under the MIT License.                                            │
│  See the LICENSE file in the project root for more information.             │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using com.IvanMurzak.McpPlugin;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    [AiToolType]
    public partial class Tool_InputSystem
    {
        public static class Error
        {
            public static string AssetPathRequired()
                => "[Error] 'assetPath' is required. Provide an 'Assets/'-rooted path ending in '.inputactions'.";

            public static string AssetPathInvalid(string assetPath)
                => $"[Error] Asset path '{assetPath}' must start with 'Assets/' and end with '.inputactions'.";

            public static string AssetNotFound(string assetPath)
                => $"[Error] No InputActionAsset found at '{assetPath}'. Use 'inputsystem-asset-create' first, or 'assets-find' to locate it.";

            public static string AssetAlreadyExists(string assetPath)
                => $"[Error] An asset already exists at '{assetPath}'. Choose a different path or delete the existing asset first.";

            public static string ActionMapNotFound(string mapName)
                => $"[Error] ActionMap '{mapName}' not found in the asset. Use 'inputsystem-get' to list the available maps.";

            public static string ActionMapAlreadyExists(string mapName)
                => $"[Error] ActionMap '{mapName}' already exists in the asset.";

            public static string ActionNotFound(string actionName, string mapName)
                => $"[Error] Action '{actionName}' not found in ActionMap '{mapName}'. Use 'inputsystem-get' to list the available actions.";

            public static string ActionAlreadyExists(string actionName, string mapName)
                => $"[Error] Action '{actionName}' already exists in ActionMap '{mapName}'.";

            public static string BindingIndexOutOfRange(int index, int count)
                => $"[Error] Binding index {index} is out of range. The action/map has {count} binding(s) (valid indices 0..{count - 1}).";

            public static string ControlSchemeAlreadyExists(string schemeName)
                => $"[Error] Control scheme '{schemeName}' already exists in the asset.";

            public static string ReflectorNotAvailable()
                => "[Error] ReflectorNet reflector is not available.";
        }
    }
}

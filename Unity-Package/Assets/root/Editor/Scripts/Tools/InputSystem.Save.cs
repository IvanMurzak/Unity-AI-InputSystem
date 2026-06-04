/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using System.ComponentModel;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string SaveToolId = "inputsystem-save";

        [AiTool
        (
            SaveToolId,
            Title = "InputSystem / Save Asset",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Re-serialize a `.inputactions` InputActionAsset to disk and re-import it. Useful to " +
            "force-persist an asset after external edits.")]
        [AiSkillBody("Persist an `InputActionAsset` to its `.inputactions` file. The mutation tools save " +
            "automatically; this tool is an explicit save for cases where the on-disk JSON should be regenerated " +
            "(e.g. after edits made outside these tools).\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, re-serializes it to JSON via `ToJson()`, writes the file, and re-imports it into the " +
            "AssetDatabase. Runs on the Unity main thread.")]
        [Description("Re-serializes an InputActionAsset to disk and re-imports it.")]
        public SaveResponse SaveInputActionAsset
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath
        )
        {
            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                SaveAsset(asset);

                return new SaveResponse
                {
                    assetPath = Normalize(assetPath),
                    actionMapCount = asset.actionMaps.Count,
                    controlSchemeCount = asset.controlSchemes.Count,
                    success = true
                };
            });
        }

        public class SaveResponse
        {
            [Description("Path of the saved asset.")]
            public string assetPath = string.Empty;

            [Description("Number of ActionMaps in the saved asset.")]
            public int actionMapCount;

            [Description("Number of control schemes in the saved asset.")]
            public int controlSchemeCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

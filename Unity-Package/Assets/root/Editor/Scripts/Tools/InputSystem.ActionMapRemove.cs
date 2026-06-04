/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System;
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string ActionMapRemoveToolId = "inputsystem-actionmap-remove";

        [AiTool
        (
            ActionMapRemoveToolId,
            Title = "InputSystem / Remove ActionMap",
            ReadOnlyHint = false,
            DestructiveHint = true,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Remove an ActionMap (and all its Actions and Bindings) from a `.inputactions` asset and save it.")]
        [AiSkillBody("Remove an `InputActionMap` from an InputActionAsset. This deletes the map along with every " +
            "Action and Binding it contains.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` — required. Name of the ActionMap to remove.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, removes the named ActionMap (failing if it does not exist), saves and re-imports the " +
            "asset. Destructive. Runs on the Unity main thread.")]
        [Description("Removes an ActionMap (and its Actions/Bindings) from an InputActionAsset and saves it.")]
        public ActionMapRemoveResponse RemoveActionMap
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap to remove.")]
            string mapName
        )
        {
            if (string.IsNullOrWhiteSpace(mapName))
                throw new ArgumentException("mapName is required.", nameof(mapName));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                var map = GetActionMap(asset, mapName);

                asset.RemoveActionMap(map);
                SaveAsset(asset);

                return new ActionMapRemoveResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionMapCount = asset.actionMaps.Count,
                    success = true
                };
            });
        }

        public class ActionMapRemoveResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the removed ActionMap.")]
            public string mapName = string.Empty;

            [Description("Total number of ActionMaps remaining in the asset.")]
            public int actionMapCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

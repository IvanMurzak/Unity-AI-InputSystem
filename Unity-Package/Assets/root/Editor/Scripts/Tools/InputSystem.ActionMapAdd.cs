/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
│  Licensed under the MIT License.                                            │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System;
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using UnityEngine.InputSystem;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string ActionMapAddToolId = "inputsystem-actionmap-add";

        [AiTool
        (
            ActionMapAddToolId,
            Title = "InputSystem / Add ActionMap",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Add a new ActionMap to an existing `.inputactions` InputActionAsset and save it.")]
        [AiSkillBody("Add a new `InputActionMap` to an InputActionAsset. An ActionMap groups related Actions " +
            "(e.g. a 'Player' map and a 'UI' map).\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` — required. Name of the new ActionMap. Must be unique within the asset.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, adds the ActionMap (failing if a map of that name already exists), saves the asset back " +
            "to disk, and re-imports it. Runs on the Unity main thread.")]
        [Description("Adds a new ActionMap to an existing InputActionAsset and saves it.")]
        public ActionMapAddResponse AddActionMap
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the new ActionMap (unique within the asset).")]
            string mapName
        )
        {
            if (string.IsNullOrWhiteSpace(mapName))
                throw new ArgumentException("mapName is required.", nameof(mapName));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                if (asset.FindActionMap(mapName, throwIfNotFound: false) != null)
                    throw new Exception(Error.ActionMapAlreadyExists(mapName));

                asset.AddActionMap(mapName);
                SaveAsset(asset);

                return new ActionMapAddResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionMapCount = asset.actionMaps.Count,
                    success = true
                };
            });
        }

        public class ActionMapAddResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the added ActionMap.")]
            public string mapName = string.Empty;

            [Description("Total number of ActionMaps in the asset after the addition.")]
            public int actionMapCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

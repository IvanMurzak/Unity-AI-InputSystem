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
using UnityEngine.InputSystem;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string ActionRemoveToolId = "inputsystem-action-remove";

        [AiTool
        (
            ActionRemoveToolId,
            Title = "InputSystem / Remove Action",
            ReadOnlyHint = false,
            DestructiveHint = true,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Remove an Action (and its Bindings) from an ActionMap in a `.inputactions` asset and save it.")]
        [AiSkillBody("Remove an `InputAction` from an `InputActionMap`. This deletes the Action and every Binding " +
            "associated with it.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` — required. Name of the ActionMap containing the Action.\n" +
            "- `actionName` — required. Name of the Action to remove.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, removes the named Action from the map (failing if it does not exist), saves and " +
            "re-imports the asset. Destructive. Runs on the Unity main thread.")]
        [Description("Removes an Action (and its Bindings) from an ActionMap and saves the asset.")]
        public ActionRemoveResponse RemoveAction
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap containing the Action.")]
            string mapName,
            [Description("Name of the Action to remove.")]
            string actionName
        )
        {
            if (string.IsNullOrWhiteSpace(mapName))
                throw new ArgumentException("mapName is required.", nameof(mapName));
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("actionName is required.", nameof(actionName));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                var map = GetActionMap(asset, mapName);
                var action = GetAction(map, actionName);

                action.RemoveAction();
                SaveAsset(asset);

                return new ActionRemoveResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionName = actionName,
                    actionCount = map.actions.Count,
                    success = true
                };
            });
        }

        public class ActionRemoveResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the ActionMap the Action was removed from.")]
            public string mapName = string.Empty;

            [Description("Name of the removed Action.")]
            public string actionName = string.Empty;

            [Description("Number of Actions remaining in the map.")]
            public int actionCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

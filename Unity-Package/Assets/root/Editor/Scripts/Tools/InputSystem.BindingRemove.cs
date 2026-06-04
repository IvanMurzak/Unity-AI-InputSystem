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
        public const string BindingRemoveToolId = "inputsystem-binding-remove";

        [AiTool
        (
            BindingRemoveToolId,
            Title = "InputSystem / Remove Binding",
            ReadOnlyHint = false,
            DestructiveHint = true,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Remove a Binding by its index from an Action in a `.inputactions` asset, then save it.")]
        [AiSkillBody("Remove an `InputBinding` from an `InputAction` by its index within that Action's binding list. " +
            "Use 'inputsystem-get' to discover binding indices first.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` / `actionName` — required. The Action that owns the binding.\n" +
            "- `bindingIndex` — required. Zero-based index of the binding within the Action.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, resolves the Action, erases the binding at the given index (failing if out of range), " +
            "saves and re-imports the asset. Note: erasing a composite root also affects its parts. Destructive. " +
            "Runs on the Unity main thread.")]
        [Description("Removes a Binding by index from an Action and saves the asset.")]
        public BindingRemoveResponse RemoveBinding
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap containing the Action.")]
            string mapName,
            [Description("Name of the Action that owns the binding.")]
            string actionName,
            [Description("Zero-based index of the binding within the Action's binding list.")]
            int bindingIndex
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

                if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
                    throw new Exception(Error.BindingIndexOutOfRange(bindingIndex, action.bindings.Count));

                action.ChangeBinding(bindingIndex).Erase();
                SaveAsset(asset);

                return new BindingRemoveResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionName = actionName,
                    removedIndex = bindingIndex,
                    bindingCount = action.bindings.Count,
                    success = true
                };
            });
        }

        public class BindingRemoveResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the ActionMap.")]
            public string mapName = string.Empty;

            [Description("Name of the Action.")]
            public string actionName = string.Empty;

            [Description("Index of the binding that was removed.")]
            public int removedIndex = -1;

            [Description("Number of bindings remaining on the Action.")]
            public int bindingCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

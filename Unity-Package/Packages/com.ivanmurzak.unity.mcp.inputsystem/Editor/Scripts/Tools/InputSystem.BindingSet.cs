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
        public const string BindingSetToolId = "inputsystem-binding-set";

        [AiTool
        (
            BindingSetToolId,
            Title = "InputSystem / Set Binding",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Update a Binding's path, groups, interactions and/or processors by index on an Action " +
            "in a `.inputactions` asset, then save it. Only the supplied fields are changed.")]
        [AiSkillBody("Update fields of an existing `InputBinding` (identified by its index on an `InputAction`). Only " +
            "the parameters you provide are applied; the rest are left unchanged. Use 'inputsystem-get' to find the " +
            "binding index first.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` / `actionName` — required. The Action that owns the binding.\n" +
            "- `bindingIndex` — required. Zero-based index of the binding.\n" +
            "- `path` — optional new control path.\n" +
            "- `groups` — optional new control-scheme group(s).\n" +
            "- `interactions` — optional new interactions.\n" +
            "- `processors` — optional new processors.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, resolves the Action and binding index (failing if out of range), applies each supplied " +
            "field via the `ChangeBinding` syntax (`WithPath` / `WithGroups` / `WithInteractions` / `WithProcessors`), " +
            "saves and re-imports the asset. Runs on the Unity main thread.")]
        [Description("Updates a Binding's path/groups/interactions/processors by index on an Action and saves the asset.")]
        public BindingSetResponse SetBinding
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap containing the Action.")]
            string mapName,
            [Description("Name of the Action that owns the binding.")]
            string actionName,
            [Description("Zero-based index of the binding to update.")]
            int bindingIndex,
            [Description("Optional new control path.")]
            string? path = null,
            [Description("Optional new control-scheme group(s).")]
            string? groups = null,
            [Description("Optional new interactions.")]
            string? interactions = null,
            [Description("Optional new processors.")]
            string? processors = null
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

                var syntax = action.ChangeBinding(bindingIndex);
                if (path != null)
                    syntax = syntax.WithPath(path);
                if (groups != null)
                    syntax = syntax.WithGroups(groups);
                if (interactions != null)
                    syntax = syntax.WithInteractions(interactions);
                if (processors != null)
                    syntax = syntax.WithProcessors(processors);

                SaveAsset(asset);

                var binding = action.bindings[bindingIndex];
                return new BindingSetResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionName = actionName,
                    bindingIndex = bindingIndex,
                    path = binding.path,
                    groups = binding.groups,
                    interactions = binding.interactions,
                    processors = binding.processors,
                    success = true
                };
            });
        }

        public class BindingSetResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the ActionMap.")]
            public string mapName = string.Empty;

            [Description("Name of the Action.")]
            public string actionName = string.Empty;

            [Description("Index of the updated binding.")]
            public int bindingIndex = -1;

            [Description("Resulting control path of the binding.")]
            public string? path;

            [Description("Resulting control-scheme group(s).")]
            public string? groups;

            [Description("Resulting interactions.")]
            public string? interactions;

            [Description("Resulting processors.")]
            public string? processors;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

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
        public const string BindingAddToolId = "inputsystem-binding-add";

        [AiTool
        (
            BindingAddToolId,
            Title = "InputSystem / Add Binding",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Add a simple (non-composite) Binding to an Action in a `.inputactions` asset, with an " +
            "optional control path, groups, interactions and processors, then save it.")]
        [AiSkillBody("Add an `InputBinding` to an existing `InputAction`. A binding maps a physical control path " +
            "(e.g. `<Keyboard>/space`) to the Action. For composite bindings (2DVector / 1DAxis) use " +
            "'inputsystem-binding-composite-add' instead.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` — required. ActionMap containing the Action.\n" +
            "- `actionName` — required. Action to add the binding to.\n" +
            "- `path` — control path (e.g. `<Keyboard>/space`, `<Gamepad>/leftStick`).\n" +
            "- `groups` — optional control-scheme group(s) (semicolon-separated, e.g. `Keyboard&Mouse`).\n" +
            "- `interactions` — optional interactions (e.g. `hold`, `press,tap`).\n" +
            "- `processors` — optional processors (e.g. `invert`, `scale(factor=2)`).\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, resolves the Action, appends a binding with the supplied metadata, saves and re-imports " +
            "the asset, and returns the new binding's index. Runs on the Unity main thread.")]
        [Description("Adds a simple Binding (path/groups/interactions/processors) to an Action and saves the asset.")]
        public BindingAddResponse AddBinding
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap containing the Action.")]
            string mapName,
            [Description("Name of the Action to add the binding to.")]
            string actionName,
            [Description("Control path for the binding (e.g. '<Keyboard>/space').")]
            string path,
            [Description("Optional control-scheme group(s), semicolon-separated.")]
            string? groups = null,
            [Description("Optional interactions (e.g. 'hold', 'press,tap').")]
            string? interactions = null,
            [Description("Optional processors (e.g. 'invert').")]
            string? processors = null
        )
        {
            if (string.IsNullOrWhiteSpace(mapName))
                throw new ArgumentException("mapName is required.", nameof(mapName));
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("actionName is required.", nameof(actionName));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is required.", nameof(path));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                var map = GetActionMap(asset, mapName);
                var action = GetAction(map, actionName);

                action.AddBinding(
                    path: path,
                    interactions: string.IsNullOrEmpty(interactions) ? null : interactions,
                    processors: string.IsNullOrEmpty(processors) ? null : processors,
                    groups: string.IsNullOrEmpty(groups) ? null : groups);

                SaveAsset(asset);

                return new BindingAddResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionName = actionName,
                    path = path,
                    bindingIndex = action.bindings.Count - 1,
                    bindingCount = action.bindings.Count,
                    success = true
                };
            });
        }

        public class BindingAddResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the ActionMap.")]
            public string mapName = string.Empty;

            [Description("Name of the Action.")]
            public string actionName = string.Empty;

            [Description("The control path that was bound.")]
            public string path = string.Empty;

            [Description("Index of the new binding within the Action's binding list.")]
            public int bindingIndex = -1;

            [Description("Number of bindings on the Action after the addition.")]
            public int bindingCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

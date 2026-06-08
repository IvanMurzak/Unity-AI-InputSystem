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
        public const string ActionAddToolId = "inputsystem-action-add";

        [AiTool
        (
            ActionAddToolId,
            Title = "InputSystem / Add Action",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Add an Action (with type and optional expectedControlType) to an ActionMap in a " +
            "`.inputactions` asset, optionally with an initial binding path, and save it.")]
        [AiSkillBody("Add an `InputAction` to an existing `InputActionMap`. An Action represents a single input " +
            "intent (e.g. 'Jump', 'Move', 'Fire').\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` — required. Name of the ActionMap to add the Action to.\n" +
            "- `actionName` — required. Name of the new Action (unique within the map).\n" +
            "- `actionType` — input behavior type: `Button` (default), `Value`, or `PassThrough`.\n" +
            "- `expectedControlType` — optional control layout the action expects (e.g. `Button`, `Vector2`, `Axis`).\n" +
            "- `binding` — optional initial binding control path (e.g. `<Gamepad>/buttonSouth`).\n" +
            "- `groups` / `interactions` / `processors` — optional binding metadata applied to the initial binding.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, adds the Action to the named map (failing if the action name already exists), sets its " +
            "`expectedControlType` when provided, saves and re-imports the asset. Runs on the Unity main thread.")]
        [Description("Adds an Action (type + optional expectedControlType + optional initial binding) to an ActionMap and saves the asset.")]
        public ActionAddResponse AddAction
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap to add the Action to.")]
            string mapName,
            [Description("Name of the new Action (unique within the map).")]
            string actionName,
            [Description("Input behavior type: Button (default), Value, or PassThrough.")]
            InputActionType actionType = InputActionType.Button,
            [Description("Optional expected control type / layout (e.g. 'Button', 'Vector2', 'Axis').")]
            string? expectedControlType = null,
            [Description("Optional initial binding control path (e.g. '<Gamepad>/buttonSouth').")]
            string? binding = null,
            [Description("Optional binding group(s) for the initial binding (semicolon-separated).")]
            string? groups = null,
            [Description("Optional interactions for the initial binding (e.g. 'press,hold').")]
            string? interactions = null,
            [Description("Optional processors for the initial binding (e.g. 'invert').")]
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
                if (map.FindAction(actionName, throwIfNotFound: false) != null)
                    throw new Exception(Error.ActionAlreadyExists(actionName, mapName));

                var action = map.AddAction(
                    name: actionName,
                    type: actionType,
                    binding: string.IsNullOrEmpty(binding) ? null : binding,
                    interactions: string.IsNullOrEmpty(interactions) ? null : interactions,
                    processors: string.IsNullOrEmpty(processors) ? null : processors,
                    groups: string.IsNullOrEmpty(groups) ? null : groups);

                if (!string.IsNullOrEmpty(expectedControlType))
                    action.expectedControlType = expectedControlType;

                SaveAsset(asset);

                return new ActionAddResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionName = actionName,
                    actionType = actionType.ToString(),
                    expectedControlType = action.expectedControlType,
                    bindingCount = action.bindings.Count,
                    success = true
                };
            });
        }

        public class ActionAddResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the ActionMap the Action was added to.")]
            public string mapName = string.Empty;

            [Description("Name of the added Action.")]
            public string actionName = string.Empty;

            [Description("Resolved input behavior type.")]
            public string actionType = string.Empty;

            [Description("Resolved expected control type, or null.")]
            public string? expectedControlType;

            [Description("Number of bindings on the new Action.")]
            public int bindingCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

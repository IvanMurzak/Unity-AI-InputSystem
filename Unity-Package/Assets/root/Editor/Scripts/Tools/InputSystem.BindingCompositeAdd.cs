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
        public const string BindingCompositeAddToolId = "inputsystem-binding-composite-add";

        [AiTool
        (
            BindingCompositeAddToolId,
            Title = "InputSystem / Add Composite Binding",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Add a composite Binding (e.g. `2DVector` / WASD, `1DAxis`) with its named parts to an " +
            "Action in a `.inputactions` asset, then save it.")]
        [AiSkillBody("Add a composite `InputBinding` to an `InputAction`. A composite synthesizes a value (e.g. a " +
            "`Vector2`) from several part bindings. The classic example is a `2DVector` (WASD) composite with " +
            "`up`/`down`/`left`/`right` parts; a `1DAxis` composite has `negative`/`positive` parts.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `mapName` / `actionName` — required. The Action that receives the composite.\n" +
            "- `composite` — composite type, e.g. `2DVector`, `1DAxis`, `Axis`, `Dpad` (default `2DVector`).\n" +
            "- `parts` — required. One or more `{ name, path }` entries (e.g. `up` → `<Keyboard>/w`).\n" +
            "- `interactions` / `processors` — optional, applied to the composite root.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, resolves the Action, adds the composite via `AddCompositeBinding` and chains each part " +
            "via `.With(name, path)`, saves and re-imports the asset. Runs on the Unity main thread.")]
        [Description("Adds a composite Binding (2DVector/1DAxis/etc.) with named parts to an Action and saves the asset.")]
        public BindingCompositeAddResponse AddCompositeBinding
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Name of the ActionMap containing the Action.")]
            string mapName,
            [Description("Name of the Action to add the composite to.")]
            string actionName,
            [Description("Composite parts: each entry has a 'name' (e.g. 'up') and a control 'path' (e.g. '<Keyboard>/w').")]
            CompositePart[] parts,
            [Description("Composite type, e.g. '2DVector' (default), '1DAxis', 'Axis', 'Dpad'.")]
            string composite = "2DVector",
            [Description("Optional interactions applied to the composite root.")]
            string? interactions = null,
            [Description("Optional processors applied to the composite root.")]
            string? processors = null
        )
        {
            if (string.IsNullOrWhiteSpace(mapName))
                throw new ArgumentException("mapName is required.", nameof(mapName));
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("actionName is required.", nameof(actionName));
            if (string.IsNullOrWhiteSpace(composite))
                throw new ArgumentException("composite is required.", nameof(composite));
            if (parts == null || parts.Length == 0)
                throw new ArgumentException("At least one composite part is required.", nameof(parts));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                var map = GetActionMap(asset, mapName);
                var action = GetAction(map, actionName);

                var syntax = action.AddCompositeBinding(
                    composite,
                    interactions: string.IsNullOrEmpty(interactions) ? null : interactions,
                    processors: string.IsNullOrEmpty(processors) ? null : processors);

                foreach (var part in parts)
                {
                    if (part == null || string.IsNullOrWhiteSpace(part.name) || string.IsNullOrWhiteSpace(part.path))
                        throw new ArgumentException("Each composite part requires a non-empty 'name' and 'path'.", nameof(parts));
                    syntax = syntax.With(part.name, part.path, groups: string.IsNullOrEmpty(part.groups) ? null : part.groups);
                }

                SaveAsset(asset);

                return new BindingCompositeAddResponse
                {
                    assetPath = Normalize(assetPath),
                    mapName = mapName,
                    actionName = actionName,
                    composite = composite,
                    partCount = parts.Length,
                    bindingCount = action.bindings.Count,
                    success = true
                };
            });
        }

        public class CompositePart
        {
            [Description("Part name within the composite (e.g. 'up', 'down', 'left', 'right', 'negative', 'positive').")]
            public string name = string.Empty;

            [Description("Control path bound to this part (e.g. '<Keyboard>/w').")]
            public string path = string.Empty;

            [Description("Optional control-scheme group(s) for this part.")]
            public string? groups;
        }

        public class BindingCompositeAddResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the ActionMap.")]
            public string mapName = string.Empty;

            [Description("Name of the Action.")]
            public string actionName = string.Empty;

            [Description("The composite type that was added.")]
            public string composite = string.Empty;

            [Description("Number of composite parts added.")]
            public int partCount;

            [Description("Number of bindings on the Action after the addition (composite root + parts).")]
            public int bindingCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

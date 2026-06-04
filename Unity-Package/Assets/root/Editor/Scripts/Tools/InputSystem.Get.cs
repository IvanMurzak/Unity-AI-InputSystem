/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using UnityEngine.InputSystem;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string GetToolId = "inputsystem-get";

        [AiTool
        (
            GetToolId,
            Title = "InputSystem / Get Asset Structure",
            ReadOnlyHint = true,
            DestructiveHint = false,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Read the full structure of a `.inputactions` InputActionAsset — its ActionMaps, Actions " +
            "(type / expectedControlType), Bindings (path / groups / interactions / processors / index) and Control " +
            "Schemes. Read-only.")]
        [AiSkillBody("Inspect an `InputActionAsset` and return its structure so you can discover map names, action " +
            "names, and binding indices to drive the other tools.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset and walks its ActionMaps → Actions → Bindings plus its Control Schemes, returning a " +
            "compact structured summary (including each binding's zero-based index, whether it is a composite, and " +
            "its part metadata). Read-only. Runs on the Unity main thread.")]
        [Description("Reads an InputActionAsset's full structure (maps, actions, bindings, control schemes). Read-only.")]
        public GetResponse GetAssetData
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath
        )
        {
            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);

                var maps = new List<MapInfo>();
                foreach (var map in asset.actionMaps)
                {
                    var actions = new List<ActionInfo>();
                    foreach (var action in map.actions)
                    {
                        var bindings = new List<BindingInfo>();
                        for (int i = 0; i < action.bindings.Count; i++)
                        {
                            var b = action.bindings[i];
                            bindings.Add(new BindingInfo
                            {
                                index = i,
                                path = b.path,
                                name = b.name,
                                groups = b.groups,
                                interactions = b.interactions,
                                processors = b.processors,
                                isComposite = b.isComposite,
                                isPartOfComposite = b.isPartOfComposite
                            });
                        }
                        actions.Add(new ActionInfo
                        {
                            name = action.name,
                            type = action.type.ToString(),
                            expectedControlType = action.expectedControlType,
                            bindings = bindings.ToArray()
                        });
                    }
                    maps.Add(new MapInfo
                    {
                        name = map.name,
                        actions = actions.ToArray()
                    });
                }

                var schemes = new List<SchemeInfo>();
                foreach (var scheme in asset.controlSchemes)
                {
                    var devices = new List<string>();
                    foreach (var req in scheme.deviceRequirements)
                        devices.Add((req.isOptional ? "(optional) " : "") + req.controlPath);
                    schemes.Add(new SchemeInfo
                    {
                        name = scheme.name,
                        bindingGroup = scheme.bindingGroup,
                        devices = devices.ToArray()
                    });
                }

                return new GetResponse
                {
                    assetPath = Normalize(assetPath),
                    actionMaps = maps.ToArray(),
                    controlSchemes = schemes.ToArray()
                };
            });
        }

        public class GetResponse
        {
            [Description("Path of the inspected asset.")]
            public string assetPath = string.Empty;

            [Description("ActionMaps in the asset.")]
            public MapInfo[] actionMaps = Array.Empty<MapInfo>();

            [Description("Control schemes in the asset.")]
            public SchemeInfo[] controlSchemes = Array.Empty<SchemeInfo>();
        }

        public class MapInfo
        {
            [Description("ActionMap name.")]
            public string name = string.Empty;

            [Description("Actions in the map.")]
            public ActionInfo[] actions = Array.Empty<ActionInfo>();
        }

        public class ActionInfo
        {
            [Description("Action name.")]
            public string name = string.Empty;

            [Description("Action type (Button / Value / PassThrough).")]
            public string type = string.Empty;

            [Description("Expected control type, or null.")]
            public string? expectedControlType;

            [Description("Bindings on the action (with their indices).")]
            public BindingInfo[] bindings = Array.Empty<BindingInfo>();
        }

        public class BindingInfo
        {
            [Description("Zero-based index of the binding within the action.")]
            public int index;

            [Description("Control path, or empty for a composite root.")]
            public string? path;

            [Description("Binding/part name (e.g. composite part 'up'), or empty.")]
            public string? name;

            [Description("Control-scheme group(s).")]
            public string? groups;

            [Description("Interactions.")]
            public string? interactions;

            [Description("Processors.")]
            public string? processors;

            [Description("True if this binding is a composite root.")]
            public bool isComposite;

            [Description("True if this binding is a part of a composite.")]
            public bool isPartOfComposite;
        }

        public class SchemeInfo
        {
            [Description("Control scheme name.")]
            public string name = string.Empty;

            [Description("Binding group string for the scheme.")]
            public string? bindingGroup;

            [Description("Device requirements (prefixed '(optional)' when optional).")]
            public string[] devices = Array.Empty<string>();
        }
    }
}

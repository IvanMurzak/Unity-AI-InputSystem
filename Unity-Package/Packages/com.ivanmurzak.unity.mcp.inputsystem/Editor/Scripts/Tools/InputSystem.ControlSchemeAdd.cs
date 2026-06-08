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
        public const string ControlSchemeAddToolId = "inputsystem-controlscheme-add";

        [AiTool
        (
            ControlSchemeAddToolId,
            Title = "InputSystem / Add Control Scheme",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Add a Control Scheme (with optional required / optional device requirements) to a " +
            "`.inputactions` asset, then save it.")]
        [AiSkillBody("Add an `InputControlScheme` to an InputActionAsset. A control scheme groups bindings by a set " +
            "of devices (e.g. a 'Keyboard&Mouse' scheme and a 'Gamepad' scheme), letting the same Actions support " +
            "multiple input setups.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `schemeName` — required. Unique control-scheme name.\n" +
            "- `requiredDevices` — optional device control paths that MUST be present (e.g. `<Gamepad>`).\n" +
            "- `optionalDevices` — optional device control paths that MAY be present (e.g. `<Mouse>`).\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, adds the control scheme via `AddControlScheme`, registers each required / optional " +
            "device requirement, saves and re-imports the asset. Runs on the Unity main thread.")]
        [Description("Adds a Control Scheme (with required/optional device requirements) to an InputActionAsset and saves it.")]
        public ControlSchemeAddResponse AddControlScheme
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("Unique name of the new control scheme.")]
            string schemeName,
            [Description("Optional required device control paths (e.g. '<Gamepad>'). All must be present for the scheme to be usable.")]
            string[]? requiredDevices = null,
            [Description("Optional optional-device control paths (e.g. '<Mouse>').")]
            string[]? optionalDevices = null
        )
        {
            if (string.IsNullOrWhiteSpace(schemeName))
                throw new ArgumentException("schemeName is required.", nameof(schemeName));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);
                foreach (var existing in asset.controlSchemes)
                {
                    if (string.Equals(existing.name, schemeName, StringComparison.OrdinalIgnoreCase))
                        throw new Exception(Error.ControlSchemeAlreadyExists(schemeName));
                }

                var syntax = asset.AddControlScheme(schemeName);
                if (requiredDevices != null)
                {
                    foreach (var dev in requiredDevices)
                    {
                        if (!string.IsNullOrWhiteSpace(dev))
                            syntax = syntax.WithRequiredDevice(dev);
                    }
                }
                if (optionalDevices != null)
                {
                    foreach (var dev in optionalDevices)
                    {
                        if (!string.IsNullOrWhiteSpace(dev))
                            syntax = syntax.WithOptionalDevice(dev);
                    }
                }
                syntax.Done();

                SaveAsset(asset);

                return new ControlSchemeAddResponse
                {
                    assetPath = Normalize(assetPath),
                    schemeName = schemeName,
                    requiredDeviceCount = requiredDevices?.Length ?? 0,
                    optionalDeviceCount = optionalDevices?.Length ?? 0,
                    controlSchemeCount = asset.controlSchemes.Count,
                    success = true
                };
            });
        }

        public class ControlSchemeAddResponse
        {
            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Name of the added control scheme.")]
            public string schemeName = string.Empty;

            [Description("Number of required devices registered.")]
            public int requiredDeviceCount;

            [Description("Number of optional devices registered.")]
            public int optionalDeviceCount;

            [Description("Total number of control schemes in the asset.")]
            public int controlSchemeCount;

            [Description("Whether the operation succeeded.")]
            public bool success;
        }
    }
}

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
using System.Linq;
using com.IvanMurzak.McpPlugin;
using Microsoft.Extensions.Logging;
using com.IvanMurzak.ReflectorNet.Model;
using com.IvanMurzak.ReflectorNet.Utils;
using com.IvanMurzak.Unity.MCP.Utils;
using UnityEngine.InputSystem;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string ModifyToolId = "inputsystem-modify";

        [AiTool
        (
            ModifyToolId,
            Title = "InputSystem / Modify Asset (generic)",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Generic write escape-hatch: apply a `SerializedMember` diff to the `InputActionAsset` " +
            "object itself via ReflectorNet `TryModify`, for fields not covered by the dedicated tools. Use " +
            "'inputsystem-get' first to inspect the structure.")]
        [AiSkillBody("Modify an `InputActionAsset` by applying a `SerializedMember` diff via ReflectorNet. This is " +
            "the generic escape hatch for asset-level fields/properties that the dedicated tools do not expose.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. Path to the existing `.inputactions` asset.\n" +
            "- `data` — the `SerializedMember` diff to apply. Route **fields** through the `fields` channel and " +
            "**properties** through the `props` channel — ReflectorNet resolves them separately with no cross-fallback.\n\n" +
            "## Behavior\n\n" +
            "Loads the asset, applies the diff via `Reflector.TryModify`, and on success re-serializes and saves the " +
            "asset back to disk. The applied logs are returned. Runs on the Unity main thread.")]
        [Description("Generic: apply a SerializedMember diff to an InputActionAsset via ReflectorNet TryModify (fields via 'fields', props via 'props'). Use inputsystem-get first.")]
        public ModifyResponse ModifyAsset
        (
            [Description("'Assets/'-rooted path to the existing '.inputactions' asset.")]
            string assetPath,
            [Description("The SerializedMember diff to apply. Fields go through the 'fields' channel; properties through 'props'.")]
            SerializedMember data
        )
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return MainThread.Instance.Run(() =>
            {
                var asset = LoadAsset(assetPath);

                var reflector = UnityMcpPluginEditor.Instance.Reflector ?? throw new Exception(Error.ReflectorNotAvailable());
                var logger = UnityLoggerFactory.LoggerFactory.CreateLogger<Tool_InputSystem>();

                var response = new ModifyResponse
                {
                    assetPath = Normalize(assetPath),
                    assetType = asset.GetType().FullName ?? asset.GetType().Name
                };

                var logs = new List<string>();
                var modifyLogs = new Logs();
                object? boxed = asset;
                if (reflector.TryModify(ref boxed, data, logs: modifyLogs, logger: logger))
                {
                    response.success = true;
                    logs.Add("InputActionAsset modified successfully.");
                    SaveAsset(asset);
                }
                else
                {
                    logs.Add("No modifications were made.");
                }
                logs.AddRange(modifyLogs.Select(l => l.ToString()));

                response.logs = logs.ToArray();
                return response;
            });
        }

        public class ModifyResponse
        {
            [Description("Whether the modification was successful.")]
            public bool success;

            [Description("Path of the modified asset.")]
            public string assetPath = string.Empty;

            [Description("Full type name of the modified asset.")]
            public string assetType = string.Empty;

            [Description("Log of modifications and any warnings/errors.")]
            public string[]? logs;
        }
    }
}

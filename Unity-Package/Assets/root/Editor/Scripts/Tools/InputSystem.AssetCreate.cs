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
using System.IO;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        public const string AssetCreateToolId = "inputsystem-asset-create";

        [AiTool
        (
            AssetCreateToolId,
            Title = "InputSystem / Create InputActionAsset",
            ReadOnlyHint = false,
            DestructiveHint = false,
            IdempotentHint = false,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Create a new `.inputactions` InputActionAsset at an 'Assets/'-rooted path, optionally " +
            "seeding an initial ActionMap. Returns the created asset path.")]
        [AiSkillBody("Create a new Unity Input System `InputActionAsset` (a `.inputactions` file). This is the " +
            "container for ActionMaps, Actions, Bindings and Control Schemes.\n\n" +
            "## Inputs\n\n" +
            "- `assetPath` — required. An 'Assets/'-rooted path ending in `.inputactions` (e.g. " +
            "`Assets/Input/Player.inputactions`). Intermediate folders must already exist (use 'assets-create-folder').\n" +
            "- `initialActionMap` — optional. When provided, an empty ActionMap with this name is added to the new asset.\n\n" +
            "## Behavior\n\n" +
            "Creates an empty `InputActionAsset`, optionally adds the initial ActionMap, serializes it to JSON, writes " +
            "the file, and imports it into the AssetDatabase. Fails if an asset already exists at the path. Runs on " +
            "the Unity main thread.")]
        [Description("Creates a new .inputactions InputActionAsset at the given Assets/ path, optionally with an initial ActionMap.")]
        public AssetCreateResponse CreateAsset
        (
            [Description("'Assets/'-rooted path ending in '.inputactions' for the new asset.")]
            string assetPath,
            [Description("Optional name for an initial ActionMap to add to the new asset.")]
            string? initialActionMap = null
        )
        {
            ValidateAssetPath(assetPath);

            return MainThread.Instance.Run(() =>
            {
                var normalized = Normalize(assetPath);
                if (AssetDatabase.LoadAssetAtPath<InputActionAsset>(normalized) != null)
                    throw new Exception(Error.AssetAlreadyExists(normalized));

                var asset = ScriptableObject.CreateInstance<InputActionAsset>();
                asset.name = Path.GetFileNameWithoutExtension(normalized);

                if (!string.IsNullOrEmpty(initialActionMap))
                    asset.AddActionMap(initialActionMap!);

                File.WriteAllText(normalized, ToSafeJson(asset));
                AssetDatabase.ImportAsset(normalized, ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();

                return new AssetCreateResponse
                {
                    assetPath = normalized,
                    initialActionMap = string.IsNullOrEmpty(initialActionMap) ? null : initialActionMap,
                    success = true
                };
            });
        }

        public class AssetCreateResponse
        {
            [Description("Path of the created InputActionAsset.")]
            public string assetPath = string.Empty;

            [Description("Name of the initial ActionMap that was added, or null.")]
            public string? initialActionMap;

            [Description("Whether the asset was created successfully.")]
            public bool success;
        }
    }
}

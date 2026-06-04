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
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_InputSystem
    {
        const string AssetExtension = ".inputactions";

        /// <summary>Validate an 'Assets/'-rooted .inputactions path (throws on failure).</summary>
        static void ValidateAssetPath(string? assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException(Error.AssetPathRequired(), nameof(assetPath));

            var normalized = Normalize(assetPath!);
            if (!normalized.StartsWith("Assets/", StringComparison.Ordinal) ||
                !normalized.EndsWith(AssetExtension, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(Error.AssetPathInvalid(assetPath!), nameof(assetPath));
        }

        static string Normalize(string path) => path.Replace('\\', '/');

        /// <summary>Load an existing InputActionAsset from disk (throws when missing).</summary>
        static InputActionAsset LoadAsset(string assetPath)
        {
            ValidateAssetPath(assetPath);
            var normalized = Normalize(assetPath);
            var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(normalized);
            if (asset == null)
                throw new Exception(Error.AssetNotFound(normalized));
            return asset;
        }

        /// <summary>Find an ActionMap by name (throws when missing).</summary>
        static InputActionMap GetActionMap(InputActionAsset asset, string mapName)
        {
            var map = asset.FindActionMap(mapName, throwIfNotFound: false);
            if (map == null)
                throw new Exception(Error.ActionMapNotFound(mapName));
            return map;
        }

        /// <summary>Find an Action within a map by name (throws when missing).</summary>
        static InputAction GetAction(InputActionMap map, string actionName)
        {
            var action = map.FindAction(actionName, throwIfNotFound: false);
            if (action == null)
                throw new Exception(Error.ActionNotFound(actionName, map.name));
            return action;
        }

        /// <summary>
        /// Persist the in-memory InputActionAsset back to its .inputactions file as serialized JSON
        /// and re-import so the AssetDatabase reflects the change.
        /// </summary>
        static void SaveAsset(InputActionAsset asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
                throw new Exception("[Error] The InputActionAsset is not backed by a file on disk; cannot save.");

            File.WriteAllText(path, ToSafeJson(asset));
            EditorUtility.SetDirty(asset);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Serialize an InputActionAsset to JSON, guarding the InputSystem 1.x bug where
        /// <c>ToJson()</c> throws inside <c>WriteFileJson.FromMaps</c> when the asset has no
        /// ActionMaps (its internal map array is null). Emits minimal valid JSON in that case.
        /// </summary>
        static string ToSafeJson(InputActionAsset asset)
        {
            if (asset.actionMaps.Count == 0)
                return "{\n    \"name\": \"" + asset.name + "\",\n    \"maps\": [],\n    \"controlSchemes\": []\n}";
            return asset.ToJson();
        }
    }
}

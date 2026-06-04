/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
│  Licensed under the MIT License.                                            │
│  See the LICENSE file in the project root for more information.             │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using com.IvanMurzak.McpPlugin.Common.Model;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.InputSystem.Editor.Tests
{
    public class BaseTest : com.IvanMurzak.Unity.MCP.Editor.Tests.BaseTest
    {
        protected const string TestFolder = "Assets/InputSystemToolTests";
        protected const string MapName = "Player";

        readonly List<string> _createdAssets = new();

        /// <summary>Return a fresh, unique 'Assets/'-rooted .inputactions path under the test folder.</summary>
        protected string NewAssetPath(string fileName)
        {
            if (!AssetDatabase.IsValidFolder(TestFolder))
                AssetDatabase.CreateFolder("Assets", "InputSystemToolTests");

            var path = $"{TestFolder}/{fileName}_{Guid.NewGuid():N}.inputactions";
            _createdAssets.Add(path);
            return path;
        }

        [TearDown]
        public void CleanupCreatedAssets()
        {
            foreach (var path in _createdAssets)
            {
                if (File.Exists(path))
                    AssetDatabase.DeleteAsset(path);
            }
            _createdAssets.Clear();
            if (AssetDatabase.IsValidFolder(TestFolder))
                AssetDatabase.DeleteAsset(TestFolder);
            AssetDatabase.Refresh();
        }

        protected virtual ResponseData<ResponseCallTool> RunToolAllowWarnings(string toolName, string json)
        {
            var reflector = UnityMcpPluginEditor.Instance.Reflector ?? throw new Exception("Reflector not available.");

            Debug.Log($"{toolName} Started with JSON:\n{json}");

            var parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            var request = new RequestCallTool(toolName, parameters!);
            var task = UnityMcpPluginEditor.Instance.Tools!.RunCallTool(request);
            var result = task.Result;

            Debug.Log($"{toolName} Completed");

            var jsonResult = result.ToJson(reflector);
            Debug.Log($"{toolName} Result:\n{jsonResult}");

            Assert.IsFalse(result.Status == ResponseStatus.Error, $"Tool call failed with error status: {result.Message}");
            Assert.IsNotNull(result.Message, "Tool call returned null message");
            Assert.IsFalse(result.Message!.Contains("[Error]"), $"Tool call failed with error: {result.Message}");
            Assert.IsNotNull(result.Value, "Tool call returned null value");
            Assert.IsFalse(jsonResult!.Contains("[Error]"), $"Tool call failed with error in JSON: {jsonResult}");

            return result;
        }
    }
}

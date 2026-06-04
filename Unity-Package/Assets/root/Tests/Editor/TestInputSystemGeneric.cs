/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System;
using System.Collections;
using com.IvanMurzak.ReflectorNet.Model;
using com.IvanMurzak.Unity.MCP.Editor.API;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace com.IvanMurzak.Unity.MCP.InputSystem.Editor.Tests
{
    public class TestInputSystemGeneric : BaseTest
    {
        /// <summary>A plain target carrying a public *field* (not a property) so we exercise the 'fields' channel.</summary>
        public class FieldTarget
        {
            public int sensitivity;
        }

        [UnityTest]
        public IEnumerator Get_ReturnsAssetStructure()
        {
            var tool = new Tool_InputSystem();
            var path = NewAssetPath("Inspect");

            tool.CreateAsset(path, initialActionMap: MapName);
            tool.AddAction(path, MapName, "Jump", expectedControlType: "Button");
            tool.AddBinding(path, MapName, "Jump", "<Keyboard>/space");

            var data = tool.GetAssetData(path);
            Assert.AreEqual(path, data.assetPath, "Asset path should round-trip");
            Assert.AreEqual(1, data.actionMaps.Length, "There should be one action map");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Modify_ViaFieldsChannel_ReflectorNet()
        {
            var reflector = UnityMcpPluginEditor.Instance.Reflector ?? throw new Exception("Reflector not available.");

            var target = new FieldTarget { sensitivity = 1 };

            // 'sensitivity' is a public *field*, so it must be supplied through the 'fields' channel
            // (AddField). ReflectorNet's TryModify resolves 'props' as PropertyInfo only and 'fields'
            // as FieldInfo only — no cross-fallback. This mirrors the generic inputsystem-modify path.
            var diff = SerializedMember.FromValue(
                    reflector: reflector,
                    name: target.GetType().Name,
                    type: typeof(FieldTarget),
                    value: null)
                .AddField(SerializedMember.FromValue(
                    reflector: reflector,
                    name: nameof(FieldTarget.sensitivity),
                    value: 42));

            object? boxed = target;
            var ok = reflector.TryModify(ref boxed, diff);

            Assert.IsTrue(ok, "Modify through the 'fields' channel should succeed");
            Assert.AreEqual(42, ((FieldTarget)boxed!).sensitivity, "The field should be updated via the 'fields' channel");

            yield return null;
        }
    }
}

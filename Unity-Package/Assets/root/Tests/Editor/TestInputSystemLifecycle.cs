/*
┌─────────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                        │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-InputSystem)    │
│  Copyright (c) 2025 Ivan Murzak                                             │
│  Licensed under the MIT License.                                            │
└─────────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System.Collections;
using System.Linq;
using com.IvanMurzak.Unity.MCP.Editor.API;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace com.IvanMurzak.Unity.MCP.InputSystem.Editor.Tests
{
    public class TestInputSystemLifecycle : BaseTest
    {
        [UnityTest]
        public IEnumerator Create_Then_AddMap_AddAction_AddBinding()
        {
            var tool = new Tool_InputSystem();
            var path = NewAssetPath("Player");

            var create = tool.CreateAsset(path, initialActionMap: MapName);
            Assert.IsTrue(create.success, "Asset should be created");

            var action = tool.AddAction(path, MapName, "Jump",
                actionType: UnityEngine.InputSystem.InputActionType.Button,
                expectedControlType: "Button");
            Assert.IsTrue(action.success, "Action should be added");
            Assert.AreEqual("Button", action.actionType);

            var binding = tool.AddBinding(path, MapName, "Jump", "<Keyboard>/space");
            Assert.IsTrue(binding.success, "Binding should be added");
            Assert.AreEqual(0, binding.bindingIndex, "First binding index should be 0");

            // Verify via the get tool.
            var data = tool.GetAssetData(path);
            var map = data.actionMaps.Single(m => m.name == MapName);
            var jump = map.actions.Single(a => a.name == "Jump");
            Assert.AreEqual("Button", jump.expectedControlType);
            Assert.IsTrue(jump.bindings.Any(b => b.path == "<Keyboard>/space"),
                "Jump should carry the space binding");

            yield return null;
        }

        [UnityTest]
        public IEnumerator AddCompositeBinding_2DVector_WASD()
        {
            var tool = new Tool_InputSystem();
            var path = NewAssetPath("Move");

            tool.CreateAsset(path, initialActionMap: MapName);
            tool.AddAction(path, MapName, "Move",
                actionType: UnityEngine.InputSystem.InputActionType.Value,
                expectedControlType: "Vector2");

            var parts = new[]
            {
                new Tool_InputSystem.CompositePart { name = "up",    path = "<Keyboard>/w" },
                new Tool_InputSystem.CompositePart { name = "down",  path = "<Keyboard>/s" },
                new Tool_InputSystem.CompositePart { name = "left",  path = "<Keyboard>/a" },
                new Tool_InputSystem.CompositePart { name = "right", path = "<Keyboard>/d" },
            };
            var composite = tool.AddCompositeBinding(path, MapName, "Move", parts, composite: "2DVector");
            Assert.IsTrue(composite.success, "Composite binding should be added");
            Assert.AreEqual(4, composite.partCount);

            var data = tool.GetAssetData(path);
            var move = data.actionMaps.Single(m => m.name == MapName).actions.Single(a => a.name == "Move");
            Assert.IsTrue(move.bindings.Any(b => b.isComposite), "There should be a composite root binding");
            Assert.AreEqual(4, move.bindings.Count(b => b.isPartOfComposite), "There should be 4 composite parts");

            yield return null;
        }

        [UnityTest]
        public IEnumerator SetBinding_And_RemoveBinding()
        {
            var tool = new Tool_InputSystem();
            var path = NewAssetPath("Fire");

            tool.CreateAsset(path, initialActionMap: MapName);
            tool.AddAction(path, MapName, "Fire");
            tool.AddBinding(path, MapName, "Fire", "<Mouse>/leftButton");

            var set = tool.SetBinding(path, MapName, "Fire", bindingIndex: 0,
                path: "<Gamepad>/rightTrigger", interactions: "press");
            Assert.IsTrue(set.success, "Binding should be updated");
            Assert.AreEqual("<Gamepad>/rightTrigger", set.path);
            Assert.AreEqual("press", set.interactions);

            var remove = tool.RemoveBinding(path, MapName, "Fire", bindingIndex: 0);
            Assert.IsTrue(remove.success, "Binding should be removed");
            Assert.AreEqual(0, remove.bindingCount, "No bindings should remain");

            yield return null;
        }

        [UnityTest]
        public IEnumerator AddControlScheme_And_RemoveMapAndAction()
        {
            var tool = new Tool_InputSystem();
            var path = NewAssetPath("Schemes");

            tool.CreateAsset(path, initialActionMap: MapName);
            tool.AddAction(path, MapName, "Look");

            var scheme = tool.AddControlScheme(path, "Gamepad",
                requiredDevices: new[] { "<Gamepad>" });
            Assert.IsTrue(scheme.success, "Control scheme should be added");
            Assert.AreEqual(1, scheme.requiredDeviceCount);

            var get = tool.GetAssetData(path);
            Assert.IsTrue(get.controlSchemes.Any(s => s.name == "Gamepad"), "Gamepad scheme should be present");

            var removeAction = tool.RemoveAction(path, MapName, "Look");
            Assert.IsTrue(removeAction.success, "Action should be removed");

            var removeMap = tool.RemoveActionMap(path, MapName);
            Assert.IsTrue(removeMap.success, "Map should be removed");
            Assert.AreEqual(0, removeMap.actionMapCount, "No maps should remain");

            yield return null;
        }
    }
}

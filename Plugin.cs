using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectPlacementTool.ExamplePatches;
using ObjectPlacementTool.Helpers;
using UnityEngine;
using ObjectPlacementTool.Patches;
using EFT.InputSystem;
using EFT;
using Comfort.Net;
using Comfort.Common;
using EFT.UI;

namespace ObjectPlacementTool
{
    [BepInPlugin("Jehree.ObjectPlacementTool", "ObjectPlacementTool", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_NAME = "Object Placement Tool";

        public static ManualLogSource LogSource;
        public static GameObject TargetObject;
        public static Player Player;
        public static NoteWindow NoteUIPanel;
        public static InventoryScreen InventoryUI;
        public static TasksScreen TasksUI;

        public static InputMode Mode = InputMode.Translate;
        public static bool LockInput = false;
        public enum InputMode
        {
            Translate,
            Scale,
            Rotate
        }

        private void Awake()
        {
            LogSource = Logger;
            Settings.Init(Config);

            new GameStartedPatch().Enable();
            new GameEndedPatch().Enable();
            new GetAvailableActionsPatch().Enable();
            new NoteWindowAwakePatch().Enable();
            new InventoryScreenAwakePatch().Enable();
            new TasksScreenAwakePatch().Enable();
        }

        private void Update()
        {

            if (!Settings.ModEnabled.Value || LockInput) return;
            if (Player == null) return;

            if (Settings.SpawnObjectKey.Value.IsDown())
            {
                SpawnObject();
            }

            if (TargetObject == null) return;

            if (Settings.ResetKey.Value.IsDown())
            {
                switch (Mode)
                {
                    case InputMode.Rotate:
                    {
                        TargetObject.transform.rotation = Quaternion.identity;
                        break;
                    }
                    case InputMode.Translate:
                    {
                        TargetObject.transform.position = Player.Transform.position;
                        break;
                    }
                    case InputMode.Scale:
                    {
                        TargetObject.transform.localScale = new Vector3(1, 1, 1);
                        break;
                    }
                }
            }
            if (Settings.TranslateKey.Value.IsDown())
            {
                Mode = InputMode.Translate;
                TargetObject.GetComponent<Renderer>().material.color = Color.green;
            }
            if (Settings.ScaleKey.Value.IsDown())
            {
                Mode = InputMode.Scale;
                TargetObject.GetComponent<Renderer>().material.color = Color.blue;
            }
            if (Settings.RotateKey.Value.IsDown())
            {
                Mode = InputMode.Rotate;
                TargetObject.GetComponent<Renderer>().material.color = Color.red;
            }

            float delta = Time.deltaTime;
            float speed = Settings.ChangeSpeed.Value;

            switch (Mode)
            {
                case InputMode.Rotate:
                {
                    HandleRotation(speed, delta);
                    break;
                }
                case InputMode.Translate:
                {
                    HandleTranslation(speed, delta);
                    break;
                }
                case InputMode.Scale:
                {
                    HandleScaling(speed, delta);
                    break;
                }
            }

        }

        public static void SpawnObject()
        {
            if (NoteUIPanel == null)
            {
                EFT.UI.ConsoleScreen.LogError($"{MOD_NAME}: Before you can spawn or interact with any object, go to your tasks screen and click `Add Note`. I know it's weird, but it is inactive then the game starts so the mod can't find it. After opening it once you won't need to for the rest of the raid.");
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ErrorMessage);
                return;
            }

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<Renderer>().enabled = true;
            cube.transform.position = Singleton<GameWorld>.Instance.MainPlayer.Transform.position;
            cube.name = "target_object";
            cube.AddComponent<InteractableComponent>();
            cube.GetComponent<InteractableComponent>().Init();

            Plugin.SelectObject(cube);
        }

        public static void SelectObject(GameObject obj)
        {
            if (NoteUIPanel == null)
            {
                LogSource.LogError("You need to open a note window to so the mod can ref that game object before you select any objects!");
                return;
            }

            UnselectObject();
            TargetObject = obj;
            Mode = InputMode.Translate;
            TargetObject.GetComponent<Renderer>().material.color = Color.green;
        }

        public static void UnselectObject()
        {
            if (TargetObject == null) return;
            TargetObject.GetComponent<Renderer>().material.color = Color.magenta;
            TargetObject = null;
        }

        public static void HandleScaling(float speed, float delta)
        {
            if (Settings.PositiveXKey.Value.IsPressed())
            {
                ScaleTargetObject("x", speed * delta);
            }
            if (Settings.NegativeXKey.Value.IsPressed())
            {
                ScaleTargetObject("x", -(speed * delta));
            }
            if (Settings.PositiveYKey.Value.IsPressed())
            {
                ScaleTargetObject("y", speed * delta);
            }
            if (Settings.NegativeYKey.Value.IsPressed())
            {
                ScaleTargetObject("y", -(speed * delta));
            }
            if (Settings.PositiveZKey.Value.IsPressed())
            {
                ScaleTargetObject("z", speed * delta);
            }
            if (Settings.NegativeZKey.Value.IsPressed())
            {
                ScaleTargetObject("z", -(speed * delta));
            }
        }

        public static void HandleRotation(float speed, float delta)
        {
            float rotSpeed = speed * 25;

            if (Settings.PositiveXKey.Value.IsPressed())
            {
                RotateTargetObject("x", rotSpeed * delta);
            }
            if (Settings.NegativeXKey.Value.IsPressed())
            {
                RotateTargetObject("x", -rotSpeed * delta);
            }
            if (Settings.PositiveYKey.Value.IsPressed())
            {
                RotateTargetObject("y", rotSpeed * delta);
            }
            if (Settings.NegativeYKey.Value.IsPressed())
            {
                RotateTargetObject("y", -rotSpeed * delta);
            }
            if (Settings.PositiveZKey.Value.IsPressed())
            {
                RotateTargetObject("z", rotSpeed * delta);
            }
            if (Settings.NegativeZKey.Value.IsPressed())
            {
                RotateTargetObject("z", -rotSpeed * delta);
            }
        }

        public static void HandleTranslation(float speed, float delta)
        {
            if (Settings.PositiveXKey.Value.IsPressed())
            {
                TranslateTargetObject("x", speed * delta);
            }
            if (Settings.NegativeXKey.Value.IsPressed())
            {
                TranslateTargetObject("x", -(speed * delta));
            }
            if (Settings.PositiveYKey.Value.IsPressed())
            {
                TranslateTargetObject("y", speed * delta);
            }
            if (Settings.NegativeYKey.Value.IsPressed())
            {
                TranslateTargetObject("y", -(speed * delta));
            }
            if (Settings.PositiveZKey.Value.IsPressed())
            {
                TranslateTargetObject("z", speed * delta);
            }
            if (Settings.NegativeZKey.Value.IsPressed())
            {
                TranslateTargetObject("z", -(speed * delta));
            }
        }

        public static void TranslateTargetObject(string axis, float amount)
        {
            Vector3 translation = new Vector3(0,0,0);

            switch(axis)
            {
                case "x": translation = new Vector3(amount, 0, 0); break;
                case "y": translation = new Vector3(0, amount, 0); break;
                case "z": translation = new Vector3(0, 0, amount); break;
            }

            TargetObject.transform.position = TargetObject.transform.position + translation;
        }

        public static void ScaleTargetObject(string axis, float amount)
        {
            Vector3 scaleAmount = new Vector3(0, 0, 0);

            switch (axis)
            {
                case "x": scaleAmount = new Vector3(amount, 0, 0); break;
                case "y": scaleAmount = new Vector3(0, amount, 0); break;
                case "z": scaleAmount = new Vector3(0, 0, amount); break;
            }

            TargetObject.transform.localScale = TargetObject.transform.localScale + scaleAmount;
        }

        public static void RotateTargetObject(string axis, float amount)
        {
            Vector3 rotation = new Vector3(0, 0, 0);

            switch (axis)
            {
                case "x": rotation = new Vector3(amount, 0, 0); break;
                case "y": rotation = new Vector3(0, amount, 0); break;
                case "z": rotation = new Vector3(0, 0, amount); break;
            }

            TargetObject.transform.rotation = TargetObject.transform.rotation * Quaternion.Euler(rotation);
        }
    }
}

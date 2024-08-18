using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZonePlacementTool.ExamplePatches;
using ZonePlacementTool.Helpers;
using UnityEngine;
using ZonePlacementTool.Patches;
using EFT.InputSystem;
using EFT;
using Comfort.Net;
using Comfort.Common;
using EFT.UI;

namespace ZonePlacementTool
{
    [BepInPlugin("Jehree.ZonePlacementTool", "ZonePlacementTool", "1.0.0")] 
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_NAME = "Jehree's Zone Placement Tool";

        public static ManualLogSource LogSource;
        public static InteractableComponent TargetInteractableComponent;
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
        }

        private void Update()
        {

            if (!Settings.ModEnabled.Value || LockInput) return;
            if (Player == null) return;
            if (TargetInteractableComponent == null) return;

            if (Settings.TranslateKey.Value.IsDown())
            {
                Mode = InputMode.Translate;
                TargetInteractableComponent.SetColor(Color.green);
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModFunc);
            }
            if (Settings.ScaleKey.Value.IsDown())
            {
                Mode = InputMode.Scale;
                TargetInteractableComponent.SetColor(Color.blue);
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModGear);

            }
            if (Settings.RotateKey.Value.IsDown())
            {
                Mode = InputMode.Rotate;
                TargetInteractableComponent.SetColor(Color.red);
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModVital);
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

        public static void SelectObject(GameObject obj)
        {
            UnselectObject();
            TargetInteractableComponent = obj.GetComponent<InteractableComponent>();
            Mode = InputMode.Translate;
            TargetInteractableComponent.SetColor(Color.green);
            Settings.SelectedObjectName.Value = TargetInteractableComponent.GetName();
        }

        public static void UnselectObject()
        {
            if (TargetInteractableComponent == null) return;
            if (Settings.SelectedObjectName.Value == "")
            {
                ConsoleScreen.LogError($"{MOD_NAME}: Your object has an empty name! SOMETHING WILL EXPLODE AHHH FIX IT NOW");
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ErrorMessage);
            }
            TargetInteractableComponent.SetName(Settings.SelectedObjectName.Value);
            TargetInteractableComponent.SetColor(Color.magenta);
            TargetInteractableComponent = null;
            Settings.SelectedObjectName.Value = "";
        }

        public static void HandleScaling(float speed, float delta)
        {
            if (Settings.PositiveXKey.Value.IsPressed())
            {
                TargetInteractableComponent.ScaleMe("x", speed * delta);
            }
            if (Settings.NegativeXKey.Value.IsPressed())
            {
                TargetInteractableComponent.ScaleMe("x", -(speed * delta));
            }
            if (Settings.PositiveYKey.Value.IsPressed())
            {
                TargetInteractableComponent.ScaleMe("y", speed * delta);
            }
            if (Settings.NegativeYKey.Value.IsPressed())
            {
                TargetInteractableComponent.ScaleMe("y", -(speed * delta));
            }
            if (Settings.PositiveZKey.Value.IsPressed())
            {
                TargetInteractableComponent.ScaleMe("z", speed * delta);
            }
            if (Settings.NegativeZKey.Value.IsPressed())
            {
                TargetInteractableComponent.ScaleMe("z", -(speed * delta));
            }
        }

        public static void HandleRotation(float speed, float delta)
        {
            float rotSpeed = speed * 25;

            if (Settings.PositiveXKey.Value.IsPressed())
            {
                TargetInteractableComponent.RotateMe("x", rotSpeed * delta);
            }
            if (Settings.NegativeXKey.Value.IsPressed())
            {
                TargetInteractableComponent.RotateMe("x", -rotSpeed * delta);
            }
            if (Settings.PositiveYKey.Value.IsPressed())
            {
                TargetInteractableComponent.RotateMe("y", rotSpeed * delta);
            }
            if (Settings.NegativeYKey.Value.IsPressed())
            {
                TargetInteractableComponent.RotateMe("y", -rotSpeed * delta);
            }
            if (Settings.PositiveZKey.Value.IsPressed())
            {
                TargetInteractableComponent.RotateMe("z", rotSpeed * delta);
            }
            if (Settings.NegativeZKey.Value.IsPressed())
            {
                TargetInteractableComponent.RotateMe("z", -rotSpeed * delta);
            }
        }

        public static void HandleTranslation(float speed, float delta)
        {
            if (Settings.PositiveXKey.Value.IsPressed())
            {
                TargetInteractableComponent.TranslateMe("x", speed * delta);
            }
            if (Settings.NegativeXKey.Value.IsPressed())
            {
                TargetInteractableComponent.TranslateMe("x", -(speed * delta));
            }
            if (Settings.PositiveYKey.Value.IsPressed())
            {
                TargetInteractableComponent.TranslateMe("y", speed * delta);
            }
            if (Settings.NegativeYKey.Value.IsPressed())
            {
                TargetInteractableComponent.TranslateMe("y", -(speed * delta));
            }
            if (Settings.PositiveZKey.Value.IsPressed())
            {
                TargetInteractableComponent.TranslateMe("z", speed * delta);
            }
            if (Settings.NegativeZKey.Value.IsPressed())
            {
                TargetInteractableComponent.TranslateMe("z", -(speed * delta));
            }
        }
    }
}

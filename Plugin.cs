using BepInEx;
using BepInEx.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZonePlacementTool.Helpers;
using UnityEngine;
using ZonePlacementTool.Patches;
using EFT.InputSystem;
using EFT;
using Comfort.Net;
using Comfort.Common;
using EFT.UI;
using System.Reflection;
using EFT.Interactive;

namespace ZonePlacementTool
{
    [BepInPlugin("Jehree.ZonePlacementTool", "ZonePlacementTool", "1.2.0")] 
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_NAME = "Jehree's Zone Placement Tool";
        public static string AssemblyPath { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public static MapData MapData;

        public static ManualLogSource LogSource;
        public static InteractableComponent TargetInteractableComponent;
        public static List<InteractableComponent> AllInteractableComponents = new List<InteractableComponent>();
        public static Player Player;
        public static NoteWindow NoteUIPanel;
        public static InventoryScreen InventoryUI;
        public static TasksScreen TasksUI;
        public static List<ExfiltrationPoint> ExfiltrationPointList = new List<ExfiltrationPoint>();

        public static EInputMode Mode = EInputMode.Translate;
        public enum EInputMode
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
            new ExfiltrationPointAwakePatch().Enable();
        }

        private void Update()
        {
            if (Player == null) return;
            if (TargetInteractableComponent == null) return;

            if (Settings.TranslateKey.Value.IsDown())
            {
                Mode = EInputMode.Translate;
                TargetInteractableComponent.SetColor(Color.green);
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModFunc);
            }
            if (Settings.ScaleKey.Value.IsDown())
            {
                Mode = EInputMode.Scale;
                TargetInteractableComponent.SetColor(Color.blue);
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModGear);

            }
            if (Settings.RotateKey.Value.IsDown())
            {
                Mode = EInputMode.Rotate;
                TargetInteractableComponent.SetColor(Color.red);
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModVital);
            }
            
            float delta = Time.deltaTime;
            float speed = Settings.ChangeSpeed.Value;

            switch (Mode)//
            {
                case EInputMode.Rotate:
                {
                    HandleRotation(speed, delta);
                    break;
                }
                case EInputMode.Translate:
                {
                    HandleTranslation(speed, delta);
                    break;
                }
                case EInputMode.Scale:
                {
                    HandleScaling(speed, delta);
                    break;
                }
            }
        }

        public static void SelectObject(GameObject obj, bool mute = false)
        {
            if (TargetInteractableComponent != null)
            {
                if (NameFieldEmpty() || NameTaken()) return;
            }

            TargetInteractableComponent = obj.GetComponent<InteractableComponent>();
            Mode = EInputMode.Translate;
            TargetInteractableComponent.SetColor(Color.green);
            Settings.SelectedObjectName.Value = TargetInteractableComponent.GetName();

            if (!mute)
            {
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuWeaponAssemble);
            }
        }

        public static void UnselectObject(bool mute = false)
        {
            if (TargetInteractableComponent == null) return;
            if (NameFieldEmpty() || NameTaken()) return;

            string oldName = TargetInteractableComponent.GetName();

            TargetInteractableComponent.SetName(Settings.SelectedObjectName.Value);
            TargetInteractableComponent.SetColor(Color.magenta);
            MapDataUtils.UpdateObjectData(oldName, TargetInteractableComponent);

            TargetInteractableComponent = null;
            Settings.SelectedObjectName.Value = "";

            MapData.Save();

            if (!mute)
            {
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuWeaponDisassemble);
            }
        }

        public static bool NameTaken()
        {
            if (Settings.SelectedObjectName.Value != TargetInteractableComponent.GetName() && MapDataUtils.ObjectDataExists(Settings.SelectedObjectName.Value))
            {
                ConsoleScreen.LogError($"{Plugin.MOD_NAME}: An object by the name of {Settings.SelectedObjectName.Value} already exists!");
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ErrorMessage);
                return true;
            }
            return false;
        }

        public static bool NameFieldEmpty()
        {
            if (Settings.SelectedObjectName.Value == "")
            {
                ConsoleScreen.LogError($"{MOD_NAME}: Your object has an empty name! SOMETHING WILL EXPLODE AHHH FIX IT NOW (jk just fix it and you'll be fine)");
                Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ErrorMessage);
                return true;
            }
            return false;
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

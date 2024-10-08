﻿using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ZonePlacementTool.Helpers
{
    internal class Settings
    {
        public static ConfigEntry<float> ChangeSpeed;

        public static ConfigEntry<KeyboardShortcut> PositiveXKey;
        public static ConfigEntry<KeyboardShortcut> NegativeXKey;
        public static ConfigEntry<KeyboardShortcut> PositiveYKey;
        public static ConfigEntry<KeyboardShortcut> NegativeYKey;
        public static ConfigEntry<KeyboardShortcut> PositiveZKey;
        public static ConfigEntry<KeyboardShortcut> NegativeZKey;

        public static ConfigEntry<KeyboardShortcut> TranslateKey;
        public static ConfigEntry<KeyboardShortcut> ScaleKey;
        public static ConfigEntry<KeyboardShortcut> RotateKey;
        public static ConfigEntry<bool> LockXAndZRotation;

        public static ConfigEntry<string> SelectedObjectName;

        public static ConfigEntry<string> ClosestExfilName;

        public static void Init(ConfigFile config)
        {
            SelectedObjectName = config.Bind(
                "1.0: Object Control",
                "Selected Object Name",
                ""
            );
            ChangeSpeed = config.Bind(
                "1.0: Object Control",
                "Change Speed",
                3f
            );
            config.Bind(
                "1.0: Object Control",
                "Spawn Object (name required)",
                "",
                new ConfigDescription(
                    "Creates a new zone object",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerSpawnObject }
                )
            );

            config.Bind(
                "1.1: Object Control",
                "Move Object To Player Feet",
                "",
                new ConfigDescription(
                    "Moves object to player",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerResetTranslation }
                )
            );
            config.Bind(
                "1.1: Object Control",
                "Face Player Camera Direction",
                "",
                new ConfigDescription(
                    "Creates a new zone object",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerMatchPlayerYRotation }
                )
            );
            config.Bind(
                "1.1: Object Control",
                "Unselect Object",
                "",
                new ConfigDescription(
                    "Unselects currently selected object",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerUnselectObject }
                )
            );
            config.Bind(
                "1.1: Object Control",
                "Toggle All Objects",
                "",
                new ConfigDescription(
                    "Toggles all objects on or off",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerToggleAllObjects }
                )
            );
            PositiveZKey = config.Bind(
                "2.0: Object Movement Keybinds",
                "1. Forward",
                new KeyboardShortcut(KeyCode.U)
            );
            NegativeZKey = config.Bind(
                "2.0: Object Movement Keybinds",
                "2. Backward",
                new KeyboardShortcut(KeyCode.E)
            );
            NegativeXKey = config.Bind(
                "2.0: Object Movement Keybinds",
                "3. Left",
                new KeyboardShortcut(KeyCode.N)
            );
            PositiveXKey = config.Bind(
                "2.0: Object Movement Keybinds",
                "4. Right",
                new KeyboardShortcut(KeyCode.I)
            );
            PositiveYKey = config.Bind(
                "2.0: Object Movement Keybinds",
                "5. Up",
                new KeyboardShortcut(KeyCode.Y)
            );
            NegativeYKey = config.Bind(
                "2.0: Object Movement Keybinds",
                "6. Down",
                new KeyboardShortcut(KeyCode.L)
            );

            TranslateKey = config.Bind(
                "3.0: Mode Keybinds",
                "Translate",
                new KeyboardShortcut(KeyCode.J)
            );
            ScaleKey = config.Bind(
                "3.0: Mode Keybinds",
                "Scale",
                new KeyboardShortcut(KeyCode.H)
            );
            RotateKey = config.Bind(
                "3.0: Mode Keybinds",
                "Rotate",
                new KeyboardShortcut(KeyCode.Comma)
            );
            LockXAndZRotation = config.Bind(
                "3.0: Mode Keybinds",
                "Lock X And Z Rotation Axes",
                true
            );
            
            ClosestExfilName = config.Bind(
                "1.2: Utilities",
                "1. Closest Exfil To Player",
                ""
            );
            config.Bind(
                "1.2: Utilities",
                "2. Update Closest Exfil Name",
                "",
                new ConfigDescription(
                    "Puts the exfil name currently closest to player in the Closest Exfil To Player field",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerUpdateClosestExfilName }
                )
            );
            config.Bind(
                "1.2: Utilities",
                "3. Toggle All Exfil Zones",
                "",
                new ConfigDescription(
                    "Toggles all exfil zones on and off so you can move around in them without extracting",
                    null,
                    new ConfigurationManagerAttributes { CustomDrawer = DrawerToggleExfils }
                )
            );
        }

        private static void DrawerSpawnObject(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;
            InitializeButton(InteractableComponent.Spawn, "Spawn Object");
        }
        private static void DrawerMatchPlayerYRotation(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;
            if (Plugin.TargetInteractableComponent == null) return;
            InitializeButton(Plugin.TargetInteractableComponent.MatchPlayerYRotation, "Object Faces Camera Direction");
        }
        private static void DrawerResetTranslation(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;
            if (Plugin.TargetInteractableComponent == null) return;
            InitializeButton(Plugin.TargetInteractableComponent.ResetTranslation, "Move Object To Player Feet");
        }
        private static void DrawerUnselectObject(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;
            if (Plugin.TargetInteractableComponent == null) return;
            InitializeButton(() => { Plugin.UnselectObject(); }, "Unselect Object");
        }
        private static void DrawerUpdateClosestExfilName(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;
            InitializeButton(() => { 
                string name = Utils.GetClosestExfilName(Plugin.ExfiltrationPointList);
                ClosestExfilName.Value = name;
            }, "Update");
        }
        private static void DrawerToggleAllObjects(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;

            InitializeButton(() => { 
                foreach (var x in Plugin.AllInteractableComponents)
                {
                    if (x.gameObject.activeSelf)
                    {
                        x.gameObject.SetActive(false);
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuWeaponDisassemble);
                    }
                    else
                    {
                        x.gameObject.SetActive(true);
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuWeaponAssemble);
                    }
                }
            }, "Toggle Objects");
        }
        private static void DrawerToggleExfils(ConfigEntryBase entry)
        {
            if (Plugin.Player == null) return;

            InitializeButton(() => {
                foreach (var x in Singleton<GameWorld>.Instance.ExfiltrationController.ExfiltrationPoints)
                {
                    if (x.gameObject.activeSelf)
                    {
                        x.gameObject.SetActive(false);
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuWeaponDisassemble);
                    }
                    else
                    {
                        x.gameObject.SetActive(true);
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuWeaponAssemble);
                    }
                }
            }, "Toggle Exfils");
        }
        private static void InitializeButton(Action callable, string buttonName)
        {
            if (GUILayout.Button(buttonName, GUILayout.ExpandWidth(true)))
            {
                callable();
            }
        }
        public static void OnGameStarted()
        {
            SelectedObjectName.Value = "";
        }
    }
}

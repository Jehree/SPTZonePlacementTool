using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GClass1750;

namespace ObjectPlacementTool.Helpers
{
    internal class Settings
    {
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<float> ChangeSpeed;

        public static ConfigEntry<KeyboardShortcut> SpawnObjectKey;
        public static ConfigEntry<KeyboardShortcut> ResetKey;
        public static ConfigEntry<KeyboardShortcut> PositiveXKey;
        public static ConfigEntry<KeyboardShortcut> NegativeXKey;
        public static ConfigEntry<KeyboardShortcut> PositiveYKey;
        public static ConfigEntry<KeyboardShortcut> NegativeYKey;
        public static ConfigEntry<KeyboardShortcut> PositiveZKey;
        public static ConfigEntry<KeyboardShortcut> NegativeZKey;

        public static ConfigEntry<KeyboardShortcut> TranslateKey;
        public static ConfigEntry<KeyboardShortcut> ScaleKey;
        public static ConfigEntry<KeyboardShortcut> RotateKey;

        public static void Init(ConfigFile config)
        {
            ModEnabled = config.Bind(
                "1: Mod",
                "Mod Enabled",
                true
            );
            ChangeSpeed = config.Bind(
                "1: Mod",
                "Change Speed",
                1f
            );

            SpawnObjectKey = config.Bind(
                "4: Controlling le object",
                "Spawn Object",
                new KeyboardShortcut(KeyCode.Keypad3)
            );
            ResetKey = config.Bind(
                "4: Controlling le object",
                "Reset Selected Object Edit Mode (position(green), rotation(red), or scale(blue))",
                new KeyboardShortcut(KeyCode.Keypad0)
            );
            PositiveXKey = config.Bind(
                "2: Object Movement",
                "Positive X",
                new KeyboardShortcut(KeyCode.I)
            );
            NegativeXKey = config.Bind(
                "2: Object Movement",
                "Negative X",
                new KeyboardShortcut(KeyCode.N)
            );
            PositiveYKey = config.Bind(
                "2: Object Movement",
                "Positive Y",
                new KeyboardShortcut(KeyCode.Y)
            );
            NegativeYKey = config.Bind(
                "2: Object Movement",
                "Negative Y",
                new KeyboardShortcut(KeyCode.L)
            );
            PositiveZKey = config.Bind(
                "2: Object Movement",
                "Positive Z",
                new KeyboardShortcut(KeyCode.U)
            );
            NegativeZKey = config.Bind(
                "2: Object Movement",
                "Negative Z",
                new KeyboardShortcut(KeyCode.E)
            );

            TranslateKey = config.Bind(
                "3: Modifiers",
                "Translate",
                new KeyboardShortcut(KeyCode.J)
            );
            ScaleKey = config.Bind(
                "3: Modifiers",
                "Scale",
                new KeyboardShortcut(KeyCode.H)
            );
            RotateKey = config.Bind(
                "3: Modifiers",
                "Rotate",
                new KeyboardShortcut(KeyCode.Comma)
            );
        }


    }
}

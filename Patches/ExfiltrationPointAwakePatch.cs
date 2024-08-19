using Comfort.Common;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZonePlacementTool;

namespace ZonePlacementTool.Patches
{
    internal class ExfiltrationPointAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ExfiltrationPoint).GetMethod(nameof(ExfiltrationPoint.Awake));
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref ExfiltrationPoint __instance)
        {
            Plugin.ExfiltrationPointList.Add(__instance);
            return true;
        }
    }
}

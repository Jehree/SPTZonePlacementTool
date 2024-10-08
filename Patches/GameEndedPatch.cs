﻿using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlacementTool.Patches
{
    internal class GameEndedPatch : ModulePatch
    {
        private static Type _targetClassType;

        protected override MethodBase GetTargetMethod()
        {
            _targetClassType = PatchConstants.EftTypes.Single(targetClass =>
                !targetClass.IsInterface &&
                !targetClass.IsNested &&
                targetClass.GetMethods().Any(method => method.Name == "OfflineRaidEnded") &&
                targetClass.GetMethods().Any(method => method.Name == "ReceiveInsurancePrices")
            );

            return AccessTools.Method(_targetClassType.GetTypeInfo(), "OfflineRaidEnded");
        }

        [PatchPostfix]
        public static void Postfix()
        {
            Plugin.MapData.Save();
            Plugin.MapData = null;
            Plugin.TargetInteractableComponent = null;
            Plugin.Player = null;
            Plugin.AllInteractableComponents.Clear();
;           Plugin.ExfiltrationPointList.Clear();
        }
    }
}

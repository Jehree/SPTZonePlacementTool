using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using ObjectPlacementTool.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EFT.UI;
using UnityEngine.SceneManagement;

namespace ObjectPlacementTool.Patches
{
    internal class GameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        public static void PatchPostfix()
        {
            if (!Settings.ModEnabled.Value) return;

            Plugin.Player = Singleton<GameWorld>.Instance.MainPlayer;
        }


    }
}

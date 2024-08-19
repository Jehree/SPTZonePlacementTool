using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using ZonePlacementTool.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EFT.UI;
using UnityEngine.SceneManagement;
using System.IO;

namespace ZonePlacementTool.Patches
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
            Plugin.Player = Singleton<GameWorld>.Instance.MainPlayer;
            Settings.OnGameStarted();

            string locId = Singleton<GameWorld>.Instance.LocationId;
            string locationJsonPath = MapData.GetPathByMapID(locId);

            if (File.Exists(locationJsonPath))
            {
                string json = File.ReadAllText(locationJsonPath);
                Plugin.MapData = MapData.GetDataFromJson(json);

                foreach (ObjectData obj in Plugin.MapData.Objects)
                {
                    InteractableComponent.ForceSpawn(obj);
                }
            }
            else
            {
                Plugin.MapData = new MapData(locId);
            }
        }
    }
}

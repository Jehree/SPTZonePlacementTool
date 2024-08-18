using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPlacementTool.Patches
{
    internal class NoteWindowAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(NoteWindow).GetMethod(nameof(NoteWindow.Awake));
        }

        [PatchPostfix]
        public static void PostfixPatch(ref NoteWindow __instance)
        {
            Plugin.NoteUIPanel = __instance;
        }
    }

    internal class InventoryScreenAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(InventoryScreen).GetMethod(nameof(InventoryScreen.Awake));
        }

        [PatchPostfix]
        public static void PostfixPatch(ref InventoryScreen __instance)
        {
            Plugin.InventoryUI = __instance;
        }
    }

    internal class TasksScreenAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(TasksScreen).GetMethod(nameof(TasksScreen.Awake));
        }

        [PatchPostfix]
        public static void PostfixPatch(ref TasksScreen __instance)
        {
            Plugin.TasksUI = __instance;
        }
    }
}

using EFT;
using HarmonyLib;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPlacementTool.Patches
{
    internal class GetAvailableActionsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.FirstMethod(typeof(GetActionsClass), x => x.Name == nameof(GetActionsClass.GetAvailableActions) && x.GetParameters()[0].Name == "owner");
        }

        [PatchPrefix]
        public static bool PatchPrefix(GamePlayerOwner owner, [CanBeNull] GInterface102 interactive, ref ActionsReturnClass __result)
        {
            if (!(interactive is InteractableComponent)) return true;
            var customInteractable = interactive as InteractableComponent;

            __result = new ActionsReturnClass()
            {
                Actions = customInteractable.Actions
            };
            return false;
        }
    }
}

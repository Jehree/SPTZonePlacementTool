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
using static RootMotion.FinalIK.InteractionTrigger.Range;

namespace ObjectPlacementTool.Patches
{
    internal class GetAvailableActionsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.FirstMethod(typeof(GetActionsClass), x => x.Name == nameof(GetActionsClass.GetAvailableActions) && x.GetParameters()[0].Name == "owner");
        }

        [PatchPrefix]
        public static bool PatchPrefix(object[] __args, ref ActionsReturnClass __result)
        {
            // __args[1] is a GInterface called "interactive", it represents the component that enables interaction
            if (!(__args[1] is InteractableComponent)) return true;
            var customInteractable = __args[1] as InteractableComponent;

            __result = new ActionsReturnClass()
            {
                Actions = customInteractable.Actions
            };
            return false;
        }
    }
}

using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRUI;

namespace PSVRUI.HarmonyPatches
{
    [HarmonyPatch(typeof(SettingsFlowCoordinator))]
    [HarmonyPatch("DidActivate", MethodType.Normal)]
    class SettingsFlowCoordinator_DidActivate
    {
        public static bool Prefix(bool firstActivation, FlowCoordinator.ActivationType activationType, SettingsFlowCoordinator __instance)
        {
            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                __instance.InvokePrivateMethod("ProvideInitialViewControllers", new object[] { Resources.FindObjectsOfTypeAll<FloorAdjustViewController>().FirstOrDefault(), null, null, null });
            }
            return false;
        }
    }
}

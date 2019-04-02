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
    [HarmonyPatch(typeof(MainFlowCoordinator))]
    [HarmonyPatch("DidActivate", MethodType.Normal)]
    class MainFlowCoordinator_DidActivate
    {
        public static void Prefix(bool firstActivation, FlowCoordinator.ActivationType activationType, ref ReleaseInfoViewController ____releaseInfoViewController)
        {
            ____releaseInfoViewController = null;
        }
    }

    [HarmonyPatch(typeof(MainFlowCoordinator))]
    [HarmonyPatch("HandleFloorAdjustViewControllerDidFinishEvent", MethodType.Normal)]
    class MainFlowCoordinator_HandleFloorAdjustViewControllerDidFinishEvent
    {
        public static bool Prefix(FloorAdjustViewController viewController, MainFlowCoordinator __instance)
        {
            __instance.InvokePrivateMethod("DismissFlowCoordinator", new object[] { Resources.FindObjectsOfTypeAll<SettingsFlowCoordinator>().FirstOrDefault(), null, false });
            __instance.GetPrivateField<MainSettingsModel>("_mainSettingsModel").Save();
            return false;
        }
    }
}

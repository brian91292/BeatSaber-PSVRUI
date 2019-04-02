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
    [HarmonyPatch(typeof(PlatformLeaderboardViewController))]
    [HarmonyPatch("DidActivate", MethodType.Normal)]
    class PlatformLeaderboardViewController_DidActivate
    {
        public static void Postfix(bool firstActivation, VRUIViewController.ActivationType activationType, PlatformLeaderboardViewController __instance)
        {
            // Disable the "around you" scoreboard filter
            var scopeSelection = __instance.transform.Find("ScopeSelection");
            scopeSelection.Find("VMiddleIconSegmentedControlCell(Clone)").gameObject.SetActive(false);
            scopeSelection.Find("DarkHorizontalSeparator(Clone)").gameObject.SetActive(false);
        }
    }
}

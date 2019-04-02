using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PSVRUI.HarmonyPatches
{
    [HarmonyPatch(typeof(MainMenuViewController))]
    [HarmonyPatch("HandleMenuButton", MethodType.Normal)]
    class MainMenuViewController_HandleMenuButton
    {
        public static bool Prefix(MainMenuViewController.MenuButton menuButton)
        {
            if(menuButton == MainMenuViewController.MenuButton.SoloFreePlay)
            {
                CustomMenuFlow.PresentPackMenu("Solo", Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().FirstOrDefault().GetPrivateField<BeatmapLevelPackCollectionSO>("_levelPackCollection"));
                return false;
            }
            else if (menuButton == MainMenuViewController.MenuButton.Party)
            {
                CustomMenuFlow.PresentPackMenu("Party", Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().FirstOrDefault().GetPrivateField<BeatmapLevelPackCollectionSO>("_levelPackCollection"));
                return false;
            }
            return true;
        }
    }
}

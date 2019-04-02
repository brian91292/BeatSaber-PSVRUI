using CustomUI.BeatSaber;
using PSVRUI.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace PSVRUI
{
    class CustomMenuFlow : MonoBehaviour
    {
        public static CustomViewController _psvrPackBrowserClone;
        private static BeatmapLevelPackCollectionSO _levels;
        private static MainFlowCoordinator mainFlowCoordinator;
        private static string _title = "";
        public static bool isActive = false;

        public static void OnLoad()
        {
            mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().FirstOrDefault();
            if (_psvrPackBrowserClone == null)
            {
                _psvrPackBrowserClone = BeatSaberUI.CreateViewController<CustomViewController>();
                _psvrPackBrowserClone.includeBackButton = true;
                _psvrPackBrowserClone.didActivateEvent += (firstActivation, type) =>
                {
                    if (firstActivation)
                    {
                        var levelPacksTableView = Resources.FindObjectsOfTypeAll<LevelPacksTableView>().FirstOrDefault();
                        levelPacksTableView.transform.localScale *= 1.25f;
                        levelPacksTableView.SetPrivateField("_cellWidth", 28f);
                        levelPacksTableView.GetComponentsInChildren<Button>().ToList().ForEach(b => b.gameObject.SetActive(false));
                        levelPacksTableView.transform.SetParent(_psvrPackBrowserClone.transform, false);
                        levelPacksTableView.transform.localPosition += new Vector3(6, 0);
                        levelPacksTableView.SetData(_levels);
                    }
                    isActive = true;
                };
                _psvrPackBrowserClone.didDeactivateEvent += (type) =>
                {
                    isActive = false;
                };
                _psvrPackBrowserClone.backButtonPressed += () =>
                {
                    DismissPackMenu();
                };
            }
        }

        public static void PresentPackSongList(IBeatmapLevelPack levelPack)
        {
            mainFlowCoordinator.InvokePrivateMethod("PresentFlowCoordinator", new object[] { (_title == "Solo" ? (FlowCoordinator)mainFlowCoordinator.GetPrivateField<SoloFreePlayFlowCoordinator>("_soloFreePlayFlowCoordinator") : (FlowCoordinator)mainFlowCoordinator.GetPrivateField<PartyFreePlayFlowCoordinator>("_partyFreePlayFlowCoordinator")), null, false, false });
            LevelPackTableCell_HighlightDidChange.playButtonImage.enabled = false;
            Resources.FindObjectsOfTypeAll<LevelPackLevelsViewController>().FirstOrDefault().SetData(levelPack);
            Resources.FindObjectsOfTypeAll<LevelPackDetailViewController>().FirstOrDefault().SetData(levelPack);
        }
        
        public static void PresentPackMenu(string title, BeatmapLevelPackCollectionSO levels)
        {
            _title = title;
            _levels = levels;
            (mainFlowCoordinator as FlowCoordinator).SetPrivateProperty("title", title);
            mainFlowCoordinator.InvokePrivateMethod("PresentViewController", new object[] { _psvrPackBrowserClone, null, false });
            var gameplaySetupViewController = Resources.FindObjectsOfTypeAll<GameplaySetupViewController>().FirstOrDefault();
            var currentLocalPlayer = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().FirstOrDefault().currentLocalPlayer;
            gameplaySetupViewController.Init(currentLocalPlayer.playerSpecificSettings, currentLocalPlayer.gameplayModifiers);
            mainFlowCoordinator.InvokePrivateMethod("SetLeftScreenViewController", new object[] { gameplaySetupViewController, false });
        }

        public static void DismissPackMenu()
        {
            (mainFlowCoordinator as FlowCoordinator).SetPrivateProperty("title", "");
            mainFlowCoordinator.InvokePrivateMethod("DismissViewController", new object[] { _psvrPackBrowserClone, null, false });
        }
    }
}

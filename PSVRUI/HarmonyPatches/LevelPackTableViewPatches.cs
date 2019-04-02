using Harmony;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSVRUI.HarmonyPatches
{

    [HarmonyPatch(typeof(LevelPacksTableView))]
    [HarmonyPatch("HandleDidSelectColumnEvent", MethodType.Normal)]
    class LevelPacksTableView_SelectionDidChange
    {
        public static void Postfix(TableView tableView, int column, LevelPacksTableView __instance, IBeatmapLevelPackCollection ____levelPackCollection)
        {
            Console.WriteLine($"SelectionDidChange! Selected pack {____levelPackCollection.beatmapLevelPacks[column].packName}");
            CustomMenuFlow.PresentPackSongList(____levelPackCollection.beatmapLevelPacks[column]);
        }
    }
}

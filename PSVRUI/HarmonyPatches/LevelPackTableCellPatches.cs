using CustomUI.Utilities;
using Harmony;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Image = UnityEngine.UI.Image;

namespace PSVRUI.HarmonyPatches
{
    [HarmonyPatch(typeof(LevelPackTableCell))]
    [HarmonyPatch("HighlightDidChange", MethodType.Normal)]
    class LevelPackTableCell_HighlightDidChange
    {
        public static Image playButtonImage;
        public static Sprite playButtonSprite;
        public static void Prefix(TableCell.TransitionType transitionType, LevelPackTableCell __instance, Image ____coverImage, Image ____selectionImage, TextMeshProUGUI ____infoText, TextMeshProUGUI ____packNameText)
        {
            if (playButtonSprite == null)
                playButtonSprite = UIUtilities.LoadSpriteFromResources("PSVRUI.Resources.PlayButton.png");

            if (CustomMenuFlow.isActive)
            {
                if (playButtonImage == null)
                {
                    var newGameObj = new GameObject();
                    newGameObj.transform.SetParent(____coverImage.transform, false);
                    ReflectionUtil.CopyComponent(Image.Instantiate(____coverImage), typeof(Image), typeof(Image), newGameObj);
                    playButtonImage = newGameObj.GetComponent<Image>();
                    playButtonImage.sprite = playButtonSprite;
                    playButtonImage.color = Color.white.ColorWithAlpha(1f);
                }

                playButtonImage.transform.SetParent(____coverImage.transform, false);
                playButtonImage.enabled = (__instance as TableCell).highlighted;
            }
        }
    }


    [HarmonyPatch(typeof(LevelPackTableCell))]
    [HarmonyPatch("RefreshVisuals", MethodType.Normal)]
    class LevelPackTableCell_RefreshVisuals
    {
        public static void Postfix(LevelPackTableCell __instance, Image ____coverImage, Image ____selectionImage, TextMeshProUGUI ____infoText, TextMeshProUGUI ____packNameText)
        {
            ____coverImage.color = ((!(__instance as TableCell).highlighted) ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f));
            ____selectionImage.color = new Color(0, 0, 0, 0);

            __instance.showNewRibbon = false;

            float yPos = -15;
            float fontSize = 2.35f;

            Vector3[] outArray = new Vector3[4];
            ____coverImage.rectTransform.GetLocalCorners(outArray);

            ____packNameText.transform.position = ____coverImage.transform.TransformPoint(outArray[1]  + new Vector3(11.4f,0));
            ____infoText.transform.position = ____packNameText.transform.position;

            Vector3 packTextPos = ____packNameText.rectTransform.localPosition;
            ____packNameText.rectTransform.localPosition = new Vector3(packTextPos.x, yPos, packTextPos.z);
            ____packNameText.alignment = TextAlignmentOptions.Left;
            ____packNameText.fontSize = fontSize;
            ____packNameText.enableWordWrapping = false;
            ____packNameText.enabled = true;

            Vector3 infoTextPos = ____infoText.rectTransform.localPosition;
            ____infoText.rectTransform.localPosition = new Vector3(packTextPos.x + 3.5f, yPos - 5.3f, infoTextPos.z);
            ____infoText.alignment = TextAlignmentOptions.Left;
            ____infoText.fontSize = fontSize;
            ____infoText.enableWordWrapping = false;
            ____infoText.enabled = true;
        }
    }
}

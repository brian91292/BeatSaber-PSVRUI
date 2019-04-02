using CustomUI.Utilities;
using Harmony;
using HMUI;
using IllusionPlugin;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using Image = UnityEngine.UI.Image;

namespace PSVRUI
{
    public class Plugin : IPlugin
    {
        public string Name => "PSVRUI";
        public string Version => "0.0.1";
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            HarmonyInstance hi = HarmonyInstance.Create("com.brian91292.beatsaber.psvrui");
            hi.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(arg0.name == "MenuCore")
            {
                SharedCoroutineStarter.instance.StartCoroutine(DelayedStartup());
            }
        }

        private IEnumerator DelayedStartup()
        {
            yield return new WaitForSeconds(0.1f);

            // Enable monstercat promo button
            var monstercatButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "MonstercatPromoButton");
            monstercatButton.gameObject.SetActive(true);
            
            // Change settings button text
            var settingsButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "SettingsButton");
            settingsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Floor Adjust";

            // PlayerSettingsButton -> HowToPlayButton
            var playerSettingsButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "PlayerSettingsButton");
            var howToPlayButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "HowToPlayButton");
            var practiceButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "PracticeButton");
            playerSettingsButton.GetComponentInChildren<TextMeshProUGUI>().text = howToPlayButton.GetComponentInChildren<TextMeshProUGUI>().text;
            playerSettingsButton.onClick = howToPlayButton.onClick;
            playerSettingsButton.GetComponentsInChildren<Image>().First(x => x.name == "Icon").sprite = practiceButton.GetComponentsInChildren<Image>().First(x => x.name == "Icon").sprite;
            howToPlayButton.gameObject.SetActive(false);

            // QuitButton -> CreditsButton
            var quitButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "QuitButton");
            var creditsButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "CreditsButton");
            quitButton.GetComponentInChildren<TextMeshProUGUI>().text = creditsButton.GetComponentInChildren<TextMeshProUGUI>().text;
            quitButton.onClick = creditsButton.onClick;
            creditsButton.gameObject.SetActive(false);

            // Hide the bottom panel and move the title bar to its position
            var screenSystem = Resources.FindObjectsOfTypeAll<VRUIScreenSystem>().FirstOrDefault();
            screenSystem.bottomScreen.gameObject.SetActive(false);
            var titleBar = screenSystem.GetPrivateField<VRUITitleBar>("_titleBar");//
            titleBar.transform.position = screenSystem.bottomScreen.transform.position + new Vector3(0, 0.21f, 0.15f);
            titleBar.transform.rotation = screenSystem.bottomScreen.transform.rotation;

            // Disable background art for solo/campaign/party buttons
            var campaignButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "CampaignButton");
            var soloFreePlayButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "SoloFreePlayButton");
            var partyFreePlayButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == "PartyFreePlayButton");
            campaignButton.transform.Find("Wrapper").Find("BGArtwork").gameObject.SetActive(false);
            soloFreePlayButton.transform.Find("Wrapper").Find("BGArtwork").gameObject.SetActive(false);
            partyFreePlayButton.transform.Find("Wrapper").Find("BGArtwork").gameObject.SetActive(false);

            // Setup our custom level pack menu
            CustomMenuFlow.OnLoad();

            Transform gameplayModifiersPanel = null;
            // Disable the ghost notes/no arrows gameplay toggles, and the separators near them so it doesnt look dumb
            Resources.FindObjectsOfTypeAll<GameplayModifierToggle>().ToList().ForEach(g =>
            {
                if (g.name == "GhostNotes" || g.name == "NoArrows")
                {
                    int count = 0;
                    gameplayModifiersPanel = g.transform.parent.parent;
                    foreach (Transform t in g.transform.parent)
                    {
                        if (t.name.StartsWith("Separator"))
                        {
                            if (count == 2 && g.name == "GhostNotes")
                                t.gameObject.SetActive(false);
                            else if (count == 3 && g.name == "NoArrows")
                                t.gameObject.SetActive(false);
                            count++;
                        }
                    }
                    g.gameObject.SetActive(false);
                }
            });

            // Readjust the size of the info panel
            var infoPanel = (gameplayModifiersPanel.Find("Info") as RectTransform);
            GameObject.DestroyImmediate(infoPanel.GetComponent<HorizontalLayoutGroup>());
            var verticalLayoutGroup = infoPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = -3.5f;
            infoPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            infoPanel.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            infoPanel.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(65, 16f);
            
            // Make the player settings panel look legit
            ModPlayerSettingsPanel(gameplayModifiersPanel.parent.Find("PlayerSettings"));
            ModPlayerSettingsPanel(Resources.FindObjectsOfTypeAll<LocalPlayerSettingsViewController>().FirstOrDefault().transform.Find("PlayerSettings"), true);
            
            Resources.FindObjectsOfTypeAll<Material>().ToList().ForEach(m => Console.WriteLine($"Material: {m.name}"));
        }

        private void ModPlayerSettingsPanel(Transform playerSettingsPanel, bool minimal = false)
        {
            var rightPanel = playerSettingsPanel.Find("RightPanel");
            var leftPanel = playerSettingsPanel.Find("LeftPanel");
            leftPanel.transform.localPosition = new Vector3(0, !minimal ? playerSettingsPanel.Find("LeftPanel").transform.localPosition.y : 6);
            rightPanel.Find("SoundFX").SetParent(leftPanel, false);
            rightPanel.Find("Separator (3)").SetParent(leftPanel, false);
            if (!minimal)
            {
                rightPanel.Find("ReduceDebris").SetParent(leftPanel, false);
                rightPanel.Find("Separator (4)").SetParent(leftPanel, false);
            }
            leftPanel.Find("PlayerHeight").SetAsLastSibling();
            leftPanel.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(78, !minimal ? 53.5f : 45);
            GameObject.DestroyImmediate(playerSettingsPanel.GetComponent<HorizontalLayoutGroup>());
            rightPanel.gameObject.SetActive(false);
        }
 
        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }
       
        public void OnFixedUpdate()
        {
        }
    }
}

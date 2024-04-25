using MelonLoader;
using RUMBLE.Managers;
using RumbleModdingAPI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExtraBBRanks
{
    public class Class1 : MelonMod
    {
        private bool loaded = false;
        private bool sceneChanged = false;
        private string currentScene = "";
        private GameObject rankGameObject;
        private GameObject levelGameObject;
        private GameObject levelTextStatic;
        private GameObject rankTextStatic;
        private static TextMeshPro rankTextComponent;
        private static TextMeshPro levelTextComponent;
        private PlayerManager playerManager;
        private GameObject rankMod;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            sceneChanged = true;
            if (loaded && rankGameObject != null && levelGameObject != null)
            {
                rankGameObject.SetActive(currentScene == "Gym");
                levelGameObject.SetActive(currentScene == "Gym");

                // Update progress bar if scene is Gym
                if (currentScene == "Gym")
                {
                    UpdateLevelProgressBar();
                    UpdateRankText();
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (currentScene != "Gym")
            {
                return;
            }
            if (sceneChanged)
            {
                try
                {
                    if (!loaded)
                    {
                        playerManager = Calls.Managers.GetPlayerManager();
                        // Create rankGameObject
                        rankMod = new GameObject("RankMod");
                        rankGameObject = new GameObject("RankInfo");
                        levelGameObject = new GameObject("LevelBar");

                        // Instantiate rankTextStatic
                        rankTextStatic = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.PlayerTags.HighscoreTag0.Nr.GetGameObject());
                        if (rankTextStatic == null)
                        {
                            MelonLogger.Error("Failed to instantiate rankTextStatic.");
                            return;
                        }

                        levelTextStatic = GameObject.Instantiate(rankTextStatic);
                        if (levelTextStatic == null)
                        {
                            MelonLogger.Error("Failed to instantiate levelTextStatic.");
                        }

                        // Set rankTextStatic as a child of rankGameObject
                        rankTextStatic.transform.parent = rankGameObject.transform;
                        rankTextStatic.name = "RankText";

                        levelTextStatic.transform.parent = levelGameObject.transform;
                        levelTextStatic.name = "LevelBarText";

                        // Get TextMeshPro component
                        rankTextComponent = rankTextStatic.GetComponent<TextMeshPro>();
                        if (rankTextComponent == null)
                        {
                            MelonLogger.Error("TextMeshPro component not found on rankTextStatic.");
                            return;
                        }

                        levelTextComponent = levelTextStatic.GetComponent<TextMeshPro>();
                        if (rankTextComponent == null)
                        {
                            MelonLogger.Error("TextMeshPro component not found on levelTextStatic.");
                            return;
                        }

                        // Set properties of rankTextComponent
                        rankTextComponent.text = "Rank: " + GetPlayerRank(GetPlayerBP());
                        rankTextComponent.fontSize = 3f;
                        rankTextComponent.color = new Color(255f, 255f, 255f, 255f);
                        rankTextComponent.outlineColor = new Color32(255, 255, 255, 255);
                        rankTextComponent.alignment = TextAlignmentOptions.Center;
                        rankTextComponent.enableWordWrapping = false;
                        rankTextComponent.outlineWidth = 0.1f;

                        // Set properties of levelTextComponent
                        levelTextComponent.text = GetLevelProgressBar(GetPlayerLevel());
                        levelTextComponent.fontSize = 3f;
                        levelTextComponent.color = new Color(255f, 255f, 255f, 255f);
                        levelTextComponent.outlineColor = new Color32(255, 255, 255, 255);
                        levelTextComponent.alignment = TextAlignmentOptions.Center;
                        levelTextComponent.enableWordWrapping = false;
                        levelTextComponent.outlineWidth = 0.1f;

                        // Set positions and rotations/Scale
                        rankTextStatic.transform.localPosition = new Vector3(0f, 0.9f, 0f);
                        rankTextStatic.transform.localRotation = Quaternion.Euler(0f, 314.4196f, 0f);
                        rankGameObject.transform.position = new Vector3(1.6177f, 2.2473f, 1.7972f);
                        rankGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        levelTextStatic.transform.localPosition = new Vector3(0f, 0.7f, 0f);
                        levelTextStatic.transform.localRotation = Quaternion.Euler(0f, 314.4196f, 0f);
                        levelTextStatic.transform.localScale = new Vector3(0.8f, 0.9f, 0.1f);
                        levelGameObject.transform.position = new Vector3(1.6177f, 2.2473f, 1.7972f);
                        levelGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        loaded = true;
                        GameObject.DontDestroyOnLoad(rankGameObject);
                        GameObject.DontDestroyOnLoad(levelGameObject);
                        MelonLogger.Msg("Extra Black Belt Ranks Mod Loaded.");
                    }
                    rankGameObject.SetActive(true);
                    levelGameObject.SetActive(true);
                    sceneChanged = false;
                }
                catch
                {
                 
                }
            }
        }

        private int GetPlayerBP()
        {
            if (playerManager.localPlayer != null)
            {
                return playerManager.localPlayer.Data.GeneralData.BattlePoints;
            }
            else
            {
                MelonLogger.Msg("localPlayer is null. Player data might not be initialized yet.");
                return 0;
            }
        }

        private string GetPlayerLevel()
        {
            if (playerManager.localPlayer != null)
            {
                int battlePoints = playerManager.localPlayer.Data.GeneralData.BattlePoints;
                return battlePoints.ToString();
            }
            else
            {
                MelonLogger.Msg("localPlayer is null. Player data might not be initialized yet.");
                return "0";
            }
        }

        private string GetPlayerRank(int playerBP)
        {
            int count = 0;
            while (playerBP >= 156)
            {
                playerBP -= 156;
                count++;
            }
            return $"Black Belt x{count}";
        }

        private string GetLevelProgressBar(string PlayerLevel)
        {
            // Assuming playerBP is a string representing the player's battle points
            int bp = int.Parse(PlayerLevel);
            int level = (bp / 156) + 1; // Assuming each rank level requires 156 battle points
            int progress = (bp % 156) / 12; // Calculating progress within the current level

            // Assuming the level progress is represented by a progress bar
            string progressBar = "[";

            // Filling the progress bar with '=' for completed progress
            for (int i = 0; i < progress; i++)
            {
                progressBar += "=";
            }

            // Filling the remaining part of the progress bar with '-' for remaining progress
            for (int i = progress; i < (156) / 13; i++)
            {
                progressBar += "-";
            }

            // Closing the progress bar with ']'
            progressBar += "]";

            // Combining level information and progress bar
            string levelBar = $"Progress Until Level: {level}" +
            $"{progressBar}";

            return levelBar;
        }

        private void UpdateLevelProgressBar()
        {
            if (levelTextComponent != null)
            {
                // Update progress bar text
                levelTextComponent.text = GetLevelProgressBar(GetPlayerLevel());
            }
        }

        private void UpdateRankText()
        {
            if (rankTextComponent != null)
            {
                rankTextComponent.text = "Rank: " + GetPlayerRank(GetPlayerBP());
            }
        }
    }
}

﻿using CustomCampaigns.Campaign.Missions;
using CustomCampaigns.Managers;
using CustomCampaigns.Utils;
using HarmonyLib;
using SongCore;
using System;

namespace CustomCampaigns.HarmonyPatches
{
    [HarmonyPatch(typeof(CampaignFlowCoordinator), "StartLevel")]
    class CampaignFlowCoordinatorStartLevelPatch
    {
        static bool Prefix(Action beforeSceneSwitchCallback, CampaignFlowCoordinator __instance, MissionSelectionNavigationController ____missionSelectionNavigationController, MenuTransitionsHelper ____menuTransitionsHelper, PlayerDataModel ____playerDataModel,
                           EnvironmentsListModel ____environmentsListModel)
        {
            if (CustomCampaignManager.isCampaignLevel)
            {
                return false;
            }
            CustomMissionDataSO missionData = ____missionSelectionNavigationController.selectedMissionNode.missionData as CustomMissionDataSO;
            if (missionData != null)
            {
                var level = missionData.beatmapLevel.levelID;
                var beatmapLevel = Loader.BeatmapLevelsModelSO.GetBeatmapLevel(level);
                BeatmapKey beatmapKey = BeatmapUtils.GetMatchingBeatmapKey(level, missionData.beatmapCharacteristic, missionData.beatmapDifficulty);
                GameplayModifiers gameplayModifiers = missionData.gameplayModifiers;
                MissionObjective[] missionObjectives = missionData.missionObjectives;
                PlayerSpecificSettings playerSpecificSettings = ____playerDataModel.playerData.playerSpecificSettings;
                ColorSchemesSettings colorSchemesSettings = ____playerDataModel.playerData.colorSchemesSettings;
                ColorScheme overrideColorScheme = colorSchemesSettings.overrideDefaultColors ? colorSchemesSettings.GetSelectedColorScheme() : null;

                ____menuTransitionsHelper.StartMissionLevel("", beatmapKey, beatmapLevel, overrideColorScheme, gameplayModifiers, missionObjectives, playerSpecificSettings, ____environmentsListModel, beforeSceneSwitchCallback,
                    levelFinishedCallback: (Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults> ) __instance.GetType().GetMethod("HandleMissionLevelSceneDidFinish", AccessTools.all)?.CreateDelegate(typeof(Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults>), __instance),
                    levelRestartedCallback: (Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults>) __instance.GetType().GetMethod("HandleMissionLevelSceneRestarted", AccessTools.all)?.CreateDelegate(typeof(Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults>), __instance));

                return false;
            }
            return true;
        }
    }
}

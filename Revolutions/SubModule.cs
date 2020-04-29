﻿using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using ModLibrary.Files;
using Revolutions.CampaignBehaviors;
using Revolutions.Models;
using ModLibrary.Settlements;
using Revolutions.Settlements;
using ModLibrary.Factions;
using Revolutions.Factions;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        public static Settings Configuration;
        public static string ModuleDataPath = Path.Combine(BasePath.Name, "Modules", "Revolutions", "ModuleData");

        public static FactionManager<FactionInfoRevolutions> FactionManager { get; } = SubModule.FactionManager;

        public static SettlementManager<SettlementInfoRevolutions> SettlementManager { get; }  = SubModule.SettlementManager;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            try
            {
                InformationManager.DisplayMessage(new InformationMessage("Loaded Revolutions.", Color.White));
            }
            catch (Exception exception)
            {
                string errorMessage = "Revolutions: Could not be loaded! ";
                InformationManager.DisplayMessage(new InformationMessage(errorMessage + exception?.ToString()));
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            try
            {
                base.OnGameStart(game, gameStarter);

                if (!(game.GameType is Campaign))
                {
                    return;
                }

                this.InitializeMod(gameStarter as CampaignGameStarter);
            }
            catch (Exception exception)
            {
                string exceptionMessage = "Revolutions: Failed to load on game start! ";
                InformationManager.DisplayMessage(new InformationMessage(exceptionMessage + exception?.ToString(), Color.FromUint(4282569842U)));
            }
        }

        private void InitializeMod(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                campaignGameStarter.LoadGameTexts(Path.Combine(SubModule.ModuleDataPath, "global_strings.xml"));

                SubModule.Configuration = FileManager.Instance.Load<Settings>(SubModule.ModuleDataPath, "Settings") ?? new Settings();

                this._dataStorage = new DataStorage();
                this.AddBehaviours(campaignGameStarter);
            }
            catch (Exception exception)
            {
                string exceptionMessage = "Revolutions: Failed to initialize! ";
                InformationManager.DisplayMessage(new InformationMessage(exceptionMessage + exception?.ToString(), Color.FromUint(4282569842U)));
            }
        }

        private void AddBehaviours(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new RevolutionBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new RevolutionDailyBehavior(ref this._dataStorage));
            campaignGameStarter.AddModel(new LoyaltyModel(ref this._dataStorage));
        }
    }
}
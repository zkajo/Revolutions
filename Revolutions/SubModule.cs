using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using Revolutions.Models;
using ModLibrary.Files;
using Revolutions.CampaignBehaviours;
using Revolutions.CampaignBehaviors;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private RevolutionDataStorage _revolutionDataStorage;

        public static Configuration Configuration;
        public static string ModuleDataPath = Path.Combine(BasePath.Name, "Modules", "Revolutions", "ModuleData");

        public SubModule()
        {
            this._revolutionDataStorage = new RevolutionDataStorage();
        }

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

                SubModule.Configuration = FileManager.Instance.Load<Configuration>(SubModule.ModuleDataPath, "Settings") ?? new Configuration();

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
            campaignGameStarter.AddBehavior(new RevolutionBehavior(ref this._revolutionDataStorage));
            campaignGameStarter.AddBehavior(new RevolutionDailyBehavior(ref this._revolutionDataStorage));
        }
    }
}
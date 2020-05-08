using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using ModLibrary;
using ModLibrary.Patches;
using Revolutions.CampaignBehaviors;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        internal static string ModuleDataPath => Path.Combine(BasePath.Name, "Modules", "Revolutions", "ModuleData");

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            InformationManager.DisplayMessage(new InformationMessage("Revolutions: Loaded Mod.", ColorManager.Green));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (!(game.GameType is Campaign))
            {
                return;
            }

            //Patcher.PatchAll();

            this.InitializeMod(gameStarter as CampaignGameStarter);
        }

        private void InitializeMod(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                this._dataStorage = new DataStorage();
                this.AddBehaviours(campaignGameStarter);
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Failed to initialize!", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorManager.Red));
            }
        }

        private void AddBehaviours(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new LuckyNationBehaviour());
            campaignGameStarter.AddBehavior(new RevolutionBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new RevolutionDailyBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new GuiHandlersBehavior());
            campaignGameStarter.AddBehavior(new CleanupBehavior());

            campaignGameStarter.AddModel(new Models.SettlementLoyaltyModel(ref this._dataStorage));
        }
    }
}
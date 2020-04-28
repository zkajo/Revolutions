using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.Library;
using Revolutions.CampaignBehaviours;
using Revolutions.Models;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private Revolution _revolutionBehaviour;
        private ModOptions _modOptions;
        private LoyaltyModel _loyaltyModel;
        private MobChecker _mobChecker;
        private DebugCampaignBehaviour _debugBehaviour;
        private Common _common;

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign))
            {
                return;
            }

            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            this.AddBehaviours(gameInitializer);
        }

        private void AddBehaviours(CampaignGameStarter gameInitializer)
        {
            this._modOptions = new ModOptions();
            this._revolutionBehaviour = new Revolution();
            this._mobChecker = new MobChecker();
            this._debugBehaviour = new DebugCampaignBehaviour();
            this._common = new Common();

            gameInitializer.AddBehavior(this._revolutionBehaviour);
            gameInitializer.AddBehavior(this._modOptions);
            gameInitializer.AddBehavior(this._mobChecker);
            gameInitializer.AddBehavior(this._debugBehaviour);

            this._loyaltyModel = new LoyaltyModel
            {
                RevolutionBehaviour = _revolutionBehaviour
            };
            gameInitializer.AddModel(this._loyaltyModel);

            gameInitializer.LoadGameTexts($"{BasePath.Name}Modules/Revolutions/ModuleData/global_strings.xml");
        }
    }

    public class SaveDefiner : SaveableTypeDefiner
    {
        public SaveDefiner() : base(350040)
        {
        }

        protected override void DefineClassTypes()
        {
            this.AddClassDefinition(typeof(SettlementInfo), 350041);
            this.AddClassDefinition(typeof(FactionInfo), 350042);
            this.AddClassDefinition(typeof(ModOptionsData), 350043);
        }

        protected override void DefineGenericClassDefinitions()
        {
            this.ConstructGenericClassDefinition(typeof(Tuple<PartyBase, SettlementInfo>));
        }

        protected override void DefineContainerDefinitions()
        {
            this.ConstructContainerDefinition(typeof(List<SettlementInfo>));
            this.ConstructContainerDefinition(typeof(List<FactionInfo>));
            this.ConstructContainerDefinition(typeof(List<Tuple<PartyBase, SettlementInfo>>));
        }
    }
}
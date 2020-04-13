using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using Revolutions.CampaignBehaviours;
using Revolutions.Models;

namespace Revolutions
{
    public class RevolutionsBase : MBSubModuleBase
    {
        private Revolution _revolutionBehaviour;
        private ModOptions _modOptions;
        private LoyaltyModel _loyaltyModel;
        private MobChecker _mobChecker;

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            Campaign campaign = game.GameType as Campaign;
            if (campaign == null) return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            AddBehaviours(gameInitializer);
        }

        private void AddBehaviours(CampaignGameStarter gameInitializer)
        {
            _modOptions = new ModOptions();
            _revolutionBehaviour = new Revolution();
            _mobChecker = new MobChecker();


            gameInitializer.AddBehavior(_revolutionBehaviour);
            gameInitializer.AddBehavior(_modOptions);
            gameInitializer.AddBehavior(_mobChecker);
            
            _loyaltyModel = new LoyaltyModel();
            _loyaltyModel.RevolutionBehaviour = _revolutionBehaviour;
            
            gameInitializer.AddModel(_loyaltyModel);
        }
    }

    public class SaveDefiner : SaveableTypeDefiner
    {
        public SaveDefiner() : base(350040)
        {
        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(SettlementInfo), 350041);
            AddClassDefinition(typeof(FactionInfo), 350042);
            AddClassDefinition(typeof(ModOptionsData), 350043);
        }

        protected override void DefineGenericClassDefinitions()
        {
            ConstructGenericClassDefinition(typeof(Tuple<PartyBase, SettlementInfo>));;
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<SettlementInfo>));
            ConstructContainerDefinition(typeof(List<FactionInfo>));
            ConstructContainerDefinition(typeof(List<Tuple<PartyBase, SettlementInfo>>));
        }
    }
}
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using Revolutions.CampaignBehaviours;
using Revolutions.Models;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace Revolutions
{
    public class RevolutionsBase : MBSubModuleBase
    {
        private Revolution _revolutionBehaviour;
        private ModOptions _modOptions;
        private LoyaltyModel _loyaltyModel;
        private MobChecker _mobChecker;
        private DebugCampaignBehaviour _debugBehaviour;
        private Common _common;
        
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
            _debugBehaviour = new DebugCampaignBehaviour();
            _common = new Common();
            
            gameInitializer.AddBehavior(_revolutionBehaviour);
            gameInitializer.AddBehavior(_modOptions);
            gameInitializer.AddBehavior(_mobChecker);
            gameInitializer.AddBehavior(_debugBehaviour);
            
            _loyaltyModel = new LoyaltyModel();
            _loyaltyModel.RevolutionBehaviour = _revolutionBehaviour;
            gameInitializer.AddModel(_loyaltyModel);
            
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
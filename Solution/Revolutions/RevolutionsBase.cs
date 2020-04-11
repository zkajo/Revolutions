using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Engine.GauntletUI;
using HarmonyLib;
using System.Windows;
using Revolutions.CampaignBehaviours;
using SandBox;
using TaleWorlds.Localization;
using Revolutions.Screens;

namespace Revolutions
{
    public class RevolutionsBase : MBSubModuleBase
    {
        private Revolution _revolutionBehaviour;
        private ModOptions _modOptions;

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
            gameInitializer.AddBehavior(_revolutionBehaviour);
            gameInitializer.AddBehavior(_modOptions);
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
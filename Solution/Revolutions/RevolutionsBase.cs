﻿using System;
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

namespace Revolutions
{
    public class RevolutionsBase : MBSubModuleBase
    {
        private Revolution RevolutionModule;

        protected override void OnSubModuleLoad()
        {
            try
            {
                var h = new Harmony("KommissarsBannerlordRevolutions");
                h.PatchAll();
            }
            catch (Exception exception1)
            {
                string message;
                Exception exception = exception1;
                string str = exception.Message;
                Exception innerException = exception.InnerException;
                if (innerException != null)
                {
                    message = innerException.Message;
                }
                else
                {
                    message = null;
                }
                MessageBox.Show(string.Concat("Error patching:\n", str, " \n\n", message));
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            Campaign campaign = game.GameType as Campaign;
            if (campaign == null) return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            AddBehaviours(gameInitializer);
        }

        private void AddBehaviours(CampaignGameStarter gameInitializer)
        {
            RevolutionModule = new Revolution();
            gameInitializer.AddBehavior(RevolutionModule);
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
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<SettlementInfo>));
            ConstructContainerDefinition(typeof(List<FactionInfo>));
        }
    }
}
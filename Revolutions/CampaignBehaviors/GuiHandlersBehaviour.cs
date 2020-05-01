﻿using System;
using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.Components.Factions;
using Revolutions.Factions;
using Revolutions.Screens;
using Revolutions.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace Revolutions.CampaignBehaviors
{
    public class GuiHandlersBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }
        
        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            CreateLoyaltyMenu(obj);
        }
        
        public static void CreateModOptionsMenu()
        {
            ScreenManager.PushScreen(new ModOptionsScreen());
        }
        
        private void CreateLoyaltyMenu(CampaignGameStarter obj)
        {
            TextObject menuName = GameTexts.FindText("str_GM_TownLoyalty");
            obj.AddGameMenuOption("town", "town_enter_entr_option", menuName.ToString(), (MenuCallbackArgs args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }, (MenuCallbackArgs args) =>
            {
                SettlementInfoRevolutions setinf = RevolutionsManagers.SettlementManager.GetSettlementInfo(Settlement.CurrentSettlement);
                FactionInfoRevolutions factinfo = RevolutionsManagers.FactionManager.GetFactionInfo(Settlement.CurrentSettlement.MapFaction);
                ScreenManager.PushScreen(new TownRevolutionsScreen(setinf, factinfo));
            }, false, 4);
        }
    }
}
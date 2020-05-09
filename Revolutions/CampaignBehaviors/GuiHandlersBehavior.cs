using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;
using Revolutions.Screens;

namespace Revolutions.CampaignBehaviors
{
    public class GuiHandlersBehavior : CampaignBehaviorBase
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
            this.CreateLoyaltyMenu(obj);
        }

        private void CreateLoyaltyMenu(CampaignGameStarter obj)
        {
            var menuName = new TextObject("{=Ts1iVN8d}Town Loyalty");
            obj.AddGameMenuOption("town", "town_enter_entr_option", menuName.ToString(), (MenuCallbackArgs args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }, (MenuCallbackArgs args) =>
            {
                var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(Settlement.CurrentSettlement);
                ScreenManager.PushScreen(new TownRevolutionsScreen(settlementInfo, settlementInfo.CurrentFactionInfoRevolutions));
            }, false, 4);
        }
    }
}
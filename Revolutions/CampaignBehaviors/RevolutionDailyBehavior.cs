using System;
using TaleWorlds.CampaignSystem;
using Revolutions.Components.Settlements;
using Revolutions.Components.Factions;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionDailyBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionDailyBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            RevolutionsManagers.RevolutionManager.IncreaseDailyLoyaltyForSettlement();
            RevolutionsManagers.RevolutionManager.CheckRevolutionProgress();
            this.UpdateSettlementInfos();
        }

        private void UpdateSettlementInfos()
        {
            foreach (var factionInfo in RevolutionsManagers.FactionManager.Infos)
            {
                factionInfo.DailyUpdate();
            }

            foreach (var settlementInfo in RevolutionsManagers.SettlementManager.Infos)
            {
                settlementInfo.DailyUpdate();
            }
        }
    }
}
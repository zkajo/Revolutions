using System;
using Revolutions.Factions;
using TaleWorlds.CampaignSystem;
using Revolutions.Revolutions;
using Revolutions.Settlements;

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

        private void DailyTickEvent()
        {
            RevolutionsManagers.RevolutionManager.IncreaseDailyLoyaltyForSettlement();
            RevolutionsManagers.RevolutionManager.CheckRevolutionProgress();
            UpdateSettlementInfos();
            
        }

        public override void SyncData(IDataStore dataStore)
        {
            //REMARK: We won't sync any data here, because the data will be synced inside RevolutionBehavior
        }

        private void UpdateSettlementInfos()
        {
            foreach (var factionInfo in RevolutionsManagers.FactionManager.FactionInfos)
            {
                factionInfo.DailyUpdate();
            }

            foreach (var settlementInfo in RevolutionsManagers.SettlementManager.SettlementInfos)
            {
                settlementInfo.DailyUpdate();
            }
        }
    }
}
using System;
using TaleWorlds.CampaignSystem;
using Revolutions.Revolutions;

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
        }

        public override void SyncData(IDataStore dataStore)
        {
            //REMARK: We won't sync any data here, because the data will be synced inside RevolutionBehavior
        }
    }
}
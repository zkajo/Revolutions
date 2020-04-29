using Revolutions.Revolutions;
using System;
using TaleWorlds.CampaignSystem;

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
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlementEvent));
        }

        private void DailyTickEvent()
        {
            throw new NotImplementedException();
        }

        private void DailyTickSettlementEvent(Settlement settlement)
        {
            if (!settlement.IsTown)
            {
                return;
            }

            RevolutionManager.Instance.IncreaseDailyLoyaltyForSettlement(settlement);
            bool startRevolution = RevolutionManager.Instance.CheckRevolutionProgress(settlement);
            if(startRevolution)
            {
                RevolutionManager.Instance.StartRevolution(settlement);
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            //REMARK: We won't sync any data here, because the data will be synced inside RevolutionBehavior
        }
    }
}
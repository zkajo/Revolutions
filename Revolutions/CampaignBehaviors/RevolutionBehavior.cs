using ModLibrary.Settlements;
using Revolutions.Settlements;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.SettlementEntered));
            CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeftEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                this.DataStorage.LoadData();
            }

            if (dataStore.IsSaving)
            {
                if (this.DataStorage.SaveId.IsEmpty())
                {
                    this.DataStorage.SaveId = Guid.NewGuid().ToString();
                }

                dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                this.DataStorage.SaveData();
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            if (this.DataStorage.SaveId.IsEmpty())
            {
                this.DataStorage.InitializeData();
            }
        }

        private void SettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            SettlementInfoRevolutions settlementInfo = SettlementManager<SettlementInfoRevolutions>.Instance.GetSettlementInfo(settlement.StringId);

            if (mobileParty.IsLordParty && mobileParty.Party.Owner.Clan.StringId == settlement.OwnerClan.StringId)
            {
                settlementInfo.IsOwnerInSettlement = true;
            }
        }

        private void OnSettlementLeftEvent(MobileParty mobileParty, Settlement settlement)
        {
            SettlementInfoRevolutions settlementInfo = SettlementManager<SettlementInfoRevolutions>.Instance.GetSettlementInfo(settlement.StringId);

            if (mobileParty.IsLordParty && mobileParty.Party.Owner.Clan.StringId == settlement.OwnerClan.StringId)
            {
                settlementInfo.IsOwnerInSettlement = false;
            }
        }
    }
}
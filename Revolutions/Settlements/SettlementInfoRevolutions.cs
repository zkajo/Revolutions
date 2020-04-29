using System;
using ModLibrary.Settlements;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Settlements
{
    [Serializable]
    public class SettlementInfoRevolutions : SettlementInfo
    {
        public SettlementInfoRevolutions()
        {

        }

        public SettlementInfoRevolutions(Settlement settlement) : base(settlement)
        {

        }

        public SettlementInfoRevolutions(SettlementInfo settlementInfo)
        {
            base.SettlementId = settlementInfo.SettlementId;
            base.InitialCultureId = settlementInfo.InitialCultureId;
            base.InitialFactionId = settlementInfo.InitialFactionId;
            base.CurrentFactionId = settlementInfo.CurrentFactionId;
            base.PreviousFactionId = settlementInfo.PreviousFactionId;
            this.LoyalFactionId = settlementInfo.InitialFactionId;
        }

        public string LoyalFactionId { get; set; }

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevolutionProgress { get; set; } = 0;

        public int DaysOwnedByOwner { get; set; } = 0;

        public IFaction LoyalFaction => RevolutionsManagers.FactionManager.GetFaction(this.LoyalFactionId);
    }
}
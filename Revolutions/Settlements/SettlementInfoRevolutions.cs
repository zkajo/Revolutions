using System;
using ModLibrary.Settlements;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Settlements
{
    [Serializable]
    public class SettlementInfoRevolutions : SettlementInfo
    {
        public SettlementInfoRevolutions() : base()
        {
            this.LoyalFactionId = base.InitialFactionId;
        }

        public SettlementInfoRevolutions(Settlement settlement) : base(settlement)
        {
            this.LoyalFactionId = base.InitialFactionId;
        }

        public string LoyalFactionId { get; set; }

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevolutionProgress { get; set; } = 0;

        public int DaysOwnedByOwner { get; set; } = 0;

        public IFaction LoyalFaction => RevolutionsManagers.FactionManager.GetFaction(this.LoyalFactionId);
    }
}
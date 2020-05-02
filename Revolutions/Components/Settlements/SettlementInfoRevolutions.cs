using System;
using TaleWorlds.CampaignSystem;
using ModLibrary.Settlements;
using ModLibrary.Factions;
using ModLibrary;
using Revolutions.Components.Factions;

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

        public IFaction LoyalFaction => ModLibraryManagers.FactionManager.GetFaction(this.LoyalFactionId);

        public FactionInfo LoyalFactionInfo => ModLibraryManagers.FactionManager.GetFactionInfo(this.LoyalFactionId);

        public FactionInfoRevolutions LoyalFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetFactionInfo(this.LoyalFactionId);

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevolutionProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        public bool IsLoyalFactionOfImperialCulture => RevolutionsManagers.FactionManager.GetFaction(this.LoyalFactionId).Name.ToLower().Contains("empire");
    }
}
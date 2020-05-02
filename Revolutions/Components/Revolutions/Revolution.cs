using System;
using TaleWorlds.CampaignSystem;
using ModLibrary;
using ModLibrary.Parties;
using ModLibrary.Settlements;
using Revolutions.Components.Parties;
using Revolutions.Settlements;

namespace Revolutions.Revolutions
{
    [Serializable]
    public class Revolution
    {
        public Revolution()
        {

        }

        public Revolution(string partyId, Settlement settlement)
        {
            this.PartyId = partyId;
            this.SettlementId = settlement.StringId;
        }

        public string PartyId { get; set; }

        public PartyBase Party => ModLibraryManagers.PartyManager.GetParty(this.PartyId);

        public PartyInfo PartyInfo => ModLibraryManagers.PartyManager.GetPartyInfo(this.PartyId);

        public PartyInfoRevolutions PartyInfoRevolutions => RevolutionsManagers.PartyManager.GetPartyInfo(this.PartyId);

        public string SettlementId { get; set; }

        public Settlement Settlement => ModLibraryManagers.SettlementManager.GetSettlement(this.SettlementId);

        public SettlementInfo SettlementInfo => ModLibraryManagers.SettlementManager.GetSettlementInfo(this.SettlementId);

        public SettlementInfoRevolutions SettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetSettlementInfo(this.SettlementId);
    }
}
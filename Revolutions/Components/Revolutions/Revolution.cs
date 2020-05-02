using Revolutions.Settlements;
using System;
using ModLibrary;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Revolutions
{
    [Serializable]
    public class Revolution
    {
        public Revolution(string partyID, Settlement settlement)
        {
            this.PartyId = partyID;
            this.SettlementId = settlement.StringId;
        }

        /// <summary>
        /// This is the PartyBase ID, NOT MobileParty
        /// </summary>
        public string PartyId { get; set; }

        public PartyBase Party => ModLibraryManagers.PartyManager.GetParty(this.PartyId);

        public string SettlementId { get; set; }

        public Settlement Settlement => RevolutionsManagers.SettlementManager.GetSettlement(this.SettlementId);

        public SettlementInfoRevolutions SettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetSettlementInfo(this.SettlementId);
    }
}
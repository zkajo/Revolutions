using Revolutions.Settlements;
using System;
using ModLibrary;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Revolutions
{
    [Serializable]
    public class Revolution
    {
        public Revolution()
        {

        }

        public string PartyId { get; set; }

        public PartyBase Party => ModLibraryManagers.PartyManager.GetParty(this.PartyId);

        public string SettlementId { get; set; }

        public Settlement Settlement => RevolutionsManagers.SettlementManager.GetSettlement(this.SettlementId);

        public SettlementInfoRevolutions SettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetSettlementInfo(this.SettlementId);
    }
}
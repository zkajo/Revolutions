using System;
using TaleWorlds.CampaignSystem;
using ModLibrary;
using ModLibrary.Components.Settlements;
using ModLibrary.Components.Parties;
using Revolutions.Components.Settlements;

namespace Revolutions.Revolutions
{
    [Serializable]
    public class Revolution
    {
        public Revolution()
        {

        }

        public Revolution(string partyId, Settlement settlement, bool isMinorFaction)
        {
            this.PartyId = partyId;
            this.SettlementId = settlement.StringId;
            this.IsMinorFaction = isMinorFaction;
        }

        #region Reference Properties

        public string PartyId { get; set; }

        public string SettlementId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public PartyBase Party => ModLibraryManagers.PartyManager.GetObjectById(this.PartyId);

        public PartyInfo PartyInfo => ModLibraryManagers.PartyManager.GetInfoById(this.PartyId);

        public Settlement Settlement => ModLibraryManagers.SettlementManager.GetObjectById(this.SettlementId);

        public SettlementInfo SettlementInfo => ModLibraryManagers.SettlementManager.GetInfoById(this.SettlementId);

        public SettlementInfoRevolutions SettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetInfoById(this.SettlementId);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsMinorFaction { get; set; } = false;

        #endregion
    }
}
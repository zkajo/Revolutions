using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components;
using KNTLibrary.Components.Settlements;
using KNTLibrary.Components.Parties;
using Revolutions.Components.Settlements;
using Revolutions.Components.Parties;

namespace Revolutions.Components.Revolutions
{
    [Serializable]
    public class Revolution : IGameComponent<Revolution>
    {
        #region IGameComponent<InfoType>

        public bool Equals(Revolution other)
        {
            return this.PartyId == other.PartyId && this.SettlementId == other.SettlementId;
        }

        public override bool Equals(object other)
        {
            if (other is Revolution revolution)
            {
                return this.PartyId == revolution.PartyId && this.SettlementId == revolution.SettlementId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.PartyId.GetHashCode() ^ this.SettlementId.GetHashCode();
        }

        #endregion
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

        public PartyBase Party => RevolutionsManagers.PartyManager.GetGameObject(this.PartyId);

        public PartyInfo PartyInfo => RevolutionsManagers.PartyManager.GetInfo(this.Party);

        public PartyInfoRevolutions PartyInfoRevolutions => RevolutionsManagers.PartyManager.GetInfo(this.Party);

        public Settlement Settlement => RevolutionsManagers.SettlementManager.GetGameObject(this.SettlementId);

        public SettlementInfo SettlementInfo => RevolutionsManagers.SettlementManager.GetInfo(this.Settlement);

        public SettlementInfoRevolutions SettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetInfo(this.Settlement);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsMinorFaction { get; set; } = false;

        #endregion
    }
}
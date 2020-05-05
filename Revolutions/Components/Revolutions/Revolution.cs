﻿using System;
using TaleWorlds.CampaignSystem;
using ModLibrary;
using ModLibrary.Components;
using ModLibrary.Components.Settlements;
using ModLibrary.Components.Parties;
using Revolutions.Components.Settlements;

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
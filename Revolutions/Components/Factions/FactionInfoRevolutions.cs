using System;
using TaleWorlds.CampaignSystem;
using ModLibrary;
using ModLibrary.Components.Settlements;
using ModLibrary.Components.Factions;
using Revolutions.Components.Settlements;

namespace Revolutions.Components.Factions
{
    [Serializable]
    public class FactionInfoRevolutions : FactionInfo
    {
        public FactionInfoRevolutions() : base()
        {

        }

        public FactionInfoRevolutions(IFaction faction) : base(faction)
        {

        }

        #region Reference Properties

        public string RevoltedSettlementId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Settlement RevoltedSettlement => ModLibraryManagers.SettlementManager.GetObjectById(this.RevoltedSettlementId);

        public SettlementInfo RevoltedSettlementInfo => ModLibraryManagers.SettlementManager.GetInfoById(this.RevoltedSettlementId);

        public SettlementInfoRevolutions RevoltedSettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetInfoById(this.RevoltedSettlementId);

        #endregion

        #region Reference Properties Inherited



        #endregion



        #endregion

        #region Normal Properties

        public bool CanRevolt { get; set; } = false;

        public int DaysSinceLastRevolt { get; set; } = 0;

        public bool SuccessfullyRevolted { get; set; } = false;

        #endregion
    }
}
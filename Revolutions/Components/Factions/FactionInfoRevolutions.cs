using System;
using TaleWorlds.CampaignSystem;
using ModLibrary;
using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.Settlements;

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

        public string RevoltedSettlementId { get; set; }

        public Settlement RevoltedSettlement => ModLibraryManagers.SettlementManager.GetSettlement(this.RevoltedSettlementId);

        public SettlementInfo RevoltedSettlementInfo => ModLibraryManagers.SettlementManager.GetSettlementInfo(this.RevoltedSettlementId);

        public SettlementInfoRevolutions RevoltedSettlementInfoRevolutions => RevolutionsManagers.SettlementManager.GetSettlementInfo(this.RevoltedSettlementId);

        public bool CanRevolt { get; set; } = false;

        public int DaysSinceLastRevolt { get; set; } = 0;

        public bool SuccessfullyRevolted { get; set; } = false;
    }
}
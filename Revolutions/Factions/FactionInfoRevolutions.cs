using System;
using ModLibrary.Factions;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Factions
{
    [Serializable]
    public class FactionInfoRevolutions : FactionInfo
    {
        public FactionInfoRevolutions()
        {

        }

        public FactionInfoRevolutions(IFaction faction) : base(faction)
        {

        }

        public FactionInfoRevolutions(FactionInfo factionInfo)
        {
            base.FactionId = factionInfo.FactionId;
            base.InitialTownsCount = factionInfo.InitialTownsCount;
        }

        public string RevoltedSettlementId { get; set; }

        public bool CanRevolt { get; set; } = false;

        public int DaysSinceLastRevolution { get; set; } = 0;

        public bool SuccessfullyRevolted { get; set; } = false;
    }
}
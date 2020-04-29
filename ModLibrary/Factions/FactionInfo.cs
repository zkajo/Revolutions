using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Factions
{
    [Serializable]
    public class FactionInfo
    {
        public FactionInfo()
        {

        }

        public FactionInfo(IFaction faction)
        {
            this.FactionId = faction.StringId;
            this.InitialTownsCount = faction.Settlements.Count();
            this.CurrentTownsCount = faction.Settlements.Count();
        }

        public string FactionId { get; set; }

        public int InitialTownsCount { get; set; } = 0;

        public int CurrentTownsCount { get; set; } = 0;
    }
}
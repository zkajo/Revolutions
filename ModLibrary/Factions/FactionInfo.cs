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
            this._faction = faction;
            this.FactionId = faction.StringId;
            this.InitialTownsCount = CountTowns();
        }

        public string FactionId { get; set; }

        public int InitialTownsCount { get; set; } = 0;

        public int CurrentTownsCount => CountTowns();

        private IFaction _faction;

        private int CountTowns()
        {
            int count = 0;
            
            foreach (var town in _faction.Settlements.Where(n => n.IsTown))
            {
                count++;
            }

            return count;
        } 
    }
}
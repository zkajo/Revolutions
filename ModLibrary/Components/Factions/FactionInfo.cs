using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Factions
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
            this.InitialTownsCount = this.CurrentTownsCount;
        }

        public string FactionId { get; set; }

        public IFaction Faction => ModLibraryManagers.FactionManager.GetObjectById(this.FactionId);

        public int InitialTownsCount { get; set; }

        public int CurrentTownsCount => this.Faction.Settlements.Where(settlement => settlement.IsTown).Count();
    }
}
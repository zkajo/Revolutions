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
            InitialTownsCount = ModLibraryManagers.FactionManager.GetFaction(this.FactionId).Settlements.Where(settlement => settlement.IsTown).Count();
        }

        public string FactionId { get; set; }

        public IFaction Faction => ModLibraryManagers.FactionManager.GetFaction(this.FactionId);

        public int InitialTownsCount { get; set; }

        public int CurrentTownsCount => ModLibraryManagers.FactionManager.GetFaction(this.FactionId).Settlements.Where(settlement => settlement.IsTown).Count();

        public bool Remove { get; set; } = false;
    }
}
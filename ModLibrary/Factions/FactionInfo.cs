using System;
using System.Linq;

namespace ModLibrary.Factions
{
    [Serializable]
    public class FactionInfo
    {
        public FactionInfo()
        {

        }

        public string FactionId { get; set; }

        public int InitialTownsCount { get; set; } = 0;

        public int CurrentTownsCount => ModLibraryManagers.FactionManager.GetFaction(this.FactionId).Settlements.Where(settlement => settlement.IsTown).Count();
    }
}
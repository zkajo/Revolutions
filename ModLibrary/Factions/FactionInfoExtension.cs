using System.Linq;

namespace ModLibrary.Factions
{
    public static class FactionInfoExtension
    {
        public static void UpdateCurrentTownsCount(this FactionInfo factionInfo)
        {
            factionInfo.CurrentTownsCount = FactionManager<FactionInfo>.Instance.GetFaction(factionInfo).Settlements.Count();
        }
    }
}
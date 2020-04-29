using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Factions
{
    public static class FactionInfoExtension
    {
        public static IFaction GetFaction(this FactionInfo factionInfo)
        {
            return FactionManager<FactionInfo>.Instance.GetFaction(factionInfo);
        }

        public static void UpdateCurrentTownsCount(this FactionInfo factionInfo)
        {
            factionInfo.CurrentTownsCount = FactionManager<FactionInfo>.Instance.GetFaction(factionInfo).Settlements.Count();
        }
    }
}
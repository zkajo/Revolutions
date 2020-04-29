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
    }
}
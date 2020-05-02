using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public static class SettlementInfoExtension
    {
        public static bool IsOfInitialCulture(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.InitialCultureId == cultureStringId;

        }

        public static bool IsOfInitialCulture(this SettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsOfInitialCulture(culture.StringId);
        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.Settlement.Culture.StringId == cultureStringId;
        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsOfCulture(culture.StringId);
        }

        public static void UpdateOwner(this SettlementInfo settlementInfo, IFaction faction = null)
        {
            if (faction == null)
            {
                settlementInfo.PreviousFactionId = settlementInfo.CurrentFactionId;
            }
            else
            {
                settlementInfo.PreviousFactionId = settlementInfo.CurrentFactionId;
                settlementInfo.CurrentFactionId = faction.StringId;
            }
        }

        public static PartyBase GetGarrison(this SettlementInfo settlementInfo)
        {
            return settlementInfo.Settlement.Parties.FirstOrDefault(n => n.IsGarrison).Party;
        }

        public static PartyBase GetMilitia(this SettlementInfo settlementInfo)
        {
            return settlementInfo.Settlement.Parties.FirstOrDefault(n => n.IsMilitia).Party;
        }
    }
}
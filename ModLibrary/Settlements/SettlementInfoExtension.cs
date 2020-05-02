using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public static class SettlementInfoExtension
    {
        public static bool IsInitialCultureOf(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.InitialCultureId == cultureStringId;

        }
        public static bool IsInitialCultureOf(this SettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsInitialCultureOf(culture.StringId);
        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.Settlement.Culture.StringId == cultureStringId;
        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsOfCulture(culture.StringId);
        }

        public static PartyBase GetGarrison(this SettlementInfo settlementInfo)
        {
            return settlementInfo.Settlement.Parties.FirstOrDefault(party => party.IsGarrison).Party;
        }

        public static PartyBase GetMilitia(this SettlementInfo settlementInfo)
        {
            return settlementInfo.Settlement.Parties.FirstOrDefault(party => party.IsMilitia).Party;
        }
    }
}
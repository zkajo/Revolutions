using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Settlements
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
    }
}
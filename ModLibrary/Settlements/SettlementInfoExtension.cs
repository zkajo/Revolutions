using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public static class SettlementInfoExtension
    {
        public static void UpdateOwner(this SettlementInfo settlementInfo, IFaction faction = null)
        {
            if (faction == null)
            {
                settlementInfo.PreviousFaction = settlementInfo.CurrentFaction;
            }
            else
            {
                settlementInfo.PreviousFaction = settlementInfo.CurrentFaction;
                settlementInfo.CurrentFaction = faction;
            }
        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.OriginalCultureId == cultureStringId;
        }

        public static bool IsOriginalOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.OriginalFaction.Culture.Name.ToLower().Contains("empire");
        }

        public static bool IsCurrentOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.CurrentFaction.Culture.Name.ToLower().Contains("empire");
        }

        public static bool IsPreviousOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.PreviousFaction.Culture.Name.ToLower().Contains("empire");
        }
    }
}
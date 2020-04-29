using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ModLibrary.Settlements
{
    public static class SettlementInfoExtension
    {
        public static Settlement GetSettlement(this SettlementInfo settlementInfo)
        {
            return Settlement.Find(settlementInfo.SettlementId);
        }

        public static CultureObject GetOriginalCulture(this SettlementInfo settlementInfo)
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(settlementInfo.OriginalCultureId);
        }

        public static IFaction GetOriginalFaction(this SettlementInfo settlementInfo)
        {
            foreach (IFaction faction in Campaign.Current.Factions)
            {
                if (faction.StringId == settlementInfo.OriginalFactionId)
                {
                    return faction;
                }
            }

            return null;
        }

        public static IFaction GetCurrentFaction(this SettlementInfo settlementInfo)
        {
            return settlementInfo.GetSettlement().MapFaction;
        }

        public static IFaction GetPreviousFaction(this SettlementInfo settlementInfo)
        {
            foreach (IFaction faction in Campaign.Current.Factions)
            {
                if (faction.StringId == settlementInfo.PreviousFactionId)
                {
                    return faction;
                }
            }

            return null;
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

        public static bool IsOfCulture(this SettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.OriginalCultureId == culture.StringId;
        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.OriginalCultureId == cultureStringId;
        }

        public static bool IsOriginalOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.GetOriginalCulture().Name.ToLower().Contains("empire");
        }

        public static bool IsCurrentOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.GetCurrentFaction().Culture.Name.ToLower().Contains("empire");
        }

        public static bool IsPreviousOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.GetPreviousFaction().Culture.Name.ToLower().Contains("empire");
        }
    }
}
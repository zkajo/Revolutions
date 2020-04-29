﻿using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public static class SettlementInfoExtension
    {
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

        public static bool IsOfCulture(this SettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.InitialCultureId == cultureStringId;

        }

        public static bool IsOfCulture(this SettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsOfCulture(culture.StringId);
        }

        public static bool IsOriginalOwnerOfImperialCulture(this SettlementInfo settlementInfo)
        {
            return settlementInfo.InitialCulture.Name.ToLower().Contains("empire");
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
﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using ModLibrary.Factions;

namespace ModLibrary.Settlements
{
    public static class SettlementInfoExtension
    {
        public static CultureObject GetOriginalCulture(this SettlementInfo settlementInfo)
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(settlementInfo.InitialCultureId);
        }

        public static IFaction GetInitialFaction(this SettlementInfo settlementInfo)
        {
            return FactionManager<FactionInfo>.Instance.GetFaction(settlementInfo.InitialFactionId).MapFaction;
        }

        public static IFaction GetOriginalFaction(this SettlementInfo settlementInfo)
        {
            return FactionManager<FactionInfo>.Instance.GetFaction(settlementInfo.OriginalFactionId).MapFaction;
        }

        public static IFaction GetCurrentFaction(this SettlementInfo settlementInfo)
        {
            return FactionManager<FactionInfo>.Instance.GetFaction(settlementInfo.CurrentFactionId).MapFaction;
        }

        public static IFaction GetPreviousFaction(this SettlementInfo settlementInfo)
        {
            return FactionManager<FactionInfo>.Instance.GetFaction(settlementInfo.PreviousFactionId).MapFaction;
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
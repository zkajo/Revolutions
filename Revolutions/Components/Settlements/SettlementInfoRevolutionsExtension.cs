using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Settlements
{
    public static class SettlementInfoRevolutionsExtension
    {
        public static void UpdateOwnerRevolution(this SettlementInfoRevolutions settlementInfo, IFaction faction = null)
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

            settlementInfo.DaysOwnedByOwner = 0;
        }

        public static void DailyUpdate(this SettlementInfoRevolutions settlementInfo)
        {
            settlementInfo.DaysOwnedByOwner++;

            if (settlementInfo.LoyalFactionId != settlementInfo.CurrentFactionId &&
                settlementInfo.DaysOwnedByOwner > Settings.Instance.DaysUntilLoyaltyChange)
            {
                settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;
            }
        }
    }
}
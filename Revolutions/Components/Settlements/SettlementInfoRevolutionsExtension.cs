using TaleWorlds.CampaignSystem;

namespace Revolutions.Settlements
{
    public static class SettlementInfoRevolutionsExtension
    {
        public static void UpdateOwnerRevolution(this SettlementInfoRevolutions settlementInfoRevolutions, IFaction faction = null)
        {
            if (faction == null)
            {
                settlementInfoRevolutions.PreviousFactionId = settlementInfoRevolutions.CurrentFactionId;
            }
            else
            {
                settlementInfoRevolutions.PreviousFactionId = settlementInfoRevolutions.CurrentFactionId;
                settlementInfoRevolutions.CurrentFactionId = faction.StringId;
            }

            settlementInfoRevolutions.DaysOwnedByOwner = 0;
        }

        public static void DailyUpdate(this SettlementInfoRevolutions settlementInfoRevolutions)
        {
            settlementInfoRevolutions.DaysOwnedByOwner++;

            if (settlementInfoRevolutions.LoyalFactionId != settlementInfoRevolutions.CurrentFactionId &&
                settlementInfoRevolutions.DaysOwnedByOwner > Settings.Instance.DaysUntilLoyaltyChange)
            {
                settlementInfoRevolutions.LoyalFactionId = settlementInfoRevolutions.CurrentFactionId;
            }
        }
    }
}
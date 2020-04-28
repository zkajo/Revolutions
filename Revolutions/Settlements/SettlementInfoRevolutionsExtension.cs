using TaleWorlds.CampaignSystem;
using ModLibrary.Settlements;

namespace Revolutions.Settlements
{
    public static class SettlementInfoRevolutionsExtension
    {
        public static void UpdateOwnerRevolution(this SettlementInfoRevolutions settlementInfoRevolutions, IFaction faction = null)
        {
            ((SettlementInfo)settlementInfoRevolutions).UpdateOwner(faction);

            if (settlementInfoRevolutions.OriginalFactionId == settlementInfoRevolutions.CurrentFactionId)
            {
                return;
            }

            if (settlementInfoRevolutions.DaysOwnedByOwner >= SubModule.Configuration.DaysUntilLoyaltyChange)
            {
                settlementInfoRevolutions.CurrentFaction = settlementInfoRevolutions.CurrentFaction;
            }

            settlementInfoRevolutions.DaysOwnedByOwner++;
        }

        public static void ResetOwnership(this SettlementInfoRevolutions settlementInfoRevolutions)
        {
            settlementInfoRevolutions.RevolutionProgress = 0;
            settlementInfoRevolutions.DaysOwnedByOwner = 0;
        }
    }
}
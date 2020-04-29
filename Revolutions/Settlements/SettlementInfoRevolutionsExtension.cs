using ModLibrary.Factions;
using TaleWorlds.CampaignSystem;
using ModLibrary.Settlements;

namespace Revolutions.Settlements
{
    public static class SettlementInfoRevolutionsExtension
    {
        public static void UpdateOwnerRevolution(this SettlementInfoRevolutions settlementInfoRevolutions, IFaction faction = null)
        {
            settlementInfoRevolutions.UpdateOwner(faction);

            if (settlementInfoRevolutions.LoyalFactionId == settlementInfoRevolutions.CurrentFactionId)
            {
                return;
            }

            if (settlementInfoRevolutions.DaysOwnedByOwner >= SubModule.Configuration.DaysUntilLoyaltyChange)
            {
                settlementInfoRevolutions.LoyalFactionId = settlementInfoRevolutions.CurrentFactionId;
            }

            settlementInfoRevolutions.DaysOwnedByOwner++;
        }

        public static void ResetOwnership(this SettlementInfoRevolutions settlementInfoRevolutions)
        {
            settlementInfoRevolutions.RevolutionProgress = 0;
            settlementInfoRevolutions.DaysOwnedByOwner = 0;
        }

        public static IFaction GetLoyalFaction(this SettlementInfoRevolutions settlementInfo)
        {
            return RevolutionsManagers.FactionManager.GetFaction(settlementInfo.LoyalFactionId).MapFaction;
        }
    }
}
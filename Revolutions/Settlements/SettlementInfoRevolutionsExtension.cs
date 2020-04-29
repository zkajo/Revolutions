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

            if (settlementInfoRevolutions.LoyalFactionID == settlementInfoRevolutions.CurrentFactionId)
            {
                return;
            }

            if (settlementInfoRevolutions.DaysOwnedByOwner >= SubModule.Configuration.DaysUntilLoyaltyChange)
            {
                settlementInfoRevolutions.LoyalFactionID = settlementInfoRevolutions.CurrentFactionId;
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
            return FactionManager<FactionInfo>.Instance.GetFaction(settlementInfo.LoyalFactionID).MapFaction;
        }
    }
}
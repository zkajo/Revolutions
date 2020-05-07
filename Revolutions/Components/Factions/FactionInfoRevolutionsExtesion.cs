using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Factions
{
    public static class FactionInfoRevolutionsExtesion
    {
        public static void CityRevoltionFailed(this FactionInfoRevolutions factionInfo, Settlement settlement)
        {
            factionInfo.CanRevolt = false;
            factionInfo.RevoltedSettlementId = settlement.Id.InternalValue;
            factionInfo.DaysSinceLastRevolt = 0;
            factionInfo.SuccessfullyRevolted = false;

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.Id.InternalValue);
            settlementInfo.HasRebellionEvent = false;
            settlementInfo.RevolutionProgress = 0;
        }

        public static void CityRevoltionSucceeded(this FactionInfoRevolutions factionInfo, Settlement settlement)
        {
            factionInfo.CanRevolt = false;
            factionInfo.RevoltedSettlementId = settlement.Id.InternalValue;
            factionInfo.DaysSinceLastRevolt = 0;
            factionInfo.SuccessfullyRevolted = true;

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.Id.InternalValue);
            settlementInfo.HasRebellionEvent = false;
            settlementInfo.RevolutionProgress = 0;
        }

        public static void DailyUpdate(this FactionInfoRevolutions factionInfo)
        {
            factionInfo.DaysSinceLastRevolt++;

            if (factionInfo.DaysSinceLastRevolt > Settings.Instance.RevoltCooldownTime)
            {
                factionInfo.CanRevolt = true;
            }
        }
    }
}
using TaleWorlds.CampaignSystem;

namespace Revolutions.Factions
{
    public static class FactionInfoRevolutionsExtesion
    {
        public static void CityRevoltionFailed(this FactionInfoRevolutions factionInfoRevolutions, Settlement settlement)
        {
            factionInfoRevolutions.CanRevolt = false;
            factionInfoRevolutions.RevoltedSettlementId = settlement.StringId;
            factionInfoRevolutions.DaysSinceLastRevolution = 0;
            factionInfoRevolutions.SuccessfullyRevolted = false;
        }

        public static void CityRevoltionSucceeded(this FactionInfoRevolutions factionInfoRevolutions, Settlement settlement)
        {
            factionInfoRevolutions.CanRevolt = true;
            factionInfoRevolutions.RevoltedSettlementId = settlement.StringId;
            factionInfoRevolutions.DaysSinceLastRevolution = 0;
            factionInfoRevolutions.SuccessfullyRevolted = true;
        }
    }
}
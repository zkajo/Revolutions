using ModLibrary.Components.Factions;
using ModLibrary.Components.Settlements;
using Revolutions.Components.Factions;
using Revolutions.Components.Settlements;
using Revolutions.Revolutions;

namespace Revolutions
{
    public static class RevolutionsManagers
    {
        public static FactionManager<FactionInfoRevolutions> FactionManager { get; } = FactionManager<FactionInfoRevolutions>.Instance;

        public static SettlementManager<SettlementInfoRevolutions> SettlementManager { get; } = SettlementManager<SettlementInfoRevolutions>.Instance;

        public static RevolutionManager RevolutionManager { get; } = RevolutionManager.Instance;
    }
}
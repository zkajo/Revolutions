using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.Factions;
using Revolutions.Revolutions;
using Revolutions.Settlements;

namespace Revolutions
{
    public static class RevolutionsManagers
    {
        public static FactionManager<FactionInfoRevolutions> FactionManager { get; } = FactionManager<FactionInfoRevolutions>.Instance;

        public static SettlementManager<SettlementInfoRevolutions> SettlementManager { get; } = SettlementManager<SettlementInfoRevolutions>.Instance;

        public static RevolutionManager RevolutionManager { get; } = RevolutionManager.Instance;
    }
}

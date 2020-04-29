using ModLibrary.Factions;
using ModLibrary.Settlements;

namespace ModLibrary
{
    public static class ModLibraryManagers
    {
        public static FactionManager<FactionInfo> FactionManager { get; } = FactionManager<FactionInfo>.Instance;

        public static SettlementManager<SettlementInfo> SettlementManager { get; } = SettlementManager<SettlementInfo>.Instance;
    }
}
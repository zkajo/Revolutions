using ModLibrary.Factions;
using ModLibrary.Files;
using ModLibrary.Party;
using ModLibrary.Settlements;

namespace ModLibrary
{
    public static class ModLibraryManagers
    {
        public static FileManager FileManager { get; } = FileManager.Instance;

        public static FactionManager<FactionInfo> FactionManager { get; } = FactionManager<FactionInfo>.Instance;

        public static SettlementManager<SettlementInfo> SettlementManager { get; } = SettlementManager<SettlementInfo>.Instance;

        public static PartyManager PartyManager { get; } = PartyManager.Instance;
    }
}
using ModLibrary.Clans;
using ModLibrary.Factions;
using ModLibrary.Files;
using ModLibrary.Parties;
using ModLibrary.Settlements;

namespace ModLibrary
{
    public static class ModLibraryManagers
    {
        public static FileManager FileManager { get; } = FileManager.Instance;

        public static FactionManager<FactionInfo> FactionManager { get; } = FactionManager<FactionInfo>.Instance;

        public static SettlementManager<SettlementInfo> SettlementManager { get; } = SettlementManager<SettlementInfo>.Instance;

        public static ClanManager<ClanInfo> ClanManager { get; } = ClanManager<ClanInfo>.Instance;

        public static PartyManager<PartyInfo> PartyManager { get; } = PartyManager<PartyInfo>.Instance;
    }
}
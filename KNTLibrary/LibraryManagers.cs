using KNTLibrary.Components.Kingdoms;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Clans;
using KNTLibrary.Components.Parties;
using KNTLibrary.Components.Characters;
using KNTLibrary.Components.Settlements;

namespace KNTLibrary
{
    public static class LibraryManagers
    {
        public static FileManager FileManager { get; } = FileManager.Instance;

        public static KingdomManager<KingdomInfo> KingdomManager { get; } = KingdomManager<KingdomInfo>.Instance;

        public static FactionManager<FactionInfo> FactionManager { get; } = FactionManager<FactionInfo>.Instance;

        public static ClanManager<ClanInfo> ClanManager { get; } = ClanManager<ClanInfo>.Instance;

        public static PartyManager<PartyInfo> PartyManager { get; } = PartyManager<PartyInfo>.Instance;

        public static CharacterManager<CharacterInfo> CharacterManager { get; } = CharacterManager<CharacterInfo>.Instance;

        public static SettlementManager<SettlementInfo> SettlementManager { get; } = SettlementManager<SettlementInfo>.Instance;
    }
}
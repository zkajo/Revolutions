using ModLibrary.Components.Kingdoms;
using ModLibrary.Components.Factions;
using ModLibrary.Components.Clans;
using ModLibrary.Components.Parties;
using ModLibrary.Components.Characters;
using ModLibrary.Components.Settlements;

namespace ModLibrary
{
    public static class ModLibraryManagers
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
using ModLibrary.Factions;
using ModLibrary.Parties;
using ModLibrary.Settlements;
using Revolutions.Components.Factions;
using Revolutions.Components.Parties;
using Revolutions.Factions;
using Revolutions.Revolutions;
using Revolutions.Settlements;

namespace Revolutions
{
    public static class RevolutionsManagers
    {
        public static FactionManager<FactionInfoRevolutions> FactionManager { get; } = FactionManager<FactionInfoRevolutions>.Instance;

        public static SettlementManager<SettlementInfoRevolutions> SettlementManager { get; } = SettlementManager<SettlementInfoRevolutions>.Instance;

        public static PartyManager<PartyInfoRevolutions> PartyManager { get; } = PartyManager<PartyInfoRevolutions>.Instance;
        
        public static RevolutionManager RevolutionManager { get; } = RevolutionManager.Instance;
    }
}

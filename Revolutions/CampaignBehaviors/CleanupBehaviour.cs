using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class CleanupBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        private void TickEvent(float dt)
        {
            //TODO: Add similar functions for other manager
            RevolutionsManagers.FactionManager.WatchFactions();
            RevolutionsManagers.SettlementManager.WatchSettlements();
            RevolutionsManagers.PartyManager.WatchParties();
        }
    }
}
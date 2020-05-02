using System;
using System.Linq;
using ModLibrary.Clans;
using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class CleanupBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.PartyRemovedEvent));
            CampaignEvents.OnLordPartySpawnedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.LordPartySpawned));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        private void ClanDestroyedEvent(Clan clan)
        {
            RevolutionsManagers.ClanManager.RemoveClanInfo(clan.StringId);
        }

        private void PartyRemovedEvent(PartyBase party)
        {
            RevolutionsManagers.PartyManager.RemovePartyInfo(party.Id);
        }

        private void LordPartySpawned(MobileParty party)
        {
            RevolutionsManagers.PartyManager.AddPartyInfo(party.Party);
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            ClanInfo info = RevolutionsManagers.ClanManager.GetClanInfo(clan);
            
            if (!info.CanJoinOtherKingdoms && newKingdom.RulingClan.StringId != clan.StringId)
            {
                clan.ClanLeaveKingdom(false);
            }
        }

        private void TickEvent(float dt)
        {
            //TODO: Add similar functions for other manager
            //TODO: Better functionality here. These are too slow.
            //RevolutionsManagers.FactionManager.WatchFactions();
            //RevolutionsManagers.SettlementManager.WatchSettlements();
            //RevolutionsManagers.PartyManager.WatchParties();
            //RevolutionsManagers.ClanManager.WatchClans();
        }
    }
}
using System;
using ModLibrary.Clans;
using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class CleanupBehavior : CampaignBehaviorBase
    {
        private const int RefreshAtTick = 100;

        private int _currentTick = 0;

        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.PartyRemovedEvent));
            CampaignEvents.OnLordPartySpawnedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.LordPartySpawned));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
            CampaignEvents.PartyVisibilityChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.PartyVisibilityChangedEvent));
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

        private void PartyVisibilityChangedEvent(PartyBase party)
        {

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

            if (this._currentTick == CleanupBehavior.RefreshAtTick)
            {
                RevolutionsManagers.FactionManager.WatchFactions();
            }

            if (this._currentTick == CleanupBehavior.RefreshAtTick + 10)
            {
                RevolutionsManagers.SettlementManager.WatchSettlements();
            }

            if (this._currentTick > CleanupBehavior.RefreshAtTick + 20)
            {
                RevolutionsManagers.ClanManager.WatchClans();

            }
            if (this._currentTick == CleanupBehavior.RefreshAtTick + 30)
            {
                RevolutionsManagers.PartyManager.WatchParties();
                this._currentTick = 0;
            }

            this._currentTick++;
        }
    }
}
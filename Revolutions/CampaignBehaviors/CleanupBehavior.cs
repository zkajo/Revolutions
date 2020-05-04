using System;
using ModLibrary;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;

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
            ModLibraryManagers.ClanManager.RemoveInfo(clan.StringId);
        }

        private void PartyRemovedEvent(PartyBase party)
        {
            ModLibraryManagers.PartyManager.RemoveInfo(party.Id);
        }

        private void PartyVisibilityChangedEvent(PartyBase party)
        {

        }

        private void LordPartySpawned(MobileParty party)
        {
            ModLibraryManagers.PartyManager.AddInfo(party.Party);
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = ModLibraryManagers.ClanManager.GetInfoById(clan.StringId);

            if (!clanInfo.CanJoinOtherKingdoms && newKingdom.RulingClan.StringId != clan.StringId)
            {
                clan.ClanLeaveKingdom(false);
            }
        }

        private void TickEvent(float dt)
        {
            if (this._currentTick == CleanupBehavior.RefreshAtTick)
            {
                RevolutionsManagers.KingdomManager.UpdateInfos();
            }

            if (this._currentTick == CleanupBehavior.RefreshAtTick + 10)
            {
                RevolutionsManagers.FactionManager.UpdateInfos();
            }

            if (this._currentTick == CleanupBehavior.RefreshAtTick + 20)
            {
                RevolutionsManagers.ClanManager.UpdateInfos();
            }

            if (this._currentTick == CleanupBehavior.RefreshAtTick + 30)
            {
                RevolutionsManagers.PartyManager.UpdateInfos();
            }

            if (this._currentTick == CleanupBehavior.RefreshAtTick + 40)
            {
                RevolutionsManagers.CharacterManager.UpdateInfos();
            }

            if (this._currentTick > CleanupBehavior.RefreshAtTick + 50)
            {
                ModLibraryManagers.SettlementManager.UpdateInfos();
                this._currentTick = 0;
            }
            
            this._currentTick++;
        }
    }
}
using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class CleanupBehavior : CampaignBehaviorBase
    {
        private const int RefreshAtTick = 0;

        private int _currentTick = 0;

        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));

            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.PartyRemovedEvent));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void TickEvent(float dt)
        {
            if (this._currentTick == CleanupBehavior.RefreshAtTick)
            {
                RevolutionsManagers.FactionManager.UpdateInfos();

            }
            else if (this._currentTick == CleanupBehavior.RefreshAtTick + 20)
            {
                RevolutionsManagers.KingdomManager.UpdateInfos();
            }
            else if (this._currentTick == CleanupBehavior.RefreshAtTick + 40)
            {
                RevolutionsManagers.ClanManager.UpdateInfos();
            }
            else if (this._currentTick == CleanupBehavior.RefreshAtTick + 60)
            {
                RevolutionsManagers.SettlementManager.UpdateInfos();
            }
            else if (this._currentTick == CleanupBehavior.RefreshAtTick + 80)
            {
                RevolutionsManagers.PartyManager.UpdateInfos();
            }
            else if (this._currentTick >= CleanupBehavior.RefreshAtTick + 100)
            {
                RevolutionsManagers.CharacterManager.UpdateInfos();
                this._currentTick = 0;
            }

            this._currentTick++;
        }

        private void PartyRemovedEvent(PartyBase party)
        {
            RevolutionsManagers.PartyManager.RemoveInfo(party.Id);
        }

        private void MobilePartyDestroyed(MobileParty mobileParty, PartyBase party)
        {
            RevolutionsManagers.PartyManager.RemoveInfo(mobileParty.Party.Id);
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            RevolutionsManagers.KingdomManager.RemoveInfo(kingdom.StringId);
        }

        private void ClanDestroyedEvent(Clan clan)
        {
            RevolutionsManagers.ClanManager.RemoveInfo(clan.StringId);
        }
    }
}
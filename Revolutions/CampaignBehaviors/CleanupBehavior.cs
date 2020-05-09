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

            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.PartyRemovedEvent));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            RevolutionsManagers.FactionManager.CleanupDuplicatedInfos();
            RevolutionsManagers.KingdomManager.CleanupDuplicatedInfos();
            RevolutionsManagers.ClanManager.CleanupDuplicatedInfos();
            RevolutionsManagers.PartyManager.CleanupDuplicatedInfos();
            RevolutionsManagers.CharacterManager.CleanupDuplicatedInfos();
            RevolutionsManagers.SettlementManager.CleanupDuplicatedInfos();

            RevolutionsManagers.PartyManager.UpdateInfos();
        }

        private void TickEvent(float dt)
        {
            //Assuming, that we have 30 ticks per second, we update one of our data pieces once per second. So after 6 seconds all data was updated one time.
            switch (this._currentTick)
            {
                case RefreshAtTick:
                    RevolutionsManagers.FactionManager.UpdateInfos();
                    break;
                case RefreshAtTick + 30:
                    RevolutionsManagers.KingdomManager.UpdateInfos();
                    break;
                case RefreshAtTick + 60:
                    RevolutionsManagers.ClanManager.UpdateInfos();
                    break;
                case RefreshAtTick + 90:
                    RevolutionsManagers.SettlementManager.UpdateInfos();
                    break;
                case RefreshAtTick + 120:
                    RevolutionsManagers.CharacterManager.UpdateInfos();
                    this._currentTick = 0;
                    break;
                default:
                    this._currentTick++;
                    break;
            }
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
using System;
using System.Linq;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using ModLibrary.Settlements;
using Revolutions.Settlements;
using Revolutions.Revolutions;
using Revolutions.Factions;
using ModLibrary.Factions;
using TaleWorlds.CampaignSystem.Actions;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.SettlementEntered));
            CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeftEvent));
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                this.DataStorage.LoadData();
            }

            if (dataStore.IsSaving)
            {
                if (this.DataStorage.SaveId.IsEmpty())
                {
                    this.DataStorage.SaveId = Guid.NewGuid().ToString();
                }

                dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                this.DataStorage.SaveData();
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            if (this.DataStorage.SaveId.IsEmpty())
            {
                this.DataStorage.InitializeData();
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement.StringId);
            settlementInfo.UpdateOwner(newOwner.MapFaction);
        }

        private void SettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (mobileParty == null || !mobileParty.IsLordParty || mobileParty.LeaderHero == null)
            {
                return;
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement.StringId);

            var partyLeader = mobileParty.Leader.HeroObject;
            var clanLeader = mobileParty.Party.Owner.Clan.Leader;

            if (partyLeader.StringId == clanLeader.StringId && clanLeader.Clan.StringId == settlement.OwnerClan.StringId)
            {
                settlementInfo.IsOwnerInSettlement = true;
            }
        }

        private void OnSettlementLeftEvent(MobileParty mobileParty, Settlement settlement)
        {
            if (mobileParty == null || !mobileParty.IsLordParty || mobileParty.LeaderHero == null)
            {
                return;
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement.StringId);
            var partyLeader = mobileParty.Leader.HeroObject;
            var clanLeader = mobileParty.Party.Owner.Clan.Leader;

            if (partyLeader.StringId == clanLeader.StringId && partyLeader.Clan.StringId == settlement.OwnerClan.StringId)
            {
                settlementInfo.IsOwnerInSettlement = false;
            }
        }

        private void MapEventEnded(MapEvent mapEvent)
        {
            var revolutionParties = RevolutionsManagers.RevolutionManager.Revolutions.Select(revolution => RevolutionsManagers.RevolutionManager.GetParty(revolution)).ToList();
            var involvedRevolutionParty = mapEvent.InvolvedParties.Intersect(revolutionParties).FirstOrDefault();
            if (involvedRevolutionParty == null)
            {
                return;
            }

            var currentRevolution = RevolutionsManagers.RevolutionManager.GetRevolution(involvedRevolutionParty.Id);
            var currentSettlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(currentRevolution.SettlementId);
            var currentFactionInfoRevolutions = RevolutionsManagers.FactionManager.GetFactionInfo(currentSettlementInfoRevolutions.CurrentFactionId);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedRevolutionParty.Id) == null)
            {
                RevolutionsManagers.RevolutionManager.EndFailedRevolution(currentRevolution, currentSettlementInfoRevolutions, currentFactionInfoRevolutions);
            }
            else
            {
                RevolutionsManagers.RevolutionManager.EndSucceededRevoluton(currentRevolution, currentSettlementInfoRevolutions, currentFactionInfoRevolutions);
            }
        }
    }
}
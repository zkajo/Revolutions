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
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero,
                ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementChanged));
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

        private void OnSettlementChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            SettlementInfoRevolutions settlementInfo = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement.StringId);
            settlementInfo.UpdateOwner(newOwner.MapFaction);
        }

        private void SettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (mobileParty == null || !mobileParty.IsLordParty || mobileParty.LeaderHero == null)
            {
                return;
            }

            SettlementInfoRevolutions settlementInfo = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement.StringId);

            Hero partyLeader = mobileParty.Leader.HeroObject;
            Hero clanLeader = mobileParty.Party.Owner.Clan.Leader;

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

            SettlementInfoRevolutions settlementInfo = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement.StringId);

            Hero partyLeader = mobileParty.Leader.HeroObject;
            Hero clanLeader = mobileParty.Party.Owner.Clan.Leader;

            if (partyLeader.StringId == clanLeader.StringId && partyLeader.Clan.StringId == settlement.OwnerClan.StringId)
            {
                settlementInfo.IsOwnerInSettlement = false;
            }
        }

        private void MapEventEnded(MapEvent mapEvent)
        {
            List<PartyBase> revolutionParties = RevolutionManager.Instance.Revolutions.Select(revolution => RevolutionManager.Instance.GetParty(revolution)).ToList();
            PartyBase involvedRevolutionParty = mapEvent.InvolvedParties.Intersect(revolutionParties).FirstOrDefault();
            if (involvedRevolutionParty == null)
            {
                return;
            }

            Revolution currentRevolution = RevolutionManager.Instance.GetRevolution(involvedRevolutionParty.Id);
            SettlementInfoRevolutions currentSettlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(currentRevolution.SettlementId);
            FactionInfoRevolutions currentFactionInfoRevolutions = RevolutionsManagers.FactionManager.GetFactionInfo(currentSettlementInfoRevolutions.CurrentFactionId);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedRevolutionParty.Id) == null)
            {
                RevolutionManager.Instance.EndFailedRevolution(currentRevolution, currentSettlementInfoRevolutions, currentFactionInfoRevolutions);
            }
            else
            {
                RevolutionManager.Instance.EndSucceededRevoluton(currentRevolution, currentSettlementInfoRevolutions, currentFactionInfoRevolutions);
            }
        }
    }
}
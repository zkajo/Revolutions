using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using ModLibrary;
using Revolutions.Components.Settlements;
using Revolutions.Components.Revolutions;

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
            try
            {
                if (dataStore.IsLoading)
                {
                    this.DataStorage.InitializeData();

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
            catch (Exception exception)
            {
                var exceptionMessage = $"Revolutions: SyncData failed ({dataStore.IsLoading} | {dataStore.IsSaving} | {this.DataStorage.SaveId})! ";
                InformationManager.DisplayMessage(new InformationMessage(exceptionMessage + exception?.ToString(), ColorManager.Red));
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            this.DataStorage.InitializeData();
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.UpdateOwnerRevolution(newOwner.MapFaction);
        }

        private void SettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (mobileParty == null || !mobileParty.IsLordParty || mobileParty.LeaderHero == null)
            {
                return;
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);

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

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            var partyLeader = mobileParty.Leader.HeroObject;
            var clanLeader = mobileParty.Party.Owner.Clan.Leader;

            if (partyLeader.StringId == clanLeader.StringId && partyLeader.Clan.StringId == settlement.OwnerClan.StringId)
            {
                settlementInfo.IsOwnerInSettlement = false;
            }
        }

        private void MapEventEnded(MapEvent mapEvent)
        {
            var involvedParty = mapEvent.InvolvedParties.Intersect(RevolutionManager.Instance.GetParties()).FirstOrDefault();
            if (involvedParty == null)
            {
                return;
            }

            var currentRevolution = RevolutionsManagers.RevolutionManager.GetRevolutionByPartyId(involvedParty.Id);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedParty.Id) == null)
            {
                RevolutionsManagers.RevolutionManager.EndFailedRevolution(currentRevolution);
            }
            else
            {
                RevolutionsManagers.RevolutionManager.EndSucceededRevoluton(currentRevolution);
            }
        }
    }
}
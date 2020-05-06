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

            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));

            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));

            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
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
                    this.DataStorage.SaveId = Guid.NewGuid().ToString();
                    dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                    this.DataStorage.SaveData();
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: SyncData failed ({dataStore.IsLoading} | {dataStore.IsSaving} | {this.DataStorage.SaveId})!", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorManager.Red));
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            this.DataStorage.InitializeData();
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

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.UpdateOwnerRevolution(newOwner.MapFaction);
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            var kingdomInfo = RevolutionsManagers.KingdomManager.GetInfo(kingdom);
            if (kingdomInfo.UserMadeKingdom)
            {
                RevolutionsManagers.KingdomManager.ModifyKingdomList(kingdoms => kingdoms.Remove(kingdom));
            }
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = RevolutionsManagers.ClanManager.GetInfo(clan.StringId);

            if (!clanInfo.CanJoinOtherKingdoms && newKingdom.RulingClan.StringId != clan.StringId)
            {
                clan.ClanLeaveKingdom(false);
            }
        }
    }
}
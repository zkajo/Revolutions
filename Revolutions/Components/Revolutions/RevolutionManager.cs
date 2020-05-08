using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using ModLibrary;
using Revolutions.Components.Factions;
using TaleWorlds.TwoDimension;
using Helpers;
using System.Text;

namespace Revolutions.Components.Revolutions
{
    public class RevolutionManager
    {
        #region Singleton

        private RevolutionManager() { }

        static RevolutionManager()
        {
            Instance = new RevolutionManager();
        }

        public static RevolutionManager Instance { get; private set; }

        #endregion

        public HashSet<Revolution> Revolutions = new HashSet<Revolution>();

        public Revolution GetRevolutionByPartyId(string partyId)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.PartyId == partyId);
        }

        public Revolution GetRevolutionByParty(PartyBase party)
        {
            return this.GetRevolutionByPartyId(party.Id);
        }

        public Revolution GetRevolutionBySettlementId(string settlementId)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.SettlementId == settlementId);
        }

        public Revolution GetRevolutionBySettlement(Settlement settlement)
        {
            return this.GetRevolutionBySettlementId(settlement.StringId);
        }

        public List<Settlement> GetSettlements()
        {
            return this.Revolutions.Select(revolution => revolution.Settlement).ToList();
        }

        public List<PartyBase> GetParties()
        {
            return this.Revolutions.Select(revolution => revolution.Party).ToList();
        }

        public void IncreaseDailyLoyaltyForSettlement()
        {
            foreach (var settlementInfo in RevolutionsManagers.SettlementManager.Infos)
            {
                foreach (var mobileParty in settlementInfo.Settlement.Parties)
                {
                    if (mobileParty.IsLordParty && mobileParty.Party.Owner.Clan == settlementInfo.Settlement.OwnerClan)
                    {
                        settlementInfo.Settlement.Town.Loyalty += Settings.Instance.PlayerInTownLoyaltyIncrease;

                        if (settlementInfo.Settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
                        {
                            var textObject = new TextObject("{=PqkwszGz}Seeing you spend time at {SETTLEMENT}, your subjects feel more loyal to you.");
                            textObject.SetTextVariable("SETTLEMENT", settlementInfo.Settlement.Name.ToString());
                            InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                        }

                        break;
                    }
                }
            }
        }

        public void CheckRevolutionProgress()
        {
            foreach (var settlementInfo in RevolutionsManagers.SettlementManager.Infos)
            {
                var settlement = settlementInfo.Settlement;

                if (!settlement.IsTown)
                {
                    continue;
                }

                if (settlementInfo.LoyalFactionId == settlementInfo.CurrentFactionId)
                {
                    continue;
                }

                if (!settlementInfo.CurrentFactionInfoRevolutions.CanRevolt || settlementInfo.HasRebellionEvent)
                {
                    settlementInfo.RevolutionProgress = 0;
                    continue;
                }

                settlementInfo.RevolutionProgress += Settings.Instance.MinimumObedienceLoyalty - settlement.Town.Loyalty;

                if (settlementInfo.RevolutionProgress >= 100 && !settlement.IsUnderSiege)
                {
                    this.StartRebellionEvent(settlement);
                    continue;
                }

                if (settlementInfo.RevolutionProgress < 0)
                {
                    settlementInfo.RevolutionProgress = 0;
                }
            }
        }

        public void EndFailedRevolution(Revolution revolution)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolution.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Orange));

            revolution.SettlementInfoRevolutions.CurrentFactionInfoRevolutions.CityRevoltionFailed(revolution.Settlement);

            if (revolution.IsMinorFaction)
            {
                var mapFaction = revolution.Party.Owner.Clan.Kingdom.MapFaction;
                foreach (var faction in Campaign.Current.Factions.Where(go => go.IsAtWarWith(mapFaction)))
                {
                    if (revolution.Party.Owner.Clan.Kingdom.MapFaction.IsAtWarWith(faction))
                    {
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(faction, mapFaction);
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(mapFaction, faction);
                        MakePeaceAction.Apply(faction, mapFaction);
                    }
                }

                DestroyKingdomAction.Apply(revolution.Party.Owner.Clan.Kingdom);
                RevolutionsManagers.KingdomManager.ModifyKingdomList(list => list.Remove(revolution.Party.Owner.Clan.Kingdom));
                DestroyClanAction.Apply(revolution.Party.Owner.Clan);
            }

            DestroyPartyAction.Apply(revolution.SettlementInfoRevolutions.Garrision, revolution.Party.MobileParty);

            this.Revolutions.Remove(revolution);
        }

        public void EndSucceededRevoluton(Revolution revolution)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolution.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Orange));

            revolution.SettlementInfoRevolutions.CurrentFactionInfoRevolutions.CityRevoltionSucceeded(revolution.Settlement);

            if (Settings.Instance.EmpireLoyaltyMechanics && revolution.SettlementInfo.IsCurrentFactionOfImperialCulture && !revolution.SettlementInfoRevolutions.IsLoyalFactionOfImperialCulture)
            {
                revolution.Settlement.OwnerClan.AddRenown(-Settings.Instance.ImperialRenownLossOnWin);
            }

            if (Settings.Instance.AllowMinorFactions && revolution.IsMinorFaction)
            {
                if (revolution.Party.MobileParty.Ai.DoNotMakeNewDecisions)
                {
                    revolution.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
                }

                SetPartyAiAction.GetActionForPatrollingAroundSettlement(revolution.Party.MobileParty, revolution.Settlement);

                revolution.Party.LeaderHero.Clan.AddRenown(Settings.Instance.RenownGainOnWin);

                int amountOfEliteTroops = (Settings.Instance.BaseRevoltArmySize + (int)(revolution.Settlement.Prosperity * Settings.Instance.ArmyProsperityMulitplier)) / 2;
                revolution.Party.MobileParty.MemberRoster.Add(RevolutionsManagers.PartyManager.GenerateEliteTroopRoster(revolution.Party.LeaderHero, amountOfEliteTroops));
            }
            else
            {
                //Hero newOwner = revolution.Party.MobileParty.LeaderHero ?? revolution.SettlementInfoRevolutions.LoyalFaction.Leader;
                //revolution.Party.MobileParty.RemoveParty();
                //ChangeOwnerOfSettlementAction.ApplyBySiege(newOwner, newOwner, revolution.Settlement);
            }

            this.Revolutions.Remove(revolution);
        }

        public void StartRebellionEvent(Settlement settlement)
        {
            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Orange));

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);

            Hero hero;

            if (atWarWithLoyalFaction)
            {
                hero = RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                hero = RevolutionsManagers.CharacterManager.CreateRandomLeader(settlement.OwnerClan, settlementInfo);
                RevolutionsManagers.CharacterManager.GetInfo(hero.CharacterObject).IsRevoltKingdomLeader = true;
                var clan = RevolutionsManagers.ClanManager.CreateClan(hero, hero.Name, hero.Name);
                var kingdom = RevolutionsManagers.KingdomManager.CreateKingdom(hero, settlement, new TextObject($"Kingdom of {settlement.Name}"), new TextObject($"Kingdom of {settlement.Name}"));

                RevolutionsManagers.ClanManager.GetInfo(hero.Clan.StringId).CanJoinOtherKingdoms = false;
            }

            var mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(hero, settlement.GatePosition, settlement, !atWarWithLoyalFaction, true);

            int amountOfBasicTroops = Settings.Instance.BaseRevoltArmySize + (int)(settlement.Prosperity * Settings.Instance.ArmyProsperityMulitplier);
            mobileParty.MemberRoster.Add(RevolutionsManagers.PartyManager.GenerateBasicTroopRoster(hero, amountOfBasicTroops));

            if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
            {
                foreach (var troopRosterElement in settlement.MilitaParty.MemberRoster)
                {
                    mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number, false);
                }

                settlement.MilitaParty.RemoveParty();
            }

            if (!atWarWithLoyalFaction)
            {
                mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);
            }

            var revolution = new Revolution(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction);
            this.Revolutions.Add(revolution);

            settlementInfo.HasRebellionEvent = true;

            FactionManager.DeclareWar(hero.MapFaction, settlement.MapFaction);
            Campaign.Current.FactionManager.RegisterCampaignWar(hero.MapFaction, settlement.MapFaction);
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, settlement.OwnerClan.Leader, -20, false);
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, settlement.OwnerClan.Kingdom.Leader, -20, false);
            mobileParty.Ai.SetDoNotMakeNewDecisions(true);
            SetPartyAiAction.GetActionForBesiegingSettlement(mobileParty, settlement);
        }
    }
}
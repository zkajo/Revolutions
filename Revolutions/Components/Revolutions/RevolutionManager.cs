using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using ModLibrary;
using Revolutions.Components.Factions;

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
                DestroyKingdomAction.Apply(revolution.Party.Owner.Clan.Kingdom);
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
                ChangeOwnerOfSettlementAction.ApplyByDefault(revolution.Party.Owner, revolution.Settlement);
                MakePeaceAction.Apply(revolution.Party.Owner.Clan.Kingdom, revolution.Settlement.MapFaction);

                revolution.Party.MobileParty.ResetAiBehaviorObject();
                revolution.Party.MobileParty.EnableAi();
                revolution.Party.MobileParty.SetMovePatrolAroundSettlement(revolution.Settlement);

                revolution.Party.LeaderHero.Clan.AddRenown(Settings.Instance.RenownGainOnWin);
            }
            else
            {
                Hero newOwner = revolution.Party.MobileParty.LeaderHero ?? revolution.Party.MapFaction.Leader;

                ChangeOwnerOfSettlementAction.ApplyByDefault(newOwner, revolution.Settlement);
                revolution.Party.MobileParty.RemoveParty();
            }

            revolution.PartyInfoRevolutions.CantStarve = false;

            this.Revolutions.Remove(revolution);
        }

        public void StartRebellionEvent(Settlement settlement)
        {
            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Orange));

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);
            var isMinorFaction = false;

            Hero hero;

            if (atWarWithLoyalFaction)
            {
                hero = RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                hero = RevolutionsManagers.CharacterManager.CreateLord(settlement);

                var clan = RevolutionsManagers.ClanManager.CreateClan(hero.Name, hero.Name, hero.Culture, hero, settlement.MapFaction.Color, settlement.MapFaction.Color2, settlement.MapFaction.LabelColor, settlement.GatePosition);
                hero.Clan = clan;

                var clanInfo = RevolutionsManagers.ClanManager.GetInfo(clan.StringId);
                clanInfo.CanJoinOtherKingdoms = false;

                var kingdom = this.CreateRebelKingdom(clan, settlement);
                ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, true);
                kingdom.RulingClan = clan;

                DeclareWarAction.Apply(kingdom, settlement.MapFaction);

                isMinorFaction = true;
            }

            var partyTemplate = settlement.Culture.RebelsPartyTemplate;
            partyTemplate.IncrementNumberOfCreated();
            var partyId = string.Concat("rebels_of_", settlement.Culture.StringId, "_", partyTemplate.NumberOfCreated);
            var partyName = new TextObject("{=q2t1Ss8d}Revolutionary Mob");
            var mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(partyName, settlement.GatePosition, partyTemplate, hero, !atWarWithLoyalFaction, true);

            if (isMinorFaction)
            {
                var commanderSkill = MBMath.ClampInt(3, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
                mobileParty.Party.Owner.SetTraitLevel(DefaultTraits.Commander, commanderSkill);
                var siegecraftSkill = MBMath.ClampInt(3, DefaultTraits.Siegecraft.MinValue, DefaultTraits.Siegecraft.MaxValue);
                mobileParty.Party.Owner.SetTraitLevel(DefaultTraits.Siegecraft, siegecraftSkill);

                mobileParty.Party.Owner.ChangeState(Hero.CharacterStates.Active);
            }

            var numberOfRebels = Settings.Instance.BaseRevoltArmySize + (int)(settlement.Prosperity * Settings.Instance.ArmyProsperityMulitplier);
            mobileParty.AddElementToMemberRoster(mobileParty.MemberRoster.GetCharacterAtIndex(0), numberOfRebels, false);

            if (isMinorFaction)
            {
                mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);
            }

            if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
            {
                foreach (var troopRosterElement in settlement.MilitaParty.MemberRoster)
                {
                    mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number, false);
                }

                settlement.MilitaParty.RemoveParty();
            }

            mobileParty.IsLordParty = true;
            mobileParty.Party.Visuals.SetMapIconAsDirty();

            var revolution = new Revolution(mobileParty.Party.Id, settlement, isMinorFaction);
            this.Revolutions.Add(revolution);

            if (settlementInfo.Garrision == null)
            {
                this.EndSucceededRevoluton(revolution);
            }
            else
            {
                revolution.PartyInfoRevolutions.CantStarve = true;

                mobileParty.EnableAi();
                mobileParty.Ai.SetAIState(AIState.BesiegingCenter, revolution.SettlementInfoRevolutions.Garrision);
                mobileParty.SetMoveBesiegeSettlement(revolution.Settlement);
                var siegeEvent = Campaign.Current.SiegeEventManager.StartSiegeEvent(revolution.Settlement, mobileParty);
                var mapEvent = Campaign.Current.MapEventManager.StartSiegeMapEvent(mobileParty.Party, revolution.SettlementInfoRevolutions.Garrision);
                mapEvent.SimulateBattleSetup();

                settlementInfo.HasRebellionEvent = true;
            }
        }

        private Kingdom CreateRebelKingdom(Clan ownerClan, Settlement settlement)
        {
            var kingdomName = settlement.Name.ToString();
            var kingdom = RevolutionsManagers.KingdomManager.CreateKingdom(ownerClan, kingdomName, kingdomName);

            return kingdom;
        }
    }
}
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

        public Revolution GetRevolutionBySettlementId(uint settlementId)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.SettlementId == settlementId);
        }

        public Revolution GetRevolutionBySettlement(Settlement settlement)
        {
            return this.GetRevolutionBySettlementId(settlement.Id.InternalValue);
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

                        if (settlementInfo.Settlement.OwnerClan.Id.InternalValue == Hero.MainHero.Clan.Id.InternalValue)
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
                RevolutionsManagers.KingdomManager.ModifyKingdomList(list => list.Remove(revolution.Party.Owner.Clan.Kingdom));
                foreach (var faction in Campaign.Current.Factions.Where(n => n.IsAtWarWith(mapFaction)))
                {
                    if (revolution.Party.Owner.Clan.Kingdom.MapFaction.IsAtWarWith(faction))
                    {
                        MakePeaceAction.Apply(faction, mapFaction);
                    }
                }

                RevolutionsManagers.KingdomManager.ModifyKingdomList(list => list.Remove(revolution.Party.Owner.Clan.Kingdom));
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
                MakePeaceAction.Apply(revolution.Party.Owner.Clan.Kingdom.MapFaction, revolution.Settlement.MapFaction);

                revolution.Party.MobileParty.ResetAiBehaviorObject();
                revolution.Party.MobileParty.EnableAi();
                SetPartyAiAction.GetActionForPatrollingAroundSettlement(revolution.Party.MobileParty, revolution.Settlement);

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

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.Id.InternalValue);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction?.IsAtWarWith(settlementInfo.LoyalFaction);
            var isMinorFaction = false;

            Hero hero;

            if (atWarWithLoyalFaction.Value == true)
            {
                hero = RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                hero = RevolutionsManagers.CharacterManager.CreateLord(settlement);

                var clan = RevolutionsManagers.ClanManager.CreateClan(hero.Name, hero.Name, hero.Culture, hero, settlement.MapFaction.Color, settlement.MapFaction.Color2, settlement.MapFaction.LabelColor, settlement.GatePosition);
                hero.Clan = clan;

                var clanInfo = RevolutionsManagers.ClanManager.GetInfo(clan.Id.InternalValue);
                clanInfo.CanJoinOtherKingdoms = false;

                var kingdomName = settlement.Name.ToString();
                var kingdom = RevolutionsManagers.KingdomManager.CreateKingdom(clan, kingdomName, kingdomName);
                ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, true);
                kingdom.RulingClan = clan;

                DeclareWarAction.Apply(clan, settlement.MapFaction);
                DeclareWarAction.Apply(kingdom, settlement.MapFaction);

                isMinorFaction = true;
            }

            var partyTemplate = settlement.Culture.RebelsPartyTemplate;
            var partyName = new TextObject("{=q2t1Ss8d}Revolutionary Mob");
            var mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(partyName, settlement.GatePosition, partyTemplate, hero, atWarWithLoyalFaction == false, true);

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

            if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
            {
                foreach (var troopRosterElement in settlement.MilitaParty.MemberRoster)
                {
                    mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number, false);
                }

                settlement.MilitaParty.RemoveParty();
            }

            if (isMinorFaction)
            {
                mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);
            }

            var revolution = new Revolution(mobileParty.Party.Id, settlement, isMinorFaction);
            this.Revolutions.Add(revolution);

            if (settlementInfo.Garrision == null)
            {
                this.EndSucceededRevoluton(revolution);
            }
            else
            {
                revolution.PartyInfoRevolutions.CantStarve = true;
                Campaign.Current.MapEventManager.StartBattleMapEvent(mobileParty.Party, settlementInfo.Garrision);
                settlementInfo.HasRebellionEvent = true;
            }
        }
    }
}
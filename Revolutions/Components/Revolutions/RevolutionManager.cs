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

        public List<Revolution> Revolutions = new List<Revolution>();

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
            revolution.SettlementInfoRevolutions.CurrentFactionInfoRevolutions.CityRevoltionFailed(revolution.Settlement);

            if (revolution.IsMinorFaction)
            {
                DestroyClanAction.Apply(revolution.Party.Owner.Clan);
            }

            DestroyPartyAction.Apply(revolution.SettlementInfoRevolutions.Garrision, revolution.Party.MobileParty);

            this.Revolutions.Remove(revolution);
        }

        public void EndSucceededRevoluton(Revolution revolution)
        {
            revolution.SettlementInfoRevolutions.CurrentFactionInfoRevolutions.CityRevoltionSucceeded(revolution.Settlement);

            if ( Settings.Instance.EmpireLoyaltyMechanics && revolution.SettlementInfo.IsCurrentFactionOfImperialCulture && !revolution.SettlementInfoRevolutions.IsLoyalFactionOfImperialCulture)
            {
                revolution.Settlement.OwnerClan.AddRenown(-Settings.Instance.ImperialRenownLossOnWin);
            }

            if (Settings.Instance.AllowMinorFactions && revolution.IsMinorFaction)
            {
                ChangeOwnerOfSettlementAction.ApplyByDefault(revolution.Party.Owner, revolution.Settlement);
                revolution.Party.MobileParty.Ai.SetAIState(AIState.PatrollingAroundLocation);
                revolution.Party.LeaderHero.Clan.AddRenown(Settings.Instance.RenownGainOnWin);
            }
            else
            {
                ChangeOwnerOfSettlementAction.ApplyByDefault(revolution.Party.MobileParty.LeaderHero, revolution.Settlement);
                revolution.Party.MobileParty.RemoveParty();
            }

            this.Revolutions.Remove(revolution);
        }

        public void StartRebellionEvent(Settlement settlement)
        {
            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);
            var isMinorFaction = false;

            Hero hero;

            if (atWarWithLoyalFaction)
            {
                hero = RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                hero = HeroCreator.CreateSpecialHero(ModLibraryManagers.CharacterManager.CreateLordCharacter(settlement.Culture), settlement, null, null, -1);
                var clan = ModLibraryManagers.ClanManager.CreateClan(hero.Name, hero.Name, hero.Culture, hero, settlement.MapFaction.Color, settlement.MapFaction.Color2, settlement.MapFaction.LabelColor, settlement.GatePosition);
                clan.InitializeClan(clan.Name, clan.Name, clan.Culture, Banner.CreateRandomBanner(MBRandom.RandomInt(0, 1000000)));
                hero.Clan = clan;

                var clanInfo = ModLibraryManagers.ClanManager.GetInfoById(clan.StringId);
                clanInfo.CanJoinOtherKingdoms = false;
                isMinorFaction = true;
                DeclareWarAction.Apply(clan, settlement.MapFaction);
                CreateRebelKingdom(clan, settlement.Name.ToString().ToLower() + "_kingdom", settlement.MapFaction, settlement);
            }

            var rebelsPartyTemplate = settlement.Culture.RebelsPartyTemplate;
            rebelsPartyTemplate.IncrementNumberOfCreated();

            var id = string.Concat("rebels_of_", settlement.Culture.StringId, "_", rebelsPartyTemplate.NumberOfCreated);
            var name = new TextObject("{=q2t1Ss8d}Revolutionary Mob");
            var mobileParty = ModLibraryManagers.PartyManager.CreateMobileParty(id, name, settlement.GatePosition, rebelsPartyTemplate, hero, !atWarWithLoyalFaction, true);

            if (isMinorFaction)
            {
                var value = MBMath.ClampInt(1, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
                mobileParty.Party.Owner.SetTraitLevel(DefaultTraits.Commander, value);
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

            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.AddQuickInformation(information, 0, null, "");

            var revolution = new Revolution(mobileParty.Party.Id, settlement, isMinorFaction);
            this.Revolutions.Add(revolution);

            if (settlementInfo.Garrision == null)
            {
                this.EndSucceededRevoluton(revolution);
            }
            else
            {
                Campaign.Current.MapEventManager.StartBattleMapEvent(mobileParty.Party, settlementInfo.Garrision);
                settlementInfo.HasRebellionEvent = true;
            }
        }
        
        private Kingdom CreateRebelKingdom(Clan ownerClan, string stringId, IFaction warOnFaction, Settlement settlement)
        {
            string kingdomId = stringId;
            var kingdom = Kingdom.All.SingleOrDefault(x => x.StringId == kingdomId);

            if (kingdom == null)
            {
                string kingdomName = settlement.Name.ToString();
                kingdom = RevolutionsManagers.KingdomManager.CreateKingdom(ownerClan, kingdomId,kingdomName , kingdomName);
            }
            else
            {
                if (kingdom.IsDeactivated)
                {
                    kingdom.ReactivateKingdom();
                }
            }
            
            //For whatever reason this city's kingdom is not kingdom holder.
            //Therefore new clan is now kingdom owner.
            kingdom.RulingClan = ownerClan;
            
            if (!kingdom.IsAtWarWith(warOnFaction))
            {
                DeclareWarAction.Apply(kingdom, warOnFaction);
            }
            
            return kingdom;
        }
    }
}
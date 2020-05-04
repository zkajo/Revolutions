using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using ModLibrary;
using ModLibrary.Characters;
using ModLibrary.Settlements;
using Revolutions.Components.Clans;
using Revolutions.Components.Factions;
using Revolutions.Factions;
using Revolutions.Settlements;

namespace Revolutions.Revolutions
{
    public class RevolutionManager
    {
        #region Singleton

        private RevolutionManager() { }

        static RevolutionManager()
        {
            RevolutionManager.Instance = new RevolutionManager();
        }

        public static RevolutionManager Instance { get; private set; }

        #endregion

        public List<Revolution> Revolutions = new List<Revolution>();

        public Revolution GetRevolution(string partyId)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.PartyId == partyId);
        }

        public Revolution GetRevolution(Settlement settlement)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.SettlementId == settlement.StringId);
        }

        public PartyBase GetParty(Revolution revolution)
        {
            return ModLibraryManagers.PartyManager.GetParty(revolution.PartyId);
        }

        public void IncreaseDailyLoyaltyForSettlement()
        {
            foreach (var settlementInfoRevolutions in RevolutionsManagers.SettlementManager.SettlementInfos)
            {
                var settlement = settlementInfoRevolutions.Settlement;

                foreach (var mobileParty in settlement.Parties)
                {
                    if (mobileParty.IsLordParty && mobileParty.Party.Owner.Clan == settlement.OwnerClan)
                    {
                        settlement.Town.Loyalty += Settings.Instance.PlayerInTownLoyaltyIncrease;

                        if (settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
                        {
                            var textObject = new TextObject("{=PqkwszGz}Seeing you spend time at {SETTLEMENT}, your subjects feel more loyal to you.");
                            textObject.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
                            InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                        }

                        break;
                    }
                }
            }
        }

        public void CheckRevolutionProgress()
        {
            foreach (var settlementInfoRevolutions in RevolutionsManagers.SettlementManager.SettlementInfos)
            {
                if (!settlementInfoRevolutions.Settlement.IsTown)
                {
                    continue;
                }

                var settlement = settlementInfoRevolutions.Settlement;
                var factionInfoRevolutions = RevolutionsManagers.FactionManager.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

                if (settlementInfoRevolutions.LoyalFactionId == settlementInfoRevolutions.CurrentFactionId)
                {
                    continue;
                }

                if (!factionInfoRevolutions.CanRevolt || settlementInfoRevolutions.HasRebellionEvent)
                {
                    settlementInfoRevolutions.RevolutionProgress = 0;
                    continue;
                }

                settlementInfoRevolutions.RevolutionProgress += Settings.Instance.MinimumObedienceLoyalty - settlement.Town.Loyalty;

                if (settlementInfoRevolutions.RevolutionProgress >= 100 && !settlement.IsUnderSiege)
                {
                    this.StartRebellionEvent(settlement);
                    continue;
                }

                if (settlementInfoRevolutions.RevolutionProgress < 0)
                {
                    settlementInfoRevolutions.RevolutionProgress = 0;
                }
            }
        }

        public void EndFailedRevolution(Revolution revolution, SettlementInfoRevolutions settlementInfoRevolutions, FactionInfoRevolutions currentFactionInfo)
        {
            currentFactionInfo.CityRevoltionFailed(settlementInfoRevolutions.Settlement);
            DestroyClanAction.Apply(revolution.Party.Owner.Clan);
            DestroyPartyAction.Apply(revolution.SettlementInfoRevolutions.GetGarrison(), revolution.Party.MobileParty);

            settlementInfoRevolutions.HasRebellionEvent = false;
            this.Revolutions.Remove(revolution);
        }

        public void EndSucceededRevoluton(Revolution revolution, SettlementInfoRevolutions settlementInfoRevolutions, FactionInfoRevolutions currentFactionInfo)
        {
            var currentSettlement = SettlementManager<SettlementInfo>.Instance.GetSettlement(settlementInfoRevolutions.SettlementId);

            currentFactionInfo.CityRevoltionSucceeded(currentSettlement);

            if (Settings.Instance.AllowMinorFactions)
            {
                ChangeOwnerOfSettlementAction.ApplyByDefault(revolution.Party.Owner, currentSettlement);
                revolution.Party.MobileParty.Ai.SetAIState(AIState.PatrollingAroundLocation);
            }
            else
            {
                ChangeOwnerOfSettlementAction.ApplyByDefault(RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfoRevolutions.LoyalFaction).HeroObject, currentSettlement);
                revolution.Party.MobileParty.RemoveParty();
            }

            settlementInfoRevolutions.HasRebellionEvent = false;
            this.Revolutions.Remove(revolution);
        }

        public void StartRebellionEvent(Settlement settlement)
        {
            var settlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement);
            var currentFaction = RevolutionsManagers.FactionManager.GetFactionInfo(settlement.MapFaction);
            bool atWarWithLoyalFaction = currentFaction.Faction.IsAtWarWith(settlementInfoRevolutions.LoyalFaction);

            Hero hero;
            Clan clan;

            if (atWarWithLoyalFaction)
            {
                hero = RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfoRevolutions.LoyalFaction).HeroObject;
                clan = hero.Clan;
            }
            else
            {
                hero = HeroCreator.CreateSpecialHero(CharacterManager.Instance.CreateLordCharacter(settlement.Culture), settlement, null, null, -1);
                clan = RevolutionsManagers.ClanManager.CreateClan(hero.Name, hero.Name, hero.Culture, hero, settlement.MapFaction.Color, settlement.MapFaction.Color2, settlement.MapFaction.LabelColor, settlement.GatePosition);
                clan.InitializeClan(clan.Name, clan.Name, clan.Culture, Banner.CreateRandomBanner(MBRandom.RandomInt(0, 1000000)));

                ClanInfoRevolutions clanInfo = RevolutionsManagers.ClanManager.GetClanInfo(clan);
                clanInfo.CanJoinOtherKingdoms = false;

                DeclareWarAction.Apply(clan, settlement.MapFaction);
            }

            PartyTemplateObject rebelsPartyTemplate = settlement.Culture.RebelsPartyTemplate;
            rebelsPartyTemplate.IncrementNumberOfCreated();

            string id = string.Concat("rebels_of_", settlement.Culture.StringId, "_", rebelsPartyTemplate.NumberOfCreated);
            TextObject name = new TextObject("{=q2t1Ss8d}Revolutionary Mob");
            MobileParty mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(id, name, settlement.GatePosition, rebelsPartyTemplate, hero, !atWarWithLoyalFaction, true);

            mobileParty.Party.Owner.Clan = clan;

            if (!atWarWithLoyalFaction)
            {
                int value = MBMath.ClampInt(1, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
                mobileParty.Party.Owner.SetTraitLevel(DefaultTraits.Commander, value);
                mobileParty.Party.Owner.ChangeState(Hero.CharacterStates.Active);
            }

            mobileParty.AddElementToMemberRoster(mobileParty.MemberRoster.GetCharacterAtIndex(0), (int)((double)settlement.Prosperity * 0.1), false);

            if (!atWarWithLoyalFaction)
            {
                mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);
            }

            if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
            {
                foreach (TroopRosterElement troopRosterElement in settlement.MilitaParty.MemberRoster)
                {
                    mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number, false);
                }
                settlement.MilitaParty.RemoveParty();
            }

            mobileParty.IsLordParty = true;
            mobileParty.Party.Visuals.SetMapIconAsDirty();

            TextObject information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.AddQuickInformation(information, 0, null, "");

            Revolution revolution = new Revolution(mobileParty.Party.Id, settlement);
            this.Revolutions.Add(revolution);

            PartyBase garrison = settlementInfoRevolutions.GetGarrison();

            if (garrison == null)
            {
                this.EndSucceededRevoluton(revolution, settlementInfoRevolutions, currentFaction);
            }
            else
            {
                Campaign.Current.MapEventManager.StartBattleMapEvent(mobileParty.Party, garrison);
                settlementInfoRevolutions.HasRebellionEvent = true;
            }
        }
    }
}
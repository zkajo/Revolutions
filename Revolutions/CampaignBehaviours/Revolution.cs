using System;
using System.Collections.Generic;
using System.Linq;
using Revolutions.Screens;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Revolutions.CampaignBehaviours
{
    public class Revolution : CampaignBehaviorBase
    {
        private const int PlayerInTownLoyaltyIncrease = 5;
        private const int MinimumObedianceLoyalty = 25;

        public List<SettlementInfo> SettlementInformation = new List<SettlementInfo>();
        public List<FactionInfo> FactionInformation = new List<FactionInfo>();
        public List<Tuple<PartyBase, SettlementInfo>> Revolutionaries = new List<Tuple<PartyBase, SettlementInfo>>();

        public Revolution()
        {

        }

        #region Data Getters
        public SettlementInfo GetSettlementInformation(Settlement settlement)
        {
            foreach (var settlementInfo in this.SettlementInformation)
            {
                if (settlementInfo.Settlement.StringId == settlement.StringId)
                {
                    return settlementInfo;
                }
            }

            SettlementInfo missingSettlement = new SettlementInfo(settlement);
            this.SettlementInformation.Add(missingSettlement);

            return missingSettlement;
        }

        public FactionInfo GetFactionInformation(IFaction faction)
        {
            foreach (var factioninfo in this.FactionInformation)
            {
                if (factioninfo.StringID == faction.StringId)
                {
                    return factioninfo;
                }
            }

            FactionInfo missingInformation = new FactionInfo(faction);
            this.FactionInformation.Add(missingInformation);

            return missingInformation;
        }
        #endregion

        #region Base Functionality

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailySettlementTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));

            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_SettlementInformation", ref this.SettlementInformation);
            dataStore.SyncData("_FactionInformation", ref this.FactionInformation);
            dataStore.SyncData("_Revolutionaries", ref this.Revolutionaries);
        }

        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            if (this.SettlementInformation.Count < 1)
            {
                this.RegisterSettlements();
            }

            if (this.FactionInformation.Count < 1)
            {
                this.RegisterFactions();
            }

            this.CreateLoyaltyMenu(obj);
        }

        private void RegisterSettlements()
        {
            foreach (var settlement in Settlement.All)
            {
                SettlementInfo settInf = new SettlementInfo(settlement);
                this.SettlementInformation.Add(settInf);
            }
        }

        private void RegisterFactions()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                FactionInfo fi = new FactionInfo(faction);
                this.FactionInformation.Add(fi);
            }
        }

        private void CreateLoyaltyMenu(CampaignGameStarter obj)
        {
            TextObject menuName = GameTexts.FindText("str_GM_TownLoyalty");
            obj.AddGameMenuOption("town", "town_enter_entr_option", menuName.ToString(), (MenuCallbackArgs args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }, (MenuCallbackArgs args) =>
            {
                SettlementInfo setinf = this.GetSettlementInformation(Settlement.CurrentSettlement);
                FactionInfo factinfo = this.GetFactionInformation(Settlement.CurrentSettlement.MapFaction);
                ScreenManager.PushScreen(new TownRevolutionScreen(setinf, factinfo));
            }, false, 4);
        }

        #endregion

        #region Events

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            for (int i = 0; i < this.FactionInformation.Count; i++)
            {
                if (kingdom.StringId == this.FactionInformation[i].StringID)
                {
                    this.FactionInformation.RemoveAt(i);
                    return;
                }
            }
        }

        private void ClanDestroyedEvent(Clan clan)
        {
            for (int i = 0; i < this.FactionInformation.Count; i++)
            {
                if (clan.StringId == this.FactionInformation[i].StringID)
                {
                    this.FactionInformation.RemoveAt(i);
                    return;
                }
            }
        }

        private void DailyTickEvent()
        {
            foreach (var faction in this.FactionInformation)
            {
                faction.UpdateFactionInfo();
            }

            foreach (var settlement in this.SettlementInformation)
            {
                settlement.UpdateOwnership();
            }
        }

        private void DailySettlementTick(Settlement settlement)
        {
            this.DailyTownEvent(settlement);
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool bl, Hero hero1, Hero hero2, Hero hero3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            SettlementInfo settlementInfo = this.GetSettlementInformation(settlement);

            if (settlementInfo != null)
            {
                settlementInfo.ResetOwnership();
            }
        }

        private void OnMapEventEnded(MapEvent mapEvent)
        {
            this.RevoltMapEventEnd(mapEvent);
        }

        private void RevoltMapEventEnd(MapEvent mapEvent)
        {
            PartyBase revs = null;
            SettlementInfo currentInfo = null;
            Settlement settlement = null;

            foreach (Tuple<PartyBase, SettlementInfo> pair in this.Revolutionaries)
            {
                if (mapEvent.InvolvedParties.Contains(pair.Item1))
                {
                    revs = pair.Item1;
                    currentInfo = pair.Item2;
                    settlement = currentInfo.Settlement;
                    break;
                }
            }

            if (revs == null)
            {
                return;
            }

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            var loserSide = winnerSide.MissionSide.GetOppositeSide();

            bool revVictory = false;
            foreach (var party in winnerSide.PartiesOnThisSide)
            {
                if (party.MobileParty.Id == revs.MobileParty.Id)
                {
                    revVictory = true;
                    break;
                }
            }

            if (!revVictory)
            {
                this.GetFactionInformation(settlement.MapFaction).CityRevoltedFailure(settlement);
                this.RemoveRevolutionaryPartyFromList(revs);
                return;
            }

            Hero selectedHero = null;
            selectedHero = this.GetNobleWithLeastFiefs(revs.Owner.MapFaction);
            this.RemoveRevolutionaryPartyFromList(revs);

            if (revs.Owner.MapFaction.StringId == currentInfo.OriginalFaction.StringId)
            {
                revs.MobileParty.RemoveParty();
                ChangeOwnerOfSettlementAction.ApplyByDefault(selectedHero, currentInfo.Settlement);
            }
            else
            {
                revs.MobileParty.IsLordParty = true;
                revs.MobileParty.Ai.EnableAi();
                revs.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
                revs.MobileParty.Name = revs.Owner.Name;
                Clan ownerClan = revs.Owner.Clan;
                ownerClan.AddParty(revs);

                if (!ModOptions.OptionsData.AllowMinorFactions)
                {
                    selectedHero = this.GetNobleWithLeastFiefs(currentInfo.OriginalFaction.MapFaction);
                    revs.MobileParty.RemoveParty();
                    DestroyKingdomAction.Apply(ownerClan.Kingdom);
                    Common.Instance.ModifyKingdomList(kingdoms => kingdoms.Remove(ownerClan.Kingdom));
                }
                else
                {
                    if (!selectedHero.Clan.IsKingdomFaction)
                    {
                        revs.MobileParty.RemoveParty();
                    }
                }

                ChangeOwnerOfSettlementAction.ApplyByDefault(selectedHero, currentInfo.Settlement);
            }

            this.GetFactionInformation(settlement.MapFaction).CityRevoltedSuccess(settlement);
            settlement.AddGarrisonParty(true);
        }

        private void DailyTownEvent(Settlement settlement)
        {
            if (!settlement.IsTown)
            {
                return;
            }

            this.IncreaseDailyLoyaltyForPlayerSettlement(settlement);
            this.CheckRevoltProgress(settlement);
        }

        #endregion

        private void RemoveRevolutionaryPartyFromList(PartyBase revolutionaryParty)
        {
            for (int i = 0; i < this.Revolutionaries.Count; i++)
            {
                if (this.Revolutionaries[i].Item1.Id == revolutionaryParty.Id)
                {
                    _ = this.Revolutionaries[i].Item1;
                    this.Revolutionaries.RemoveAt(i);
                    break;
                }
            }
        }

        private void CheckRevoltProgress(Settlement settlement)
        {
            SettlementInfo info = this.GetSettlementInformation(settlement);

            if (info.OriginalFaction.Name == settlement.MapFaction.Name)
            {
                return;
            }

            if (!this.GetFactionInformation(info.CurrentFaction).RevoltCanHappen() || this.OwnedByRevoltKingdom(info))
            {
                info.RevoltProgress = 0;
                return;
            }

            info.RevoltProgress += (MinimumObedianceLoyalty - settlement.Town.Loyalty);

            if (info.RevoltProgress >= 100 && !info.Settlement.IsUnderSiege)
            {
                this.RevoltLogic(info, settlement);
            }

            if (info.RevoltProgress < 0)
            {
                info.RevoltProgress = 0;
            }
        }

        private bool OwnedByRevoltKingdom(SettlementInfo info)
        {
            string kingdomSettlementId = this.GetClanKingdomId(info.Settlement);

            if (info.CurrentFaction.StringId == kingdomSettlementId)
            {
                return true;
            }

            return false;
        }

        private Hero GetNobleWithLeastFiefs(IFaction faction)
        {
            Clan chosenClan = null;
            int leastSettlements = 100;

            foreach (var noble in faction.Nobles)
            {
                int currentSettlements = noble.Clan.Settlements.Count();
                if (currentSettlements >= leastSettlements)
                {
                    continue;
                }

                leastSettlements = currentSettlements;
                chosenClan = noble.Clan;
            }

            return chosenClan != null ? chosenClan.Nobles.GetRandomElement() : faction.Leader;
        }

        private void IncreaseDailyLoyaltyForPlayerSettlement(Settlement settlement)
        {
            if (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.StringId != settlement.StringId)
            {
                return;
            }

            if (settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
            {
                TextObject textObject = GameTexts.FindText("str_GM_LoyaltyIncrease");
                textObject.SetTextVariable("SETTLEMENT", settlement.Name.ToString());

                InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                settlement.Town.Loyalty += PlayerInTownLoyaltyIncrease;
            }
        }

        private void RevoltLogic(SettlementInfo info, Settlement settlement)
        {
            TextObject revoltNotification = GameTexts.FindText("str_GM_RevoltNotification");
            revoltNotification.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(revoltNotification.ToString()));

            Hero selectedHero = null;
            TextObject revolutionaryMob = GameTexts.FindText("str_GM_RevolutionaryMob");
            MobileParty mob = MobileParty.Create(revolutionaryMob.ToString());
            TroopRoster roster = new TroopRoster();

            TroopRoster infantry = new TroopRoster();
            infantry.FillMembersOfRoster(300, settlement.Culture.MeleeMilitiaTroop);
            roster.Add(infantry);

            TroopRoster archers = new TroopRoster();
            archers.FillMembersOfRoster(200, settlement.Culture.RangedMilitiaTroop);
            roster.Add(archers);

            TroopRoster prisonRoster = new TroopRoster
            {
                IsPrisonRoster = true
            };

            if (info.CurrentFaction.IsAtWarWith(info.OriginalFaction))
            {
                Clan chosenClan = null;
                int leastSettlements = 100;
                foreach (var noble in info.OriginalFaction.Nobles)
                {
                    int currentSettlements = noble.Clan.Settlements.Count();
                    if (currentSettlements >= leastSettlements)
                    {
                        continue;
                    }

                    leastSettlements = currentSettlements;
                    chosenClan = noble.Clan;
                }

                selectedHero = chosenClan != null ? chosenClan.Nobles.GetRandomElement() : info.OriginalFaction.Leader;
            }
            else
            {
                var clan = this.CreateRebellionClan(info);
                clan.AddRenown(500);
                DeclareWarAction.Apply(clan, info.CurrentFaction);
                selectedHero = clan.Leader;
                mob.IsLordParty = true;
            }

            mob.ChangePartyLeader(selectedHero.CharacterObject);
            mob.Party.Owner = selectedHero;

            if (!info.CurrentFaction.IsAtWarWith(info.OriginalFaction))
            {
                mob.MemberRoster.AddToCounts(mob.Party.Owner.CharacterObject, 1, false, 0, 0, true, -1);
            }

            mob.InitializeMobileParty(new TextObject(revolutionaryMob.ToString(), null), roster, prisonRoster, settlement.GatePosition, 2.0f, 2.0f);

            this.Revolutionaries.Add(new Tuple<PartyBase, SettlementInfo>(mob.Party, info));

            MobileParty garrison = settlement.Parties.FirstOrDefault(party => party.IsGarrison);

            if (garrison == null)
            {
                foreach (var party in settlement.Parties.Where(party => party.IsMilitia || party.MapFaction.StringId == settlement.OwnerClan.MapFaction.StringId))
                {
                    garrison = party;
                    break;
                }
            }

            if (garrison == null)
            {
                ChangeOwnerOfSettlementAction.ApplyByRevolt(selectedHero, settlement);
                this.GetFactionInformation(info.CurrentFaction).CityRevoltedSuccess(settlement);
            }
            else
            {
                Campaign.Current.MapEventManager.StartSiegeOutsideMapEvent(mob.Party, garrison.Party);
            }

            info.RevoltProgress = 0;
        }

        private Clan CreateRebellionClan(SettlementInfo info)
        {
            Clan clan = MBObjectManager.Instance.CreateObject<Clan>(info.Settlement.Name.ToString() + "_rebels_" + MBRandom.RandomInt(100000).ToString());
            Hero hero = HeroCreator.CreateSpecialHero(this.CreateLordCharacter(info.OriginalCulture), info.Settlement, clan, clan, 30);

            TextObject name = new TextObject(hero.Name.ToString());
            int value = MBMath.ClampInt(1, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
            hero.SetTraitLevel(DefaultTraits.Commander, value);
            hero.ChangeState(Hero.CharacterStates.Active);
            hero.Initialize();
            clan.InitializeClan(name, name, info.OriginalCulture, Banner.CreateRandomClanBanner(MBRandom.RandomInt(100000)));
            hero.Clan = clan;
            clan.SetLeader(hero);
            clan.IsUnderMercenaryService = false;
            var kingdom = this.FormRebelKingdom(clan, info.Settlement, info.CurrentFaction);
            kingdom.ReactivateKingdom();
            DeclareWarAction.Apply(kingdom, info.CurrentFaction);
            clan.ClanJoinFaction(kingdom);
            kingdom.RulingClan = clan;

            return clan;
        }

        private CharacterObject CreateLordCharacter(CultureObject culture)
        {
            List<CharacterObject> characterObjects = new List<CharacterObject>();

            foreach (CharacterObject characterObject in CharacterObject.Templates)
            {
                if (characterObject.Occupation == Occupation.Lord
                            && characterObject.Culture == culture && !(characterObject.AllEquipments == null || characterObject.AllEquipments.IsEmpty())
                            && characterObject.FirstBattleEquipment != null && characterObject.FirstCivilianEquipment != null)
                {
                    characterObjects.Add(characterObject);
                }
            }

            return characterObjects[MBRandom.RandomInt(characterObjects.Count)];
        }

        private string GetClanKingdomId(Settlement originSettlement)
        {
            return $"{originSettlement.Name.ToString().ToLower()}_kingdom";
        }

        private Kingdom FormRebelKingdom(Clan clan, Settlement originSettlement, IFaction warOnFaction)
        {
            string kingdomId = this.GetClanKingdomId(originSettlement);
            var kingdom = Kingdom.All.SingleOrDefault(x => x.StringId == kingdomId);

            if (kingdom == null)
            {
                kingdom = Common.Instance.CreateKingdomFromSettlement(clan, kingdomId, originSettlement.Name.ToString());
                DeclareWarAction.Apply(kingdom, warOnFaction);
            }

            if (!Kingdom.All.Contains(kingdom))
            {
                Common.Instance.ModifyKingdomList(kingdoms => kingdoms.Add(kingdom));
            }

            return kingdom;
        }
    }
}
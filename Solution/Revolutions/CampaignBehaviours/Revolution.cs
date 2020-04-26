using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Helpers;
using Revolutions.Screens;
using SandBox.BoardGames;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SettlementActivities;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
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
            foreach (var settlementInfo in SettlementInformation)
            {
                if (settlementInfo.Settlement.StringId == settlement.StringId)
                {
                    return settlementInfo;
                }
            }

            SettlementInfo missingSettlement = new SettlementInfo(settlement);
            SettlementInformation.Add(missingSettlement);

            return missingSettlement;
        }

        public FactionInfo GetFactionInformation(IFaction faction)
        {
            foreach (var factioninfo in FactionInformation)
            {
                if (factioninfo.stringID == faction.StringId)
                {
                    return factioninfo;
                }
            }

            FactionInfo missingInformation = new FactionInfo(faction);
            FactionInformation.Add(missingInformation);

            return missingInformation;
        }   
        #endregion

        #region Base Functionality

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this,
                new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this,
                new Action<Settlement, bool, Hero, Hero, Hero,
                    ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this,
                new Action<Settlement>(this.DailySettlementTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
            
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
        }
        
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_SettlementInformation", ref SettlementInformation);
            dataStore.SyncData("_FactionInformation", ref FactionInformation);
            dataStore.SyncData("_Revolutionaries", ref Revolutionaries);
        }
        
        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            if (SettlementInformation.Count < 1)
            {
                RegisterSettlements();
            }

            if (FactionInformation.Count < 1)
            {
                RegisterFactions();
            }

            CreateLoyaltyMenu(obj);
        }

        private void RegisterSettlements()
        {
            foreach (var settlement in Settlement.All)
            {
                SettlementInfo settInf = new SettlementInfo(settlement);
                SettlementInformation.Add(settInf);
            }
        }

        private void RegisterFactions()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                FactionInfo fi = new FactionInfo(faction);
                FactionInformation.Add(fi);
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
                SettlementInfo setinf = GetSettlementInformation(Settlement.CurrentSettlement);
                FactionInfo factinfo = GetFactionInformation(Settlement.CurrentSettlement.MapFaction);
                ScreenManager.PushScreen(new TownRevolutionScreen(setinf, factinfo));
            }, false, 4);
        }

        #endregion

        #region Events

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            for (int i = 0; i < FactionInformation.Count; i++)
            {
                if (kingdom.StringId == FactionInformation[i].stringID)
                {
                    FactionInformation.RemoveAt(i);
                    return;
                }
            }
        }

        private void ClanDestroyedEvent(Clan clan)
        {
            for (int i = 0; i < FactionInformation.Count; i++)
            {
                if (clan.StringId == FactionInformation[i].stringID)
                {
                    FactionInformation.RemoveAt(i);
                    return;
                }
            }
        }
        
        private void DailyTickEvent()
        {
            foreach (var faction in FactionInformation)
            {
                faction.UpdateFactionInfo();
            }

            foreach (var settlement in SettlementInformation)
            {
                settlement.UpdateOwnership();
            }
        }
        
        private void DailySettlementTick(Settlement settlement)
        {
            DailyTownEvent(settlement);
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool bl, Hero hero1, Hero hero2, Hero hero3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            SettlementInfo settlementInfo = GetSettlementInformation(settlement);

            if (settlementInfo != null)
            {
                settlementInfo.ResetOwnership();
            }
        }

        private void OnMapEventEnded(MapEvent mapEvent)
        {
            RevoltMapEventEnd(mapEvent);
        }

        #endregion

        private void RevoltMapEventEnd(MapEvent mapEvent)
        {
            PartyBase revs = null;
            SettlementInfo currentInfo = null;
            Settlement settlement = null;
            
            foreach (Tuple<PartyBase,SettlementInfo> pair in Revolutionaries)
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
                GetFactionInformation(settlement.MapFaction).CityRevoltedFailure(settlement);
                RemoveRevolutionaryPartyFromList(revs);
                return;
            }
            
            Hero selectedHero = null;
            Clan chosenClan = null;
            int leastSettlements = 100;

            foreach (var noble in revs.Owner.MapFaction.Nobles)
            {
                int currentSettlements = noble.Clan.Settlements.Count();
                if (currentSettlements >= leastSettlements) continue;
                leastSettlements = currentSettlements;
                chosenClan = noble.Clan;
            }

            selectedHero = chosenClan != null ? chosenClan.Nobles.GetRandomElement() : currentInfo.OriginalFaction.Leader;

            RemoveRevolutionaryPartyFromList(revs);
            
            if (revs.Owner.MapFaction.StringId == currentInfo.OriginalFaction.StringId)
            {
                revs.MobileParty.RemoveParty();
                ChangeOwnerOfSettlementAction.ApplyByDefault(selectedHero, currentInfo.Settlement);
            }
            else
            {
                revs.MobileParty.IsLordParty = true;
                revs.MobileParty.Ai.EnableAi();;
                revs.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
                revs.MobileParty.Name = revs.Owner.Name;
                Clan own = revs.Owner.Clan;
                own.AddParty(revs);
                ChangeOwnerOfSettlementAction.ApplyByDefault(selectedHero, currentInfo.Settlement);
            }
            
            GetFactionInformation(settlement.MapFaction).CityRevoltedSuccess(settlement);
            settlement.AddGarrisonParty(true);
        }

        private void RemoveRevolutionaryPartyFromList(PartyBase revolutionaryParty)
        {
            for (int i = 0; i < Revolutionaries.Count; i++)
            {
                if (Revolutionaries[i].Item1.Id == revolutionaryParty.Id)
                {
                    PartyBase partyToRemove = Revolutionaries[i].Item1;
                    Revolutionaries.RemoveAt(i);
                    break;
                }
            }
        }

        private void DailyTownEvent(Settlement settlement)
        {
            if (!settlement.IsTown)
            {
                return;
            }
            
            IncreaseDailyLoyaltyForPlayerSettlement(settlement);
            CheckRevoltProgress(settlement);
        }

        private void CheckRevoltProgress(Settlement settlement)
        {
            SettlementInfo info = GetSettlementInformation(settlement);
            
            if (info.OriginalFaction.Name == settlement.MapFaction.Name)
            {
                return;
            }
            
            if (!GetFactionInformation(info.CurrentFaction).RevoltCanHappen())
            {
                info.RevoltProgress = 0;
                return;
            }
            
            info.RevoltProgress = info.RevoltProgress + (MinimumObedianceLoyalty - settlement.Town.Loyalty);

            if (info.RevoltProgress >= 100 && !info.Settlement.IsUnderSiege)
            {
                RevoltLogic(info, settlement);
            }

            if (info.RevoltProgress < 0)
            {
                info.RevoltProgress = 0;
            }
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
                settlement.Town.Loyalty = settlement.Town.Loyalty + PlayerInTownLoyaltyIncrease;
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

            TroopRoster prisonRoster = new TroopRoster();
            prisonRoster.IsPrisonRoster = true;

            if (info.CurrentFaction.IsAtWarWith(info.OriginalFaction))
            {
                Clan chosenClan = null;
                int leastSettlements = 100;
                foreach (var noble in info.OriginalFaction.Nobles)
                {
                    int currentSettlements = noble.Clan.Settlements.Count();
                    if (currentSettlements >= leastSettlements) continue;
                    leastSettlements = currentSettlements;
                    chosenClan = noble.Clan;
                }

                selectedHero = chosenClan != null ? chosenClan.Nobles.GetRandomElement() : info.OriginalFaction.Leader;
            }
            else
            {
                var clan = CreateRebellionClan(info);
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

            //mob.Ai.DisableAi();
            Revolutionaries.Add(new Tuple<PartyBase, SettlementInfo>(mob.Party, info));

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
                GetFactionInformation(info.CurrentFaction).CityRevoltedSuccess(settlement);
            }
            else
            {
                Campaign.Current.MapEventManager.StartSiegeOutsideMapEvent(mob.Party, garrison.Party);
            }

            info.RevoltProgress = 0;
        }

        private Clan CreateRebellionClan(SettlementInfo info)
        {
            var clan = MBObjectManager.Instance.CreateObject<Clan>(info.Settlement.Name.ToString() + "_rebels_" + MBRandom.RandomInt(100000).ToString());
            var hero = HeroCreator.CreateSpecialHero(CreateLordCharacter(info.OriginalCulture), info.Settlement, clan, clan, 30);
            TextObject name = new TextObject(hero.Name.ToString()); 
            int value = MBMath.ClampInt(1, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
            hero.SetTraitLevel(DefaultTraits.Commander, value);
            hero.ChangeState(Hero.CharacterStates.Active);
            hero.Initialize();
            clan.InitializeClan(name, name, info.OriginalCulture, Banner.CreateRandomClanBanner(MBRandom.RandomInt(100000)));
            hero.Clan = clan;
            clan.SetLeader(hero);
            clan.IsUnderMercenaryService = false;
            var kingdom = FormRebelKingdom(clan, info.Settlement, info.CurrentFaction);
            clan.ClanJoinFaction(kingdom);
            return clan;
        }

        private Kingdom CreateKingdom(Clan rulingClan, string stringID, Settlement settlement)
        {
            var kingdom = MBObjectManager.Instance.CreateObject<Kingdom>(stringID);
            string kingdomName = settlement.Name.ToString();
            TextObject textObject = new TextObject(kingdomName, null);
            kingdom.InitializeKingdom(textObject, textObject, rulingClan.Culture, rulingClan.Banner, 
                rulingClan.Color, rulingClan.Color2, rulingClan.InitialPosition);
            kingdom.RulingClan = rulingClan;
            return kingdom;
        }
        
        private CharacterObject CreateLordCharacter(CultureObject culture)
        {
            List<CharacterObject> characterObjects = new List<CharacterObject>();
            
            foreach (CharacterObject characterObject in CharacterObject.Templates)
            {
                if (characterObject.Occupation == Occupation.Lord
                            && characterObject.Culture == culture  && !(characterObject.AllEquipments == null || characterObject.AllEquipments.IsEmpty())
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
            string kingdomId = GetClanKingdomId(originSettlement);
            var kingdom = Kingdom.All.SingleOrDefault(x => x.StringId == kingdomId);

            if (kingdom == null)
            {
                kingdom = CreateKingdom(clan, kingdomId, originSettlement);
                DeclareWarAction.Apply(kingdom, warOnFaction);
            }
            
            if (!Kingdom.All.Contains(kingdom))
            {
                ModifyKingdomList(kingdoms => kingdoms.Add(kingdom));
            }

            return kingdom;
        }
        
        private void ModifyKingdomList(Action<List<Kingdom>> modificator)
		{
			List<Kingdom> kingdoms = new List<Kingdom>(Campaign.Current.Kingdoms.ToList());
			modificator(kingdoms);
			AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
		}
    }
}

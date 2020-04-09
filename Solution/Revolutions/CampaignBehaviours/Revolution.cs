using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.SaveSystem;
using Revolutions.Screens;

namespace Revolutions
{
    public class SettlementInfo
    {
        public SettlementInfo(Settlement settlement)
        {
            _settlementId = settlement.StringId;
            _originalFactionId = settlement.MapFaction.StringId;
            _originalCultureId = settlement.Culture.StringId;
        }

        public string GetId()
        {
            return _settlementId;
        }

        public bool IsOfCulture(string cultureStringId)
        {
            if (_originalCultureId == cultureStringId)
            {
                return true;
            }

            return false;
        }

        public Settlement GetSettlement()
        {
            return Settlement.Find(_settlementId);
        }

        public IFaction GetOriginalFaction()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == _originalFactionId)
                {
                    return faction;
                }
            }

            return null;
        }

        public CultureObject GetOriginalCulture()
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(_originalCultureId);
        }
        
        [SaveableField(1)] private string _settlementId;
        [SaveableField(2)] private string _originalFactionId;
        [SaveableField(3)] private string _originalCultureId;
        [SaveableField(4)] public float RevoltProgress = 0;
    }

    public class Revolution : CampaignBehaviorBase
    {
        private const int PlayerInTownLoyaltyIncrease = 5;
        private const int LoyaltyChangeForForeignPower = 5;
        private const int MinimumObedianceLoyalty = 25;
        private const int ForeignLoyaltyChangeMultiplayer = 2;

        public List<SettlementInfo> SettlementInformation = new List<SettlementInfo>();
        public List<FactionInfo> FactionInformation = new List<FactionInfo>();
        public List<Tuple<PartyBase, SettlementInfo>> Revolutionaries = new List<Tuple<PartyBase, SettlementInfo>>();

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
        }
        
        private void OnMapEventEnded(MapEvent mapEvent)
        {
            PartyBase revs = null;
            SettlementInfo currentInfo = null;
            
            foreach (Tuple<PartyBase,SettlementInfo> pair in Revolutionaries)
            {
                if (mapEvent.InvolvedParties.Contains(pair.Item1))
                {
                    revs = pair.Item1;
                    currentInfo = pair.Item2;
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
                RemoveRevolutionaryPartyFromList(revs);
                return;
            }

            Hero selectedHero = null;
            Clan chosenClan = null;
            int leastSettlements = 100;
            foreach (var noble in currentInfo.GetOriginalFaction().Nobles)
            {
                int currentSettlements = noble.Clan.Settlements.Count();
                if (currentSettlements >= leastSettlements) continue;
                leastSettlements = currentSettlements;
                chosenClan = noble.Clan;
            }

            selectedHero = chosenClan != null ? chosenClan.Nobles.GetRandomElement() : currentInfo.GetOriginalFaction().Leader;

            revs.Owner = selectedHero;

            RemoveRevolutionaryPartyFromList(revs);
            revs.MobileParty.RemoveParty();
            ChangeOwnerOfSettlementAction.ApplyByRevolt(selectedHero, currentInfo.GetSettlement());
            currentInfo.GetSettlement().AddGarrisonParty(true);
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

        private SettlementInfo GetSettlementInformation(Settlement settlement)
        {
            foreach (var settlementInfo in SettlementInformation.Where(settlementInfo => settlementInfo.GetId() == settlement.StringId))
            {
                return settlementInfo;
            }

            SettlementInfo missingSettlement = new SettlementInfo(settlement);
            SettlementInformation.Add(missingSettlement);

            return missingSettlement;
        }

        private FactionInfo GetFactionInformation(IFaction faction)
        {
            foreach (var factionInfo in FactionInformation.Where(factionInfo => factionInfo.GetFaction().StringId == faction.StringId))
            {
                return factionInfo;
            }

            FactionInfo missingInformation = new FactionInfo(faction);
            FactionInformation.Add(missingInformation);

            return missingInformation;
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool bl, Hero hero1, Hero hero2, Hero hero3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            SettlementInfo settlementInfo = GetSettlementInformation(settlement);

            if (settlementInfo != null)
            {
                settlementInfo.RevoltProgress = 0;
            }
        }

        private void DailySettlementTick(Settlement settlement)
        {
            DailyTownEvent(settlement);
        }

        private void DailyTickEvent()
        {
            foreach (var faction in FactionInformation)
            {
                faction.UpdateFactionInfo();
            }
        }

        private void DailyLoyaltyEvent(Settlement settlement)
        {
            SettlementInfo info = GetSettlementInformation(settlement);

            if (info.GetOriginalFaction().MapFaction.Name == settlement.MapFaction.Name)
            {
                return;
            }

            if (!GetFactionInformation(info.GetSettlement().MapFaction).RevoltCanHappen())
            {
                info.RevoltProgress = 0;
                return;
            }
            
            //settlement in wrong hands, so penalty loyalty modifier per day
            settlement.Town.Loyalty = settlement.Town.Loyalty - CalculateLoyaltyChangeForForeignPower(info);
            info.RevoltProgress = info.RevoltProgress + (MinimumObedianceLoyalty - settlement.Town.Loyalty);

            if (info.RevoltProgress >= 100)
            {
                RevoltLogic(info, settlement);
            }

            if (info.RevoltProgress < 0)
            {
                info.RevoltProgress = 0;
            }
        }

        private void RevoltLogic(SettlementInfo info, Settlement settlement)
        {
            GetFactionInformation(info.GetSettlement().MapFaction).CityRevolted(settlement);

            InformationManager.DisplayMessage(new InformationMessage(settlement.Name.ToString() + " is revolting!"));

            Hero selectedHero = null;
            MobileParty mob = MobileParty.Create("Revolutionary Mob");
            TroopRoster roster = new TroopRoster();

            TroopRoster infantry = new TroopRoster();
            infantry.FillMembersOfRoster(300, settlement.Culture.MeleeMilitiaTroop);
            roster.Add(infantry);

            TroopRoster archers = new TroopRoster();
            archers.FillMembersOfRoster(200, settlement.Culture.RangedMilitiaTroop);
            roster.Add(archers);

            TroopRoster prisonRoster = new TroopRoster();
            prisonRoster.IsPrisonRoster = true;

            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.Name.Contains("Looter"))
                {
                    selectedHero = faction.Heroes.ToList()[0];
                }

            }

            mob.ChangePartyLeader(selectedHero.CharacterObject);
            mob.Party.Owner = selectedHero;
            mob.InitializeMobileParty(new TaleWorlds.Localization.TextObject("Revolutionary Mob", null), roster, prisonRoster, settlement.GatePosition, 2.0f, 2.0f);

            mob.Ai.DisableAi();
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
            }
            else
            {
                Campaign.Current.MapEventManager.StartSiegeOutsideMapEvent(mob.Party, garrison.Party);
            }

            info.RevoltProgress = 0;
        }

        private void DailyTownEvent(Settlement settlement)
        {
            if (!settlement.IsTown)
            {
                return;
            }

            DailyLoyaltyEvent(settlement);
            IncreaseDailyLoyaltyForPlayerSettlement(settlement);
        }

        private void IncreaseDailyLoyaltyForPlayerSettlement(Settlement settlement)
        {
            if (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.StringId != settlement.StringId)
            {
                return;
            }

            if (settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
            {
                InformationManager.DisplayMessage(new InformationMessage("Seeing you spend time at " + settlement.Name.ToString() + " , your subjects feel more loyal to you."));
                settlement.Town.Loyalty = settlement.Town.Loyalty + PlayerInTownLoyaltyIncrease;
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_SettlementInformation", ref SettlementInformation);
            dataStore.SyncData("_FactionInformation", ref FactionInformation);
            dataStore.SyncData("_Revolutionaries", ref Revolutionaries);
        }

        private int CalculateLoyaltyChangeForForeignPower(SettlementInfo info)
        {
            //by default, we can use a const.
            if (info.GetSettlement().MapFaction.Leader == Hero.MainHero)
            {    
                return GetFactionInformation(info.GetSettlement().MapFaction).TownsAboveInitial();
            }

            return LoyaltyChangeForForeignPower + GetFactionInformation(info.GetSettlement().MapFaction).TownsAboveInitial() * ForeignLoyaltyChangeMultiplayer;
        }

        private void CreateLoyaltyMenu(CampaignGameStarter obj)
        {
            obj.AddGameMenuOption("town", "town_enter_entr_option", "Town Loyalty", (MenuCallbackArgs args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }, (MenuCallbackArgs args) =>
            {
                SettlementInfo setinf = GetSettlementInformation(Settlement.CurrentSettlement);
                FactionInfo factinfo = GetFactionInformation(Settlement.CurrentSettlement.MapFaction);
                ScreenManager.PushScreen(new TownRevolutionScreen(setinf, factinfo));
            }, false, 4);
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
    }
}

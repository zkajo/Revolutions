using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.SaveSystem;
using HarmonyLib;
using SandBox;
using SandBox.Source;
using SandBox.CampaignBehaviors;
using SandBox.ViewModelCollection;

using Revolutions.Screens;

namespace Revolutions
{
    public class SettlementInfo
    {
        public SettlementInfo(Settlement _settlement)
        {
            settlementID = _settlement.StringId;
            OriginalFactionID = _settlement.MapFaction.StringId;
            OriginalCultureID = _settlement.Culture.StringId;
        }

        public string GetID()
        {
            return settlementID;
        }

        public bool IsOfCulture(string cultureStringID)
        {
            if (OriginalCultureID == cultureStringID)
            {
                return true;
            }

            return false;
        }

        public Settlement GetSettlement()
        {
            return Settlement.Find(settlementID);
        }

        public IFaction GetOriginalFaction()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == OriginalFactionID)
                {
                    return faction;
                }
            }

            return null;
        }

        public CultureObject GetOriginalCulture()
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(OriginalCultureID);
        }

        [SaveableField(1)] private string settlementID;
        [SaveableField(2)] private string OriginalFactionID;
        [SaveableField(3)] private string OriginalCultureID;
        [SaveableField(4)] public float RevoltProgress = 0;
    }

    public class Something : SandBox.Source.Towns.CommonAreaCampaignBehavior
    {
        public override void RegisterEvents()
        {
            base.RegisterEvents();
        }
    }

    public class Revolution : CampaignBehaviorBase
    {
        private const int PlayerInTownLoyaltyIncrease = 5;
        private const int LoyaltyChangeForForeignPower = 5;
        private const int MinimumObedianceLoyalty = 25;
        private const int ForeignLoyaltyChangeMultiplayer = 2;
        private bool inAlleyBattle = false;
        private Settlement alleyBattleSettlement = null;


        public List<SettlementInfo> SettlementInformation = new List<SettlementInfo>();
        public List<FactionInfo> FactionInformation = new List<FactionInfo>();
        public List<MobileParty> Revolutionaries = new List<MobileParty>();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero,
                                    ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailySettlementTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.PlayerBattleEndEvent));
        }

        private void PlayerBattleEndEvent(MapEvent mapevent)
        {
            if (inAlleyBattle && mapevent.EventType == MapEvent.BattleTypes.AlleyFight)
            {
                PartyBase looters;

                if (mapevent.PlayerSide == BattleSideEnum.Attacker)
                {
                    looters = mapevent.DefenderSide.LeaderParty;
                }
                else
                {
                    looters = mapevent.AttackerSide.LeaderParty;
                }

                looters.MobileParty.CurrentSettlement = alleyBattleSettlement;
                alleyBattleSettlement = null;

                inAlleyBattle = false;

                
            }
        }

        private SettlementInfo GetSettlementInformation(Settlement settlement)
        {
            foreach (var settlementInfo in SettlementInformation)
            {
                if (settlementInfo.GetID() == settlement.StringId)
                {
                    return settlementInfo;
                }
            }

            SettlementInfo missingSettlement = new SettlementInfo(settlement);
            SettlementInformation.Add(missingSettlement);

            return missingSettlement;
        }

        private FactionInfo GetFactionInformation(IFaction faction)
        {
            foreach (var factionInfo in FactionInformation)
            {
                if (factionInfo.GetFaction().StringId == faction.StringId)
                {
                    return factionInfo;
                }
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

            if (Revolutionaries.Count > 0)
            {
                int length = Revolutionaries.Count;

                for (int i = 0; i < length; i++)
                {
                    if (Revolutionaries[i].Party.Side != BattleSideEnum.None)
                    {
                        Revolutionaries[i].RemoveParty();
                        Revolutionaries.RemoveAt(i);
                        length = Revolutionaries.Count;
                        i--;
                    }
                }
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
            //Clan chosenClan = null;
            //int leastSettlements = 100;
            //foreach (var noble in info.GetOriginalFaction().Nobles)
            //{
            //    int currentSettlements = noble.Clan.Settlements.Count();
            //    if (currentSettlements < leastSettlements)
            //    {
            //        leastSettlements = currentSettlements;
            //        chosenClan = noble.Clan;
            //    }
            //}

            //if (chosenClan != null)
            //{
            //    selectedHero = chosenClan.Nobles.GetRandomElement();
            //}
            //else
            //{
            //    selectedHero = info.GetOriginalFaction().Leader;
            //}

            MobileParty mob = MobileParty.Create("Revolutionary Mob");
            TroopRoster roster = new TroopRoster();

            TroopRoster infnatry = new TroopRoster();
            infnatry.FillMembersOfRoster(300, settlement.Culture.MeleeMilitiaTroop);
            roster.Add(infnatry);

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
            //mob.CurrentSettlement = settlement;

            mob.Ai.DisableAi();

            Revolutionaries.Add(mob);
            //StartBattleAction.ApplyStartAssaultAgainstWalls(mob, settlement);
            MobileParty garrison = null;

            foreach (var party in settlement.Parties)
            {
                if (party.IsGarrison)
                {
                    garrison = party;
                    break;
                }
            }

            if (garrison == null)
            {
                foreach (var party in settlement.Parties)
                {
                    if (party.IsMilitia || party.MapFaction.StringId == settlement.OwnerClan.MapFaction.StringId)
                    {
                        garrison = party;
                        break;
                    }
                }
            }

            if (garrison == null)
            {
                ChangeOwnerOfSettlementAction.ApplyByRevolt(selectedHero, settlement);
            }
            else
            {
                //StartBattleAction.ApplyStartAssaultAgainstWalls(mob, settlement);
                //StartBattleAction.ApplyStartAssaultAgainstWalls(mob, settlement);
                Campaign.Current.MapEventManager.StartAlleyFightMapEvent(mob.Party, garrison.Party);
                inAlleyBattle = true;
                alleyBattleSettlement = settlement;
            }

            
            
            /*
            ChangeOwnerOfSettlementAction.ApplyByRevolt(selectedHero, settlement);
            */

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

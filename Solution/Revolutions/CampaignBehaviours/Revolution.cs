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

    public class Revolution : CampaignBehaviorBase
    {
        private const int PlayerInTownLoyaltyIncrease = 5;
        private const int LoyaltyChangeForForeignPower = 5;
        private const int MinimumObedianceLoyalty = 25;
        private const int ForeignLoyaltyChangeMultiplayer = 2;


        public List<SettlementInfo> SettlementInformation = new List<SettlementInfo>();
        public List<FactionInfo> FactionInformation = new List<FactionInfo>();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero,
                                    ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailySettlementTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
            
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
                GetFactionInformation(info.GetSettlement().MapFaction).CityRevolted(settlement);

                InformationManager.DisplayMessage(new InformationMessage(settlement.Name.ToString() + " is revolting!"));

                if (Settlement.CurrentSettlement == info.GetSettlement())
                {
                    PlayerEncounter.LeaveSettlement();
                    PlayerEncounter.Finish(true);
                }

                if (info.GetSettlement().Parties.Count > 0)
                {
                    int partyNumber = info.GetSettlement().Parties.Count;
                    for (int i = 0; i < partyNumber; i++)
                    {
                        if (info.GetSettlement().Parties[i].IsMilitia || info.GetSettlement().Parties[i].IsGarrison)
                        {
                            continue;
                        }

                        LeaveSettlementAction.ApplyForParty(info.GetSettlement().Parties[i]);
                        partyNumber = info.GetSettlement().Parties.Count;
                        i--;
                    }
                }

                Hero selectedHero = null;
                Clan chosenClan = null;
                int leastSettlements = 100;
                foreach (var noble in info.GetOriginalFaction().Nobles)
                {
                    int currentSettlements = noble.Clan.Settlements.Count();
                    if (currentSettlements < leastSettlements)
                    {
                        leastSettlements = currentSettlements;
                        chosenClan = noble.Clan;
                    }
                }

                if (chosenClan != null)
                {
                    selectedHero = chosenClan.Nobles.GetRandomElement();
                }
                else
                {
                    selectedHero = info.GetOriginalFaction().Leader;
                }

                ChangeOwnerOfSettlementAction.ApplyByRevolt(selectedHero, settlement);
                
                info.RevoltProgress = 0;
            }

            if (info.RevoltProgress < 0)
            {
                info.RevoltProgress = 0;
            }
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

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
        public List<SettlementInfo> SettlementInformation = new List<SettlementInfo>();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyLoyaltyEvent));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero,
                                    ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
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

            return null;
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool bl, Hero hero1, Hero hero2, Hero hero3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            SettlementInfo settlementInfo = GetSettlementInformation(settlement);

            if (settlementInfo != null)
            {
                settlementInfo.RevoltProgress = 0;
            }
        }

        private void DailyLoyaltyEvent(Settlement settlement)
        {
            SettlementInfo info = GetSettlementInformation(settlement);

            if (!settlement.IsTown)
            {
                return;
            }

            if (info.GetOriginalFaction().MapFaction.Name == settlement.MapFaction.Name)
            {
                return;
            }

            //settlement in wrong hands, so -5 loyalty modifier per day
            settlement.Town.Loyalty = settlement.Town.Loyalty - 5;
            info.RevoltProgress = info.RevoltProgress + (25 - settlement.Town.Loyalty);

            if (info.RevoltProgress >= 100)
            {
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

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_SettlementInformation", ref SettlementInformation);
        }

        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            if (SettlementInformation.Count < 1)
            {
                RegisterSettlements();
            }

            CreateLoyaltyMenu(obj);
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
                ScreenManager.PushScreen(new TownRevolutionScreen(setinf));
            }, false, 4);
        }

        private void RegisterSettlements()
        {
            foreach (var settlement in Settlement.All)
            {
                SettlementInfo settInf = new SettlementInfo(settlement);
                SettlementInformation.Add(settInf);
            }
        }
    }
}

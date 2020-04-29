using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using ModLibrary.Settlements;
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

        public PartyBase GetParty(string partyId)
        {
            return Campaign.Current.Parties.FirstOrDefault(party => party.Id == partyId);
        }

        public PartyBase GetParty(Revolution revolution)
        {
            return this.GetParty(revolution.PartyId);
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
                        settlement.Town.Loyalty += SubModule.Configuration.PlayerInTownLoyaltyIncrease;

                        if (settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
                        {
                            var textObject = GameTexts.FindText("str_GM_LoyaltyIncrease");
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
                var settlement = settlementInfoRevolutions.Settlement;
                var factionInfoRevolutions = RevolutionsManagers.FactionManager.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

                if (settlementInfoRevolutions.LoyalFactionId == settlementInfoRevolutions.CurrentFactionId)
                {
                    break;
                }

                if (!factionInfoRevolutions.CanRevolt)
                {
                    settlementInfoRevolutions.RevolutionProgress = 0;
                    break;
                }

                settlementInfoRevolutions.RevolutionProgress += SubModule.Configuration.MinimumObedianceLoyalty - settlement.Town.Loyalty;

                if (settlementInfoRevolutions.RevolutionProgress >= 100 && !settlement.IsUnderSiege)
                {
                    this.StartRevolution(settlement);
                    break;
                }

                if (settlementInfoRevolutions.RevolutionProgress < 0)
                {
                    settlementInfoRevolutions.RevolutionProgress = 0;
                }
            }
        }

        public void StartRevolution(Settlement settlement)
        {
            var settlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement);
            var factionInfoRevolutions = RevolutionsManagers.FactionManager.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

            this.Revolutions.Add(new Revolution());
        }

        public void EndFailedRevolution(Revolution revolution, SettlementInfoRevolutions settlementInfoRevolutions, FactionInfoRevolutions factionInfoRevolutions)
        {
            var currentSettlement = SettlementManager<SettlementInfo>.Instance.GetSettlement(settlementInfoRevolutions.SettlementId);

            factionInfoRevolutions.CityRevoltionFailed(currentSettlement);
            this.Revolutions.Remove(revolution);
        }

        public void EndSucceededRevoluton(Revolution revolution, SettlementInfoRevolutions settlementInfoRevolutions, FactionInfoRevolutions factionInfoRevolutions)
        {
            var currentSettlement = SettlementManager<SettlementInfo>.Instance.GetSettlement(settlementInfoRevolutions.SettlementId);

            //TODO: Succeed Logic
            factionInfoRevolutions.CityRevoltionSucceeded(currentSettlement);
        }
    }
}

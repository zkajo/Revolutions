using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.Factions;
using Revolutions.Settlements;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

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

        public void IncreaseDailyLoyaltyForSettlement(Settlement settlement)
        {
            SettlementInfoRevolutions settlementInfo = SettlementManager<SettlementInfoRevolutions>.Instance.GetSettlementInfo(settlement.StringId);

            if (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.StringId != settlement.StringId)
            {
                return;
            }

            if (!settlementInfo.IsOwnerInSettlement)
            {
                return;
            }

            settlement.Town.Loyalty += SubModule.Configuration.PlayerInTownLoyaltyIncrease;

            if (settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
            {
                TextObject textObject = GameTexts.FindText("str_GM_LoyaltyIncrease");
                textObject.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
                InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
            }
        }

        public bool CheckRevolutionProgress(Settlement settlement)
        {
            SettlementInfoRevolutions settlementInfoRevolutions = SettlementManager<SettlementInfoRevolutions>.Instance.GetSettlementInfo(settlement);
            FactionInfoRevolutions factionInfoRevolutions = FactionManager<FactionInfoRevolutions>.Instance.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

            if (settlementInfoRevolutions.OriginalFactionId == settlementInfoRevolutions.CurrentFactionId)
            {
                return false;
            }

            if (!factionInfoRevolutions.CanRevolt)
            {
                settlementInfoRevolutions.RevolutionProgress = 0;
                return false;
            }

            settlementInfoRevolutions.RevolutionProgress += SubModule.Configuration.MinimumObedianceLoyalty - settlement.Town.Loyalty;

            if (settlementInfoRevolutions.RevolutionProgress >= 100 && !settlement.IsUnderSiege)
            {
                return true;
            }

            if (settlementInfoRevolutions.RevolutionProgress < 0)
            {
                settlementInfoRevolutions.RevolutionProgress = 0;
            }

            return false;
        }

        public void StartRevolution(Settlement settlement)
        {
            SettlementInfoRevolutions settlementInfoRevolutions = SettlementManager<SettlementInfoRevolutions>.Instance.GetSettlementInfo(settlement);
            FactionInfoRevolutions factionInfoRevolutions = FactionManager<FactionInfoRevolutions>.Instance.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

            this.Revolutions.Add(new Revolution());
        }

        public void EndRevolution(Revolution revolution)
        {
            this.Revolutions.Remove(revolution);
        }
    }
}

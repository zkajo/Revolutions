using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.Factions;
using Revolutions.Settlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

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

        public bool CheckRevolutionProgress(SettlementInfoRevolutions settlementInfoRevolutions)
        {
            Settlement settlement = ((SettlementInfo)settlementInfoRevolutions).GetSettlement();
            FactionInfoRevolutions factionInfoRevolutions = FactionManager<FactionInfoRevolutions>.Instance.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

            if (settlementInfoRevolutions.OriginalFactionId == settlementInfoRevolutions.CurrentFactionId)
            {
                return false;
            }

            if (!factionInfoRevolutions.CanRevolt )
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

        public void StartRevolution(SettlementInfoRevolutions settlementInfoRevolutions)
        {
            Settlement settlement = ((SettlementInfo)settlementInfoRevolutions).GetSettlement();

            this.Revolutions.Add(new Revolution());
        }

        public void EndRevolution(Revolution revolution)
        {
            this.Revolutions.Remove(revolution);
        }
    }
}

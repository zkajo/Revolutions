using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public class SettlementManager
    {
        #region Singleton


        private SettlementManager() { }

        static SettlementManager()
        {
            SettlementManager.Instance = new SettlementManager();
        }

        public static SettlementManager Instance { get; private set; }

        #endregion

        public List<SettlementInfo> InitializeSettlementInfos()
        {
            var settlementInfos = new List<SettlementInfo>();

            foreach (Settlement settlement in Settlement.All)
            {
                settlementInfos.Add(new SettlementInfo(settlement));
            }

            return settlementInfos;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public class SettlementManager<T> where T : SettlementInfo, new()
    {
        #region Singleton

        private SettlementManager() { }

        static SettlementManager()
        {
            SettlementManager<T>.Instance = new SettlementManager<T>();
        }

        public static SettlementManager<T> Instance { get; private set; }

        #endregion

        public List<T> SettlementInfos = new List<T>();

        public void InitializeSettlementInfos()
        {
            foreach (var settlement in Settlement.All)
            {
                SettlementInfo info = GetSettlementInfo(settlement);
                if (info == null)
                {
                    this.SettlementInfos.Add((T)Activator.CreateInstance(typeof(T), settlement));
                }
            }
        }

        public T GetSettlementInfo(string settlementId)
        {
            return this.SettlementInfos.FirstOrDefault(settlementInfo => settlementInfo.SettlementId == settlementId);
        }

        public T GetSettlementInfo(Settlement settlement)
        {
            return this.GetSettlementInfo(settlement.StringId);
        }

        public Settlement GetSettlement(string settlementId)
        {
            return Settlement.All.FirstOrDefault(settlement => settlement.StringId == settlementId);
        }

        public Settlement GetSettlement(T settlementInfo)
        {
            return this.GetSettlement(settlementInfo.SettlementId);
        }
    }
}
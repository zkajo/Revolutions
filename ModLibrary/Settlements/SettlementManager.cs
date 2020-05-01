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
                    AddSettlement(settlement);
                }
            }
        }
        
        public void WatchSettlements()
        {
            if (SettlementInfos.Count() == Campaign.Current.Settlements.Count())
            {
                return;
            }

            foreach (var info in SettlementInfos)
            {
                info.Remove = true;
            }
            
            foreach (var settlement in Campaign.Current.Settlements)
            {
                var settlementInfo = SettlementInfos.FirstOrDefault(n => n.SettlementId == settlement.StringId);

                if (settlementInfo == null)
                {
                    AddSettlement(settlement);
                }
                else
                {
                    settlementInfo.Remove = false;
                }
            }

            int length = SettlementInfos.Count();
            
            for (int i = 0; i < length; i++)
            {
                if (SettlementInfos[i].Remove)
                {
                    RemoveSettlementInfo(SettlementInfos[i].SettlementId);
                    i--;
                }

                length = SettlementInfos.Count();
            }
        }

        public void AddSettlement(Settlement settlement)
        {
            this.SettlementInfos.Add((T)Activator.CreateInstance(typeof(T), settlement));
        }

        private void RemoveSettlementInfo(string settlementId)
        {
            var toRemove = SettlementInfos.FirstOrDefault(n => n.SettlementId == settlementId);
            SettlementInfos.Remove(toRemove);
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
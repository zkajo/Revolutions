using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Settlements
{
    public class SettlementManager<InfoType> : IManager<InfoType, Settlement> where InfoType : SettlementInfo, new()
    {
        #region Singleton

        private SettlementManager() { }

        static SettlementManager()
        {
            SettlementManager<InfoType>.Instance = new SettlementManager<InfoType>();
        }

        public static SettlementManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public List<InfoType> Infos { get; set; } = new List<InfoType>();

        public void InitializeInfos()
        {
            foreach (var settlement in Campaign.Current.Settlements)
            {
                this.GetInfoByObject(settlement);
            }
        }

        public InfoType GetInfoById(string id)
        {
            var settlementInfo = this.Infos.FirstOrDefault(info => info.SettlementId == id);

            if (settlementInfo != null)
            {
                return settlementInfo;
            }

            var missingSettlement = Campaign.Current.Settlements.FirstOrDefault(settlement => settlement.StringId == id);
            return missingSettlement != null ? this.AddInfo(missingSettlement) : null;
        }

        public InfoType GetInfoByObject(Settlement settlement)
        {
            return this.GetInfoById(settlement.StringId);
        }

        public InfoType AddInfo(Settlement settlement)
        {
            var settlementInfo = (InfoType)Activator.CreateInstance(typeof(InfoType), settlement);
            this.Infos.Add(settlementInfo);

            return settlementInfo;
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveAll(info => info.SettlementId == id);
        }

        public Settlement GetObjectById(string id)
        {
            return Campaign.Current.Settlements.FirstOrDefault(settlement => settlement.StringId == id);
        }

        public Settlement GetObjectByInfo(InfoType info)
        {
            return this.GetObjectById(info.SettlementId);
        }

        public void UpdateInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Settlements.Count())
            {
                return;
            }

            this.Infos.RemoveAll(info => !Campaign.Current.Settlements.Any(settlement => settlement.StringId == info.SettlementId));

            foreach (var settlement in Campaign.Current.Settlements)
            {
                this.GetInfoById(settlement.StringId);
            }
        }

        #endregion
    }
}
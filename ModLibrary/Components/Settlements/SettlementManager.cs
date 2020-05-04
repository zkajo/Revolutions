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
                this.AddInfo(settlement, true);
            }
        }

        public InfoType GetInfoById(string id, bool addIfNotFound = true)
        {
            var settlementInfo = this.Infos.FirstOrDefault(info => info.SettlementId == id);

            if (settlementInfo != null)
            {
                return settlementInfo;
            }

            if (!addIfNotFound)
            {
                return null;
            }

            var missingSettlement = this.GetObjectById(id);
            return missingSettlement != null ? this.AddInfo(missingSettlement, true) : null;
        }

        public InfoType GetInfoByObject(Settlement settlement, bool addIfNotFound = true)
        {
            return this.GetInfoById(settlement?.StringId, addIfNotFound);
        }

        public InfoType AddInfo(Settlement settlement, bool force = false)
        {
            if (!force)
            {
                var existingSettlementInfo = this.Infos.FirstOrDefault(info => info.SettlementId == settlement?.StringId);
                if (existingSettlementInfo != null)
                {
                    return existingSettlementInfo;
                }
            }

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
            return Campaign.Current.Settlements.FirstOrDefault(settlement => settlement?.StringId == id);
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

            this.Infos.RemoveAll(info => !Campaign.Current.Settlements.Any(settlement => settlement?.StringId == info.SettlementId));

            foreach (var settlement in Campaign.Current.Settlements.Where(settlement => !this.Infos.Any(info => info.SettlementId == settlement?.StringId)))
            {
                this.AddInfo(settlement, true);
            }
        }

        #endregion
    }
}
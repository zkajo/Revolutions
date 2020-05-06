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

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Settlements.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Settlements)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(Settlement gameObject)
        {
            var info = this.Infos.FirstOrDefault(i => i.SettlementId == gameObject.StringId);
            if (info != null)
            {
                return info;
            }

            info = (InfoType)Activator.CreateInstance(typeof(InfoType), gameObject);
            this.Infos.Add(info);

            return info;
        }

        public InfoType GetInfo(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.GetInfo(gameObject);
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveWhere(i => i.SettlementId == id);
        }

        public Settlement GetGameObject(string id)
        {
            return Campaign.Current.Settlements.FirstOrDefault(go => go.StringId == id);
        }

        public Settlement GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.SettlementId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count() == Campaign.Current.Settlements.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Settlements.Any(settlemgoent => settlemgoent.StringId == i.SettlementId));

            if(onlyRemoving)
            {
                return;
            }

            foreach (var settlement in Campaign.Current.Settlements.Where(go => !this.Infos.Any(i => i.SettlementId == go.StringId)))
            {
                this.GetInfo(settlement);
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ModLibrary.Components.Factions
{
    public class FactionManager<InfoType> /*: IManager<InfoType, IFaction>*/ where InfoType : FactionInfo, new()
    {
        #region Singleton

        private FactionManager() { }

        static FactionManager()
        {
            FactionManager<InfoType>.Instance = new FactionManager<InfoType>();
        }

        public static FactionManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public List<InfoType> Infos { get; set; } = new List<InfoType>();

        public void InitializeInfos()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                this.GetInfoByObject(faction);
            }
        }

        public InfoType GetInfoById(string id)
        {
            var factionInfo = this.Infos.FirstOrDefault(info => info.FactionId == id);

            if (factionInfo != null)
            {
                return factionInfo;
            }

            var missingFaction = Campaign.Current.Factions.FirstOrDefault(faction => faction.StringId == id);
            return missingFaction != null ? this.AddInfo(missingFaction) : null;
        }

        public InfoType GetInfoByObject(IFaction faction)
        {
            return this.GetInfoById(faction.StringId);
        }

        public InfoType AddInfo(IFaction faction)
        {
            var factionInfo = (InfoType)Activator.CreateInstance(typeof(InfoType), faction);
            this.Infos.Add(factionInfo);

            return factionInfo;
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveAll(info => info.FactionId == id);
        }

        public IFaction GetObjectById(string id)
        {
            return Campaign.Current.Factions.FirstOrDefault(faction => faction.StringId == id);
        }

        public IFaction GetObjectByInfo(InfoType info)
        {
            return this.GetObjectById(info.FactionId);
        }

        public void UpdateInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Factions.Count())
            {
                return;
            }

            this.Infos.RemoveAll(info => !Campaign.Current.Factions.Any(faction => faction.StringId == info.FactionId));

            foreach (var faction in Campaign.Current.Factions)
            {
                this.GetInfoById(faction.StringId);
            }
        }

        #endregion


        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            var noble = faction.Nobles.Aggregate((currentResult, current) => current.Clan.Settlements.Count() < currentResult.Clan.Settlements.Count() ? current : currentResult);
            return noble.Clan != null ? noble.Clan.Nobles.GetRandomElement().CharacterObject : faction.Leader.CharacterObject;
        }
    }
}
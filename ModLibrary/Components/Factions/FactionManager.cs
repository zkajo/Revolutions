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

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Factions.Count())
            {
                return;
            }

            foreach (var faction in Campaign.Current.Factions)
            {
                this.AddInfo(faction, false);
            }
        }

        public InfoType GetInfoById(string id, bool addIfNotFound = true)
        {
            var factionInfo = this.Infos.FirstOrDefault(info => info.FactionId == id);
            if (factionInfo != null)
            {
                return factionInfo;
            }

            if (!addIfNotFound)
            {
                return null;
            }

            var missingFaction = this.GetObjectById(id);
            return missingFaction != null ? this.AddInfo(missingFaction, true) : null;
        }

        public InfoType GetInfoByObject(IFaction faction, bool addIfNotFound = true)
        {
            return this.GetInfoById(faction.StringId, addIfNotFound);
        }

        public InfoType AddInfo(IFaction faction, bool force = false)
        {
            if (!force)
            {
                var existingFactionInfo = this.Infos.FirstOrDefault(info => info.FactionId == faction.StringId);
                if(existingFactionInfo != null)
                {
                    return existingFactionInfo;
                }
            }

            var factionInfo = (InfoType)Activator.CreateInstance(typeof(InfoType), faction);
            this.Infos.Add(factionInfo);

            return factionInfo;
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveWhere(info => info.FactionId == id);
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

            this.Infos.RemoveWhere(info => !Campaign.Current.Factions.ToList().Any(faction => faction.StringId == info.FactionId));

            foreach (var faction in Campaign.Current.Factions.Where(faction => !this.Infos.Any(info => info.FactionId == faction.StringId)))
            {
                this.AddInfo(faction, true);
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
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ModLibrary.Factions
{
    public class FactionManager<T> where T : FactionInfo, new()
    {
        #region Singleton

        private FactionManager() { }

        static FactionManager()
        {
            FactionManager<T>.Instance = new FactionManager<T>();
        }

        public static FactionManager<T> Instance { get; private set; }

        #endregion

        public List<T> FactionInfos = new List<T>();

        public void InitializeFactionInfos()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                this.GetFactionInfo(faction.StringId);
            }
        }

        public T GetFactionInfo(string factionId)
        {
            T factionInfo = this.FactionInfos.FirstOrDefault(info => info.FactionId == factionId);

            if (factionInfo != null)
            {
                return factionInfo;
            }

            IFaction missingFaction = Campaign.Current.Factions.FirstOrDefault(faction => faction.StringId == factionId);
            return missingFaction != null ? this.AddFactionInfo(missingFaction) : null;
        }

        public T GetFactionInfo(IFaction faction)
        {
            return this.GetFactionInfo(faction.StringId);
        }

        private T AddFactionInfo(IFaction faction)
        {
            var factionInfo = (T)Activator.CreateInstance(typeof(T), faction);
            this.FactionInfos.Add(factionInfo);

            return factionInfo;
        }

        private void RemoveFactionInfo(string factionId)
        {
            this.FactionInfos.RemoveAll(info => info.FactionId == factionId);
        }

        public IFaction GetFaction(string factionId)
        {
            return Campaign.Current.Factions.FirstOrDefault(faction => faction.StringId == factionId);
        }

        public IFaction GetFaction(T factionInfo)
        {
            return this.GetFaction(factionInfo.FactionId);
        }

        public void WatchFactions()
        {
            if (this.FactionInfos.Count() == Campaign.Current.Factions.Count())
            {
                return;
            }

            this.FactionInfos.RemoveAll(info => !Campaign.Current.Factions.Any(faction => faction.StringId == info.FactionId));

            foreach (var faction in Campaign.Current.Factions)
            {
                this.GetFactionInfo(faction);
            }
        }

        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            var noble = faction.Nobles.Aggregate((currentResult, current) => current.Clan.Settlements.Count() < currentResult.Clan.Settlements.Count() ? current : currentResult);
            return noble.Clan != null ? noble.Clan.Nobles.GetRandomElement().CharacterObject : faction.Leader.CharacterObject;
        }
    }
}
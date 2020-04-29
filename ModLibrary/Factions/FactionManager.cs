using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

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
                AddFaction(faction);
            }
        }

        public T GetFactionInfo(string factionId)
        {
            var faction = this.FactionInfos.FirstOrDefault(factionInfo => factionInfo.FactionId == factionId);

            if (faction != null)
            {
                return faction;
            }

            foreach (var mapFaction in Campaign.Current.Factions)
            {
                var info = this.FactionInfos.FirstOrDefault(factionInfo => factionInfo.FactionId == factionId);
                
                if (info == null)
                {
                    AddFaction(mapFaction);
                }
            }

            return this.FactionInfos.FirstOrDefault(factionInfo => factionInfo.FactionId == factionId);
        }

        public T GetFactionInfo(IFaction faction)
        {
            return this.GetFactionInfo(faction.StringId);
        }

        public IFaction GetFaction(string factionId)
        {
            return Campaign.Current.Factions.FirstOrDefault(faction => faction.StringId == factionId);
        }

        public IFaction GetFaction(T factionInfo)
        {
            return this.GetFaction(factionInfo.FactionId);
        }
        
        private void AddFaction(IFaction faction)
        {
            this.FactionInfos.Add((T)Activator.CreateInstance(typeof(T), faction));
        }
    }
}
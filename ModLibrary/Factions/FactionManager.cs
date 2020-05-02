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

            foreach (var info in this.FactionInfos)
            {
                info.Remove = true;
            }

            foreach (var faction in Campaign.Current.Factions)
            {
                var factionInfo = this.FactionInfos.FirstOrDefault(n => n.FactionId == faction.StringId);

                if (factionInfo == null)
                {
                    this.AddFactionInfo(faction);
                }
                else
                {
                    factionInfo.Remove = false;
                }
            }

            int length = this.FactionInfos.Count();

            for (int i = 0; i < length; i++)
            {
                if (this.FactionInfos[i].Remove)
                {
                    this.RemoveFactionInfo(this.FactionInfos[i].FactionId);
                    i--;
                }

                length = this.FactionInfos.Count();
            }
        }

        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            Clan chosenClan = null;
            int leastSettlements = 100;

            foreach (var noble in faction.Nobles)
            {
                int currentSettlements = noble.Clan.Settlements.Count();
                if (currentSettlements >= leastSettlements)
                {
                    continue;
                }

                leastSettlements = currentSettlements;
                chosenClan = noble.Clan;
            }

            return chosenClan != null ? chosenClan.Nobles.GetRandomElement().CharacterObject : faction.Leader.CharacterObject;
        }
    }
}
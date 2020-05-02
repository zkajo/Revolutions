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
                GetFactionInfo(faction.StringId);
            }
        }

        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            Hero selectedHero = null;
            Clan chosenClan = null;
            int leastSettlements = 100;
            foreach (var noble in faction.Nobles)
            {
                int currentSettlements = noble.Clan.Settlements.Count();
                if (currentSettlements >= leastSettlements)
                    continue;
                leastSettlements = currentSettlements;
                chosenClan = noble.Clan;
            }

            selectedHero = chosenClan != null ? chosenClan.Nobles.GetRandomElement() : faction.Leader;

            return selectedHero.CharacterObject;
        }

        public T GetFactionInfo(string factionId)
        {
            T info = this.FactionInfos.FirstOrDefault(factionInfo => factionInfo.FactionId == factionId);

            if (info != null)
            {
                return info;
            }

            IFaction missingFaction = Campaign.Current.Factions.FirstOrDefault(n => n.StringId == factionId);
            AddFaction(missingFaction);

            return this.FactionInfos.FirstOrDefault(factionInfo => factionInfo.FactionId == factionId);
        }

        public void WatchFactions()
        {
            if (FactionInfos.Count() == Campaign.Current.Factions.Count())
            {
                return;
            }

            foreach (var info in FactionInfos)
            {
                info.Remove = true;
            }

            foreach (var faction in Campaign.Current.Factions)
            {
                var factionInfo = FactionInfos.FirstOrDefault(n => n.FactionId == faction.StringId);

                if (factionInfo == null)
                {
                    AddFaction(faction);
                }
                else
                {
                    factionInfo.Remove = false;
                }
            }

            int length = FactionInfos.Count();

            for (int i = 0; i < length; i++)
            {
                if (FactionInfos[i].Remove)
                {
                    RemoveFactionInfo(FactionInfos[i].FactionId);
                    i--;
                }

                length = FactionInfos.Count();
            }
        }

        private void RemoveFactionInfo(string factionId)
        {
            var toRemove = FactionInfos.FirstOrDefault(n => n.FactionId == factionId);
            FactionInfos.Remove(toRemove);
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
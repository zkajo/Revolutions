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

            foreach (var gameObject in Campaign.Current.Factions)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(IFaction gameObject)
        {
            var info = this.Infos.FirstOrDefault(i => i.FactionId == gameObject.StringId);
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

            return this.GetInfo(id);
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveWhere(i => i.FactionId == id);
        }

        public IFaction GetGameObject(string id)
        {
            return Campaign.Current.Factions.FirstOrDefault(go => go.StringId == id);
        }

        public IFaction GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.FactionId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count() == Campaign.Current.Factions.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Factions.ToList().Any(go => go.StringId == i.FactionId));

            if(onlyRemoving)
            {
                return;
            }

            foreach (var faction in Campaign.Current.Factions.Where(go => !this.Infos.Any(i => i.FactionId == go.StringId)))
            {
                this.GetInfo(faction);
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
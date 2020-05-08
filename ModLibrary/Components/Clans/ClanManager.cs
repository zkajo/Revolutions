using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Components.Clans
{
    public class ClanManager<InfoType> : IManager<InfoType, Clan> where InfoType : ClanInfo, new()
    {
        #region Singleton

        private ClanManager() { }

        static ClanManager()
        {
            ClanManager<InfoType>.Instance = new ClanManager<InfoType>();
        }

        public static ClanManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Clans.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Clans)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(Clan gameObject)
        {
            var info = this.Infos.SingleOrDefault(i => i.ClanId == gameObject.StringId);
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
            this.Infos.RemoveWhere(i => i.ClanId == id);
        }

        public Clan GetGameObject(string id)
        {
            return Campaign.Current.Clans.SingleOrDefault(go => go.StringId == id);
        }

        public Clan GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.ClanId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            this.Infos.RemoveWhere(i => !Campaign.Current.Clans.Any(go => go.StringId == i.ClanId));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Clans)
            {
                this.GetInfo(gameObject);
            }
        }

        #endregion

        public Clan CreateClan(Hero leader, TextObject name, TextObject informalName)
        {
            Clan clan = MBObjectManager.Instance.CreateObject<Clan>();
            clan.Culture = leader.Culture;
            clan.AddRenown(900, false);
            clan.SetLeader(leader);
            leader.Clan = clan;

            clan.InitializeClan(name, informalName, leader.Culture, Banner.CreateRandomClanBanner(leader.StringId.GetDeterministicHashCode()));

            this.GetInfo(clan.StringId);
            return clan;
        }
    }
}
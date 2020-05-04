using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
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

        public List<InfoType> Infos { get; set; } = new List<InfoType>();

        public void InitializeInfos()
        {
            foreach (var clan in Campaign.Current.Clans)
            {
                this.AddInfo(clan, true);
            }
        }

        public InfoType GetInfoById(string id, bool addIfNotFound = true)
        {
            var clanInfo = this.Infos.FirstOrDefault(info => info.ClanId == id);

            if (clanInfo != null)
            {
                return clanInfo;
            }

            if (!addIfNotFound)
            {
                return null;
            }

            var missingClan = this.GetObjectById(id);
            return missingClan != null ? this.AddInfo(missingClan, true) : null;
        }

        public InfoType GetInfoByObject(Clan clan, bool addIfNotFound = true)
        {
            return this.GetInfoById(clan?.StringId, addIfNotFound);
        }

        public InfoType AddInfo(Clan clan, bool force = false)
        {
            if (!force)
            {
                var existingClanInfo = this.Infos.FirstOrDefault(info => info.ClanId == clan?.StringId);
                if (existingClanInfo != null)
                {
                    return existingClanInfo;
                }
            }

            var clanInfo = (InfoType)Activator.CreateInstance(typeof(InfoType), clan);
            this.Infos.Add(clanInfo);

            return clanInfo;
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveAll(info => info.ClanId == id);
        }

        public Clan GetObjectById(string id)
        {
            return Campaign.Current.Clans.FirstOrDefault(clan => clan?.StringId == id);
        }

        public Clan GetObjectByInfo(InfoType info)
        {
            return this.GetObjectById(info.ClanId);
        }

        public void UpdateInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Clans.Count())
            {
                return;
            }

            this.Infos.RemoveAll(info => !Campaign.Current.Clans.Any(clan => clan?.StringId == info.ClanId));

            foreach (var clan in Campaign.Current.Clans.Where(clan => !this.Infos.Any(info => info.ClanId == clan?.StringId)))
            {
                this.GetInfoById(clan.StringId);
            }
        }

        #endregion

        public Clan CreateClan(TextObject name, TextObject informalName, CultureObject culture, Hero owner, uint primaryColor, uint secondaryColor, uint labelColour, Vec2 position)
        {
            var clan = MBObjectManager.Instance.CreateObject<Clan>();
            clan.Culture = culture;
            clan.Name = name;
            clan.InformalName = informalName;
            clan.SetLeader(owner);
            clan.InitialPosition = position;
            clan.LabelColor = labelColour;
            clan.Color = primaryColor;
            clan.Color2 = secondaryColor;
            return clan;
        }
    }
}
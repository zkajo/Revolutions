using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Clans
{
    public class ClanManager<T> where T : ClanInfo, new()
    {
        #region Singleton

        private ClanManager() { }

        static ClanManager()
        {
            ClanManager<T>.Instance = new ClanManager<T>();
        }

        public static ClanManager<T> Instance { get; private set; }

        #endregion

        public List<T> ClanInfos = new List<T>();

        public void InitalizeClanInfos()
        {
            foreach (var clan in Campaign.Current.Clans)
            {
                ClanInfo clanInfo = this.GetClanInfo(clan);
                if (clanInfo == null)
                {
                    this.AddClanInfo(clan);
                }
            }
        }

        public T GetClanInfo(string clanId)
        {
            T clanInfo = this.ClanInfos.FirstOrDefault(info => info.ClanId == clanId);

            if (clanInfo != null)
            {
                return clanInfo;
            }

            Clan missingClan = Campaign.Current.Clans.FirstOrDefault(info => info.StringId == clanId);
            return missingClan != null ? this.AddClanInfo(missingClan) : null;
        }

        public T GetClanInfo(Clan clan)
        {
            return this.GetClanInfo(clan.StringId);
        }

        public T AddClanInfo(Clan clan)
        {
            var clanInfo = (T)Activator.CreateInstance(typeof(T), clan);
            this.ClanInfos.Add(clanInfo);

            return clanInfo;
        }

        public void RemoveClanInfo(string clanId)
        {
            this.ClanInfos.RemoveAll(info => info.ClanId == clanId);
        }

        public Clan GetClan(string clanId)
        {
            return Campaign.Current.Clans.FirstOrDefault(clan => clan.StringId == clanId);
        }

        public Clan GetClan(T clanInfo)
        {
            return this.GetClan(clanInfo.ClanId);
        }

        public void WatchClans()
        {
            if (this.ClanInfos.Count() == Campaign.Current.Clans.Count())
            {
                return;
            }

            this.ClanInfos.RemoveAll(info => !Campaign.Current.Clans.Any(clan => clan.StringId == info.ClanId));

            foreach (var clan in Campaign.Current.Clans)
            {
                this.GetClanInfo(clan);
            }
        }

        public Clan CreateClan(TextObject name, TextObject informalName, CultureObject culture, Hero owner, uint primaryColor, uint secondaryColor, uint labelColour, Vec2 position)
        {
            Clan clan = MBObjectManager.Instance.CreateObject<Clan>();
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
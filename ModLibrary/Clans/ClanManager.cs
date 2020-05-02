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
                ClanInfo clanInfo = GetClanInfo(clan);
                if (clanInfo == null)
                {
                    AddClanInfo(clan);
                }
            }
        }

        public void WatchClans()
        {
            SynchroniseWithGame();
        }

        private void SynchroniseWithGame()
        {
            if (ClanInfos.Count() == Campaign.Current.Clans.Count())
            {
                return;
            }

            foreach (var info in ClanInfos)
            {
                info.Remove = true;
            }

            foreach (var clan in Campaign.Current.Clans)
            {
                var clanInfo = ClanInfos.FirstOrDefault(n => n.StringId == clan.StringId);

                if (clanInfo == null)
                {
                    AddClanInfo(clan);
                }
                else
                {
                    clanInfo.Remove = false;
                }
            }

            int length = ClanInfos.Count();

            for (int i = 0; i < length; i++)
            {
                if (ClanInfos[i].Remove)
                {
                    RemoveClanInfo(ClanInfos[i].StringId);
                    i--;
                }

                length = ClanInfos.Count();
            }
        }

        public void RemoveClanInfo(string clanId)
        {
            var toRemove = ClanInfos.FirstOrDefault(n => n.StringId == clanId);
            ClanInfos.Remove(toRemove);
        }

        public T GetClanInfo(Clan clan)
        {
            T info = this.ClanInfos.FirstOrDefault(clanInfo => clanInfo.StringId == clan.StringId);

            if (info != null)
            {
                return info;
            }

            Clan missingClan = Campaign.Current.Clans.FirstOrDefault(n => n.StringId == clan.StringId);
            AddClanInfo(missingClan);

            return this.ClanInfos.FirstOrDefault(clanInfo => clanInfo.StringId == clan.StringId);
        }

        public void AddClanInfo(Clan clan)
        {
            this.ClanInfos.Add((T)Activator.CreateInstance(typeof(T), clan));
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
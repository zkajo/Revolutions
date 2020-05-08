using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Components.Kingdoms
{
    public class KingdomManager<InfoType> : IManager<InfoType, Kingdom> where InfoType : KingdomInfo, new()
    {
        #region Singleton

        private KingdomManager() { }

        static KingdomManager()
        {
            KingdomManager<InfoType>.Instance = new KingdomManager<InfoType>();
        }

        public static KingdomManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Kingdoms.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Kingdoms)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(Kingdom gameObject)
        {
            var info = this.Infos.SingleOrDefault(i => i.KingdomId == gameObject.StringId);
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
            this.Infos.RemoveWhere(i => i.KingdomId == id);
        }

        public Kingdom GetGameObject(string id)
        {
            return Campaign.Current.Kingdoms.SingleOrDefault(go => go?.StringId == id);
        }

        public Kingdom GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.KingdomId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count == Campaign.Current.Kingdoms.Count)
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Kingdoms.Any(go => go.StringId == i.KingdomId));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Kingdoms)
            {
                this.GetInfo(gameObject);
            }
        }

        #endregion

        public void ModifyKingdomList(Func<List<Kingdom>, List<Kingdom>> modificator)
        {
            var kingdoms = new List<Kingdom>(Campaign.Current.Kingdoms.ToList());
            kingdoms = modificator(kingdoms);
            if (kingdoms != null)
            {
                AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
            }
        }

        public void AddKingdom(Kingdom kingdom)
        {
            this.ModifyKingdomList(kingdoms =>
            {
                if (kingdoms.Contains(kingdom))
                {
                    return null;
                }

                kingdoms.Add(kingdom);
                return kingdoms;
            });
        }

        public void RemoveKingdom(Kingdom kingdom)
        {
            this.ModifyKingdomList(kingdoms =>
            {
                if (kingdoms.RemoveAll(k => k == kingdom) > 0)
                {
                    return kingdoms;
                }

                return null;
            });
        }

        public void RemoveAndDestroyKingdom(Kingdom kingdom)
        {
            this.ModifyKingdomList(kingdoms =>
            {
                if (kingdoms.RemoveAll(go => go == kingdom) > 0)
                {
                    DestroyKingdomAction.Apply(kingdom);
                    return kingdoms;
                }

                return null;
            });
        }

        public Kingdom CreateKingdom(Hero leader, Settlement settlement, TextObject name, TextObject informalName)
        {
            Kingdom kingdom = MBObjectManager.Instance.CreateObject<Kingdom>();
            kingdom.InitializeKingdom(name, informalName, leader.Culture, Banner.CreateRandomClanBanner(leader.StringId.GetDeterministicHashCode()), leader.Clan.Color, leader.Clan.Color2, leader.Clan.InitialPosition);

            ChangeKingdomAction.ApplyByJoinToKingdom(leader.Clan, kingdom, false);
            kingdom.RulingClan = leader.Clan;

            kingdom.AddPolicy(DefaultPolicies.NobleRetinues);

            MBObjectManager.Instance.RegisterObject(kingdom);
            this.AddKingdom(kingdom);

            this.GetInfo(kingdom).UserMadeKingdom = true;
            return kingdom;
        }
    }
}
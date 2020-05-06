using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

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
            if(this.Infos.Count() == Campaign.Current.Kingdoms.Count())
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
            var info = this.Infos.FirstOrDefault(i => i.KingdomId == gameObject.StringId);
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
            return Campaign.Current.Kingdoms.FirstOrDefault(go => go?.StringId == id);
        }

        public Kingdom GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.KingdomId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count() == Campaign.Current.Kingdoms.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Kingdoms.Any(go => go.StringId == i.KingdomId));

            if(onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Kingdoms.Where(go => !this.Infos.Any(i => i.KingdomId == go.StringId)))
            {
                this.GetInfo(gameObject);
            }
        }

        #endregion

        public void ModifyKingdomList(Action<List<Kingdom>> modificator)
        {
            var kingdoms = new List<Kingdom>(Campaign.Current.Kingdoms.ToList());
            modificator(kingdoms);
            AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
        }

        public Kingdom CreateKingdom(Clan rulingClan, string name, string informalName)
        {
            var kingdom = Game.Current.ObjectManager.CreateObject<Kingdom>();
            TextObject kingdomName = new TextObject(name, null);
            TextObject kingdomInformalName = new TextObject(informalName, null);

            kingdom.InitializeKingdom(kingdomName, kingdomInformalName, rulingClan.Culture, rulingClan.Banner, rulingClan.Color, rulingClan.Color2, rulingClan.InitialPosition);

            this.ModifyKingdomList(kingdoms => kingdoms.Add(kingdom));

            var info = this.GetInfo(kingdom.StringId);
            info.UserMadeKingdom = true;

            return kingdom;
        }
    }
}
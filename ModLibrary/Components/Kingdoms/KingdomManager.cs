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
            var info = this.Infos.SingleOrDefault(i => i.KingdomId == gameObject.Id.InternalValue);
            if (info != null)
            {
                return info;
            }

            info = (InfoType)Activator.CreateInstance(typeof(InfoType), gameObject);
            this.Infos.Add(info);

            return info;
        }

        public InfoType GetInfo(uint id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.GetInfo(gameObject);
        }

        public void RemoveInfo(uint id)
        {
            this.Infos.RemoveWhere(i => i.KingdomId == id);
        }

        public Kingdom GetGameObject(uint id)
        {
            return Campaign.Current.Kingdoms.SingleOrDefault(go => go?.Id.InternalValue == id);
        }

        public Kingdom GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.KingdomId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            this.Infos.RemoveWhere(i => !Campaign.Current.Kingdoms.Any(go => go.Id.InternalValue == i.KingdomId));

            if(onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Kingdoms)
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

            var info = this.GetInfo(kingdom.Id.InternalValue);
            info.UserMadeKingdom = true;

            return kingdom;
        }
    }
}
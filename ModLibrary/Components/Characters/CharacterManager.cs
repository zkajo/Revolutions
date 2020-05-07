using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ModLibrary.Components.Characters
{
    public class CharacterManager<InfoType> : IManager<InfoType, CharacterObject> where InfoType : CharacterInfo, new()
    {
        #region Singleton

        private CharacterManager() { }

        static CharacterManager()
        {
            CharacterManager<InfoType>.Instance = new CharacterManager<InfoType>();
        }

        public static CharacterManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Characters.Count)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(CharacterObject gameObject)
        {
            var info = this.Infos.SingleOrDefault(i => i.CharacterId == gameObject.Id.InternalValue);
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
            this.Infos.RemoveWhere(i => i.CharacterId == id);
        }

        public CharacterObject GetGameObject(uint id)
        {
            return Campaign.Current.Characters.SingleOrDefault(go => go.Id.InternalValue == id);
        }

        public CharacterObject GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.CharacterId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            this.Infos.RemoveWhere(i => !Campaign.Current.Characters.Any(go => go.Id.InternalValue == i.CharacterId));

            if(onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters)
            {
                this.GetInfo(gameObject);
            }
        }

        #endregion

        public Hero CreateLord(Settlement settlement)
        {
            var characterObjects = new List<CharacterObject>();

            foreach (var characterObject in CharacterObject.Templates)
            {
                if (characterObject.Occupation == Occupation.Lord && characterObject.Culture == settlement.Culture
                    && !(characterObject.AllEquipments == null || characterObject.AllEquipments.IsEmpty())
                    && characterObject.FirstBattleEquipment != null && characterObject.FirstCivilianEquipment != null)
                {
                    characterObjects.Add(characterObject);
                }
            }

            var character = characterObjects[MBRandom.RandomInt(characterObjects.Count)];
            var hero = HeroCreator.CreateSpecialHero(character, settlement, null, null, -1);
            this.GetInfo(hero.CharacterObject);

            return hero;
        }
    }
}
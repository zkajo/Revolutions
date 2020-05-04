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

        public List<InfoType> Infos { get; set; } = new List<InfoType>();

        public void InitializeInfos()
        {
            foreach (var character in Campaign.Current.Characters)
            {
                this.GetInfoByObject(character);
            }
        }

        public InfoType GetInfoById(string id)
        {
            var characterInfo = this.Infos.FirstOrDefault(info => info.CharacterId == id);

            if (characterInfo != null)
            {
                return characterInfo;
            }

            var missingCharacter = Campaign.Current.Characters.FirstOrDefault(character => character.StringId == id);
            return missingCharacter != null ? this.AddInfo(missingCharacter) : null;
        }

        public InfoType GetInfoByObject(CharacterObject character)
        {
            return this.GetInfoById(character.StringId);
        }

        public InfoType AddInfo(CharacterObject character)
        {
            var characterInfo = (InfoType)Activator.CreateInstance(typeof(InfoType), character);
            this.Infos.Add(characterInfo);

            return characterInfo;
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveAll(info => info.CharacterId == id);
        }

        public CharacterObject GetObjectById(string id)
        {
            return Campaign.Current.Characters.FirstOrDefault(character => character.StringId == id);
        }

        public CharacterObject GetObjectByInfo(InfoType info)
        {
            return this.GetObjectById(info.CharacterId);
        }

        public void UpdateInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Characters.Count())
            {
                return;
            }

            this.Infos.RemoveAll(info => !Campaign.Current.Characters.Any(character => character.StringId == info.CharacterId));

            foreach (var character in Campaign.Current.Characters)
            {
                this.GetInfoById(character.StringId);
            }
        }

        #endregion

        public CharacterObject CreateLordCharacter(CultureObject culture)
        {
            var characterObjects = new List<CharacterObject>();

            foreach (var characterObject in CharacterObject.Templates)
            {
                if (characterObject.Occupation == Occupation.Lord
                    && characterObject.Culture == culture && !(characterObject.AllEquipments == null || characterObject.AllEquipments.IsEmpty())
                    && characterObject.FirstBattleEquipment != null && characterObject.FirstCivilianEquipment != null)
                {
                    characterObjects.Add(characterObject);
                }
            }

            return characterObjects[MBRandom.RandomInt(characterObjects.Count)];
        }
    }
}
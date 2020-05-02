using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ModLibrary.Characters
{
    public class CharacterManager
    {
        #region Singleton

        private CharacterManager() { }

        static CharacterManager()
        {
            CharacterManager.Instance = new CharacterManager();
        }

        public static CharacterManager Instance { get; private set; }

        #endregion

        public CharacterObject CreateLordCharacter(CultureObject culture)
        {
            List<CharacterObject> characterObjects = new List<CharacterObject>();

            foreach (CharacterObject characterObject in CharacterObject.Templates)
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
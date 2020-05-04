using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Characters
{
    public class CharacterInfo
    {
        public CharacterInfo()
        {

        }

        public CharacterInfo(CharacterObject character)
        {
            this.CharacterId = character.StringId;
        }

        #region Reference Properties

        public string CharacterId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public CharacterObject Character => ModLibraryManagers.CharacterManager.GetObjectById(this.CharacterId);

        #endregion



        #endregion

        #region Normal Properties

        public bool CanJoinOtherKingdoms { get; set; } = true;

        #endregion
    }
}
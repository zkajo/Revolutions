namespace ModLibrary.Character
{
    public class CharacterManager
    {
        #region Singleton

        private static readonly CharacterManager instance;

        private CharacterManager() { }

        static CharacterManager()
        {
            CharacterManager.instance = new CharacterManager();
        }

        public static CharacterManager Instance => CharacterManager.instance;

        #endregion
    }
}
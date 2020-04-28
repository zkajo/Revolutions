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
    }
}
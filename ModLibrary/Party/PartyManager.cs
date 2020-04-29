namespace ModLibrary.Party
{
    public class PartyManager
    {
        #region Singleton

        private PartyManager() { }

        static PartyManager()
        {
            PartyManager.Instance = new PartyManager();
        }

        public static PartyManager Instance { get; private set; }

        #endregion
    }
}
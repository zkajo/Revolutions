namespace ModLibrary.Clans
{
    public class ClanManager
    {
        #region Singleton

        private ClanManager() { }

        static ClanManager()
        {
            ClanManager.Instance = new ClanManager();
        }

        public static ClanManager Instance { get; private set; }

        #endregion
    }
}
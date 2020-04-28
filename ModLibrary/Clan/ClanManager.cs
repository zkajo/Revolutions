namespace ModLibrary.Clan
{
    public class ClanManager
    {
        #region Singleton

        private static readonly ClanManagers instance;

        private ClanManager() { }

        static ClanManager()
        {
            ClanManager.instance = new ClanManager();
        }

        public static ClanManager Instance => ClanManager.instance;

        #endregion
    }
}
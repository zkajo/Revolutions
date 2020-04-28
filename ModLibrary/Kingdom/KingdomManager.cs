namespace ModLibrary.Kingdom
{
    public class KingdomManager
    {
        #region Singleton

        private static readonly KingdomManager instance;

        private KingdomManager() { }

        static KingdomManager()
        {
            KingdomManager.instance = new KingdomManager();
        }

        public static KingdomManager Instance => KingdomManager.instance;

        #endregion
    }
}
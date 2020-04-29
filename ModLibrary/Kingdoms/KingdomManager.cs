namespace ModLibrary.Kingdoms
{
    public class KingdomManager
    {
        #region Singleton

        private KingdomManager() { }

        static KingdomManager()
        {
            KingdomManager.Instance = new KingdomManager();
        }

        public static KingdomManager Instance { get; private set; }

        #endregion
    }
}
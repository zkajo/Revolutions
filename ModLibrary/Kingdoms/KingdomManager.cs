using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

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
        
        /// <summary>
        /// Modifies the kingdom list using Harmony.
        /// </summary>
        /// <param name="modificator"></param>
        public void ModifyKingdomList(Action<List<Kingdom>> modificator)
        {
            List<Kingdom> kingdoms = new List<Kingdom>(Campaign.Current.Kingdoms.ToList());
            modificator(kingdoms);
            AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
        }
    }
}
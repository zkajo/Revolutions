using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

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
        
        public static Clan CreateClan(TextObject name, TextObject informalName, CultureObject culture, Hero owner, uint primaryColor, uint secondaryColor, uint labelColour, Vec2 position)
        {
            Clan clan = MBObjectManager.Instance.CreateObject<Clan>();
            clan.Culture = culture;
            clan.Name = name;
            clan.InformalName = informalName;
            clan.SetLeader(owner);
            clan.InitialPosition = position;
            clan.LabelColor = labelColour;
            clan.Color = primaryColor;
            clan.Color2 = secondaryColor;
            return clan;
        }
    }
}
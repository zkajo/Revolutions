using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions
{
    public class Common
    {
        public static Common Instance = new Common();
        
        public void ModifyKingdomList(Action<List<Kingdom>> modificator)
        {
            List<Kingdom> kingdoms = new List<Kingdom>(Campaign.Current.Kingdoms.ToList());
            modificator(kingdoms);
            AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
        }
        
        public Kingdom CreateKingdomFromSettlement(Clan rulingClan, string stringID, string name)
        {
            var kingdom = MBObjectManager.Instance.CreateObject<Kingdom>(stringID);
            TextObject textObject = new TextObject(name, null);
            kingdom.InitializeKingdom(textObject, textObject, rulingClan.Culture, rulingClan.Banner, 
                rulingClan.Color, rulingClan.Color2, rulingClan.InitialPosition);
            kingdom.RulingClan = rulingClan;
            return kingdom;
        }
    }
}
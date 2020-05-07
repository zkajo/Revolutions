using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using ModLibrary;
using Revolutions.Components.Kingdoms;
using Revolutions.Components.Factions;
using Revolutions.Components.Clans;
using Revolutions.Components.Parties;
using Revolutions.Components.Characters;
using Revolutions.Components.Settlements;
using Revolutions.Components.Revolutions;

namespace Revolutions
{
    public class DataStorage
    {
        [SaveableField(0)]
        internal string SaveId = string.Empty;

        internal void InitializeData()
        {
            RevolutionsManagers.KingdomManager.InitializeInfos();
            RevolutionsManagers.FactionManager.InitializeInfos();
            RevolutionsManagers.ClanManager.InitializeInfos();
            RevolutionsManagers.PartyManager.InitializeInfos();
            RevolutionsManagers.CharacterManager.InitializeInfos();
            RevolutionsManagers.SettlementManager.InitializeInfos();
        }

        internal void LoadData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            RevolutionsManagers.FactionManager.Infos = ModLibraryManagers.FileManager.Load<HashSet<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            RevolutionsManagers.KingdomManager.Infos = ModLibraryManagers.FileManager.Load<HashSet<KingdomInfoRevolutions>>(directoryPath, "KingdomInfos");
            RevolutionsManagers.ClanManager.Infos = ModLibraryManagers.FileManager.Load<HashSet<ClanInfoRevolutions>>(directoryPath, "ClanInfos");
            RevolutionsManagers.PartyManager.Infos = ModLibraryManagers.FileManager.Load<HashSet<PartyInfoRevolutions>>(directoryPath, "PartyInfos");
            RevolutionsManagers.CharacterManager.Infos = ModLibraryManagers.FileManager.Load<HashSet<CharacterInfoRevolutions>>(directoryPath, "CharacterInfos");
            RevolutionsManagers.SettlementManager.Infos = ModLibraryManagers.FileManager.Load<HashSet<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            RevolutionsManagers.RevolutionManager.Revolutions = ModLibraryManagers.FileManager.Load<HashSet<Revolution>>(directoryPath, "Revolutions");
        }

        internal void SaveData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            ModLibraryManagers.FileManager.Save(RevolutionsManagers.KingdomManager.Infos, directoryPath, "KingdomInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.FactionManager.Infos, directoryPath, "FactionInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.ClanManager.Infos, directoryPath, "ClanInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.PartyManager.Infos, directoryPath, "PartyInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.CharacterManager.Infos, directoryPath, "CharacterInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.SettlementManager.Infos, directoryPath, "SettlementInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.RevolutionManager.Revolutions, directoryPath, "Revolutions");
        }
    }
}
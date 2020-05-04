using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using ModLibrary;
using ModLibrary.Components.Clans;
using ModLibrary.Components.Parties;
using Revolutions.Revolutions;
using Revolutions.Components.Settlements;
using Revolutions.Components.Factions;

namespace Revolutions
{
    public class DataStorage
    {
        [SaveableField(0)]
        internal string SaveId = string.Empty;

        internal void InitializeData()
        {
            RevolutionsManagers.FactionManager.InitializeInfos();
            ModLibraryManagers.ClanManager.InitializeInfos();
            ModLibraryManagers.PartyManager.InitializeInfos();
            RevolutionsManagers.SettlementManager.InitializeInfos();
        }

        internal void LoadData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            RevolutionsManagers.FactionManager.Infos = ModLibraryManagers.FileManager.Load<List<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            ModLibraryManagers.ClanManager.Infos = ModLibraryManagers.FileManager.Load<List<ClanInfo>>(directoryPath, "ClanInfos");
            ModLibraryManagers.PartyManager.Infos = ModLibraryManagers.FileManager.Load<List<PartyInfo>>(directoryPath, "PartyInfos");
            RevolutionsManagers.SettlementManager.Infos = ModLibraryManagers.FileManager.Load<List<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            RevolutionsManagers.RevolutionManager.Revolutions = ModLibraryManagers.FileManager.Load<List<Revolution>>(directoryPath, "Revolutions");
        }

        internal void SaveData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            ModLibraryManagers.FileManager.Save(RevolutionsManagers.FactionManager.Infos, directoryPath, "FactionInfos");
            ModLibraryManagers.FileManager.Save(ModLibraryManagers.ClanManager.Infos, directoryPath, "ClanInfos");
            ModLibraryManagers.FileManager.Save(ModLibraryManagers.PartyManager.Infos, directoryPath, "PartyInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.SettlementManager.Infos, directoryPath, "SettlementInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.RevolutionManager.Revolutions, directoryPath, "Revolutions");
        }
    }
}
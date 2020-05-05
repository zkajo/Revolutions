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
using System.Linq;

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

            var kingdomInfos = ModLibraryManagers.FileManager.Load<List<KingdomInfoRevolutions>>(directoryPath, "KingdomInfos");
            kingdomInfos.Reverse();
            kingdomInfos = kingdomInfos.GroupBy(kingdomInfo => kingdomInfo.KingdomId)
                .Select(kingdomInfo => kingdomInfo.First())
                .ToList();
            kingdomInfos.Reverse();
            RevolutionsManagers.KingdomManager.Infos = kingdomInfos.ToHashSet();

            var factionInfos = ModLibraryManagers.FileManager.Load<List<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            factionInfos.Reverse();
            factionInfos = factionInfos.GroupBy(factionInfo => factionInfo.FactionId)
                .Select(factionInfo => factionInfo.First())
                .ToList();
            factionInfos.Reverse();
            RevolutionsManagers.FactionManager.Infos = factionInfos.ToHashSet();

            var clanInfos = ModLibraryManagers.FileManager.Load<List<ClanInfoRevolutions>>(directoryPath, "ClanInfos");
            clanInfos.Reverse();
            clanInfos = clanInfos.GroupBy(clanInfo => clanInfo.ClanId)
                .Select(clanInfo => clanInfo.First())
                .ToList();
            clanInfos.Reverse();
            RevolutionsManagers.ClanManager.Infos = clanInfos.ToHashSet();

            var partyInfos = ModLibraryManagers.FileManager.Load<List<PartyInfoRevolutions>>(directoryPath, "PartyInfos");
            partyInfos.Reverse();
            partyInfos = partyInfos.GroupBy(partyInfo => partyInfo.PartyId)
                .Select(partyInfo => partyInfo.First())
                .ToList();
            partyInfos.Reverse();
            RevolutionsManagers.PartyManager.Infos = partyInfos.ToHashSet();

            var characterInfos = ModLibraryManagers.FileManager.Load<List<CharacterInfoRevolutions>>(directoryPath, "CharacterInfos");
            characterInfos.Reverse();
            characterInfos = characterInfos.GroupBy(characterInfo => characterInfo.CharacterId)
                .Select(characterInfo => characterInfo.First())
                .ToList();
            characterInfos.Reverse();
            RevolutionsManagers.CharacterManager.Infos = characterInfos.ToHashSet();

            var settlementInfos = ModLibraryManagers.FileManager.Load<List<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            settlementInfos.Reverse();
            settlementInfos = settlementInfos.GroupBy(settlementInfo => settlementInfo.SettlementId)
                .Select(settlementInfo => settlementInfo.First())
                .ToList();
            settlementInfos.Reverse();
            RevolutionsManagers.SettlementManager.Infos = settlementInfos.ToHashSet();

            var revolutions = ModLibraryManagers.FileManager.Load<List<Revolution>>(directoryPath, "Revolutions");
            revolutions.Reverse();
            revolutions = revolutions.GroupBy(revolution => revolution.PartyId)
                .Select(factionInfo => factionInfo.First())
                .ToList();
            revolutions.Reverse();
            RevolutionsManagers.RevolutionManager.Revolutions = revolutions.ToHashSet();
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
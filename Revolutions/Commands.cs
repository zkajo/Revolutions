using System.Collections.Generic;
using System.Linq;
using ModLibrary.Components.Kingdoms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Revolutions
{
    public static class Commands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("start_revolution", "revolutions")]
        public static string StartRevolution(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.start_revolution [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            if (settlement.IsUnderSiege)
            {
                return $"{settlement.Name} is under siege.";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.RevolutionProgress = 100;

            RevolutionsManagers.RevolutionManager.StartRebellionEvent(settlement);

            return $"Started a revolution in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("end_revolution", "revolutions")]
        public static string EndRevolution(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 2 || !strings.Contains("-s") || !strings.Contains("-w") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.end_revolution -s [Settlement Name] -w [Win (true|false)]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            var settlementNameIndex = 3;
            var winIndex = aggregatedString.IndexOf("-w ") + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, winIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var revolution = RevolutionsManagers.RevolutionManager.GetRevolutionBySettlementId(settlement.StringId);
            if (revolution == null)
            {
                return $"{settlementName} is not conflicted in a revolt.";
            }

            if (!bool.TryParse(aggregatedString.Substring(winIndex, aggregatedString.Length - winIndex), out var isWin))
            {
                return "Format is \"revolutions.end_revolution -s [Settlement Name] -w [Win (true|false)]\".";
            }

            if (isWin)
            {
                RevolutionsManagers.RevolutionManager.EndSucceededRevoluton(revolution);
            }
            else
            {
                RevolutionsManagers.RevolutionManager.EndFailedRevolution(revolution);
            }

            return $"Ended a {(isWin ? "successful" : "failed")} revolution in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to_player", "revolutions")]
        public static string SetLoyalToPlayer(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_to_player [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.LoyalFactionId = Hero.MainHero.MapFaction.StringId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to_current_owner", "revolutions")]
        public static string SetLoyalToCurrentOwner(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_to_current_owner [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to", "revolutions")]
        public static string SetLoyalTo(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 4 || !strings.Contains("-s") || !strings.Contains("-f") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_owner_to -s [Settlement Name] -f [Faction Name]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            var settlementNameIndex = 3;
            var factionNameIndex = aggregatedString.IndexOf("-f ") + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, factionNameIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var factionName = aggregatedString.Substring(factionNameIndex, aggregatedString.Length - factionNameIndex);

            var faction = Campaign.Current.Factions.FirstOrDefault(s => s.Name.ToString().ToLower() == factionName.ToLower());
            if (settlement == null)
            {
                return $"There is no Faction \"{factionName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.LoyalFactionId = faction.StringId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_owner_to_player", "revolutions")]
        public static string SetLoyalOwnerToPlayer(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_owner_to_player [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.RevolutionProgress = 0;
            settlementInfo.CurrentFactionId = Hero.MainHero.MapFaction.StringId;
            settlementInfo.LoyalFactionId = Hero.MainHero.MapFaction.StringId;
            settlement.OwnerClan = Hero.MainHero.Clan;

            return $"{settlement.Name} is now owned by and loyal to {Hero.MainHero.Clan.Name} ({settlementInfo.LoyalFaction.Name}).";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_owner_to", "revolutions")]
        public static string SetLoyalOwnerTo(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 4 || !strings.Contains("-s") || !strings.Contains("-c") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_owner_to -s [Settlement Name] -c [Clan Name]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            var settlementNameIndex = 3;
            var clanNameIndex = aggregatedString.IndexOf("-c ") + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, clanNameIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var clanName = aggregatedString.Substring(clanNameIndex, aggregatedString.Length - clanNameIndex);

            var clan = Campaign.Current.Clans.FirstOrDefault(s => s.Name.ToString().ToLower() == clanName.ToLower());
            if (clan == null)
            {
                return $"There is no Clan \"{clanName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement.StringId);
            settlementInfo.RevolutionProgress = 0;
            settlementInfo.CurrentFactionId = clan.MapFaction.StringId;
            settlementInfo.LoyalFactionId = clan.MapFaction.StringId;
            settlement.OwnerClan = clan;

            return $"{settlement.Name} is now owned by and loyal to {clan.Name} ({settlementInfo.LoyalFaction.Name}).";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("show_lucky_nations", "revolutions")]
        public static string ShowLuckyNations(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() > 0 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.show_lucky_nations\"";
            }

            List<string> luckyNations = new List<string>();

            foreach (var info in RevolutionsManagers.KingdomManager.Infos.Where(i => i.LuckyNation))
            {
                luckyNations.Add(info.Kingdom.Name.ToString());
            }

            return $"Lucky Nations: {luckyNations.Aggregate((i, j) => i + ", " + j)}";
        }
    }
}
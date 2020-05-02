using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using ModLibrary;
using ModLibrary.Characters;
using ModLibrary.Clans;
using ModLibrary.Parties;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using ModLibrary.Settlements;
using Revolutions.Components.Factions;
using Revolutions.Factions;
using Revolutions.Settlements;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using KingdomManager = ModLibrary.Kingdoms.KingdomManager;

namespace Revolutions.Revolutions
{
    public class RevolutionManager
    {
        #region Singleton

        private RevolutionManager() { }

        static RevolutionManager()
        {
            RevolutionManager.Instance = new RevolutionManager();
        }

        public static RevolutionManager Instance { get; private set; }

        #endregion

        public List<Revolution> Revolutions = new List<Revolution>();

        public Revolution GetRevolution(string partyId)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.PartyId == partyId);
        }

        public Revolution GetRevolution(Settlement settlement)
        {
            return this.Revolutions.FirstOrDefault(revolution => revolution.SettlementId == settlement.StringId);
        }
        
        public PartyBase GetParty(Revolution revolution)
        {
            return ModLibraryManagers.PartyManager.GetParty(revolution.PartyId);
        }

        public void IncreaseDailyLoyaltyForSettlement()
        {
            foreach (var settlementInfoRevolutions in RevolutionsManagers.SettlementManager.SettlementInfos)
            {
                var settlement = settlementInfoRevolutions.Settlement;

                foreach (var mobileParty in settlement.Parties)
                {
                    if (mobileParty.IsLordParty && mobileParty.Party.Owner.Clan == settlement.OwnerClan)
                    {
                        settlement.Town.Loyalty += SubModule.Configuration.PlayerInTownLoyaltyIncrease;

                        if (settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
                        {
                            var textObject = GameTexts.FindText("str_GM_LoyaltyIncrease");
                            textObject.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
                            InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                        }

                        break;
                    }
                }
            }
        }

        public void CheckRevolutionProgress()
        {
            foreach (var settlementInfoRevolutions in RevolutionsManagers.SettlementManager.SettlementInfos)
            {
                var settlement = settlementInfoRevolutions.Settlement;
                var factionInfoRevolutions = RevolutionsManagers.FactionManager.GetFactionInfo(settlementInfoRevolutions.CurrentFactionId);

                if (settlementInfoRevolutions.LoyalFactionId == settlementInfoRevolutions.CurrentFactionId)
                {
                    break;
                }

                if (!factionInfoRevolutions.CanRevolt)
                {
                    settlementInfoRevolutions.RevolutionProgress = 0;
                    break;
                }

                settlementInfoRevolutions.RevolutionProgress += SubModule.Configuration.MinimumObedianceLoyalty - settlement.Town.Loyalty;

                if (settlementInfoRevolutions.RevolutionProgress >= 100 && !settlement.IsUnderSiege)
                {
                    this.StartRebellionEvent(settlement);
                    break;
                }

                if (settlementInfoRevolutions.RevolutionProgress < 0)
                {
                    settlementInfoRevolutions.RevolutionProgress = 0;
                }
            }
        }

        public void EndFailedRevolution(Revolution revolution, SettlementInfoRevolutions settlementInfoRevolutions, FactionInfoRevolutions factionInfoRevolutions)
        {
            factionInfoRevolutions.CityRevoltionFailed(settlementInfoRevolutions.Settlement);
            this.Revolutions.Remove(revolution);
        }

        public void EndSucceededRevoluton(Revolution revolution, SettlementInfoRevolutions settlementInfoRevolutions, FactionInfoRevolutions factionInfoRevolutions)
        {
            var currentSettlement = SettlementManager<SettlementInfo>.Instance.GetSettlement(settlementInfoRevolutions.SettlementId);

            //TODO: Succeed Logic
            factionInfoRevolutions.CityRevoltionSucceeded(currentSettlement);
        }
        
        
        public void StartRebellionEvent(Settlement settlement)
		{
            var settlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement);
            var factionInfoRevolutions = settlementInfoRevolutions.CurrentFactionInfo;
            
            Hero hero = HeroCreator.CreateSpecialHero(CharacterManager.Instance.CreateLordCharacter(settlement.Culture), settlement, null, null, -1);
			PartyTemplateObject rebelsPartyTemplate = settlement.Culture.RebelsPartyTemplate;
			rebelsPartyTemplate.IncrementNumberOfCreated();
            
            string id = string.Concat("rebels_of_", settlement.Culture.StringId, "_", rebelsPartyTemplate.NumberOfCreated);
            TextObject name = GameTexts.FindText("str_GM_RevolutionaryMob");
            MobileParty mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(id, name, settlement.GatePosition, rebelsPartyTemplate, hero, true, true);

            int value = MBMath.ClampInt(1, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
			mobileParty.Party.Owner.SetTraitLevel(DefaultTraits.Commander, value);
			mobileParty.Party.Owner.ChangeState(Hero.CharacterStates.Active);
            

            Clan clan = ClanManager.CreateClan(hero.Name, hero.Name, hero.Culture, hero, settlement.MapFaction.Color,
                settlement.MapFaction.Color2, settlement.MapFaction.LabelColor, settlement.GatePosition);
            clan.InitializeClan(clan.Name, clan.Name, clan.Culture, Banner.CreateRandomBanner(MBRandom.RandomInt(0, 1000000)));

            DeclareWarAction.Apply(clan, settlement.MapFaction);
            mobileParty.Party.Owner.Clan = clan;
            mobileParty.AddElementToMemberRoster(mobileParty.MemberRoster.GetCharacterAtIndex(0), (int)((double)settlement.Prosperity * 0.1), false);
			mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);
            
			if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
			{
				foreach (TroopRosterElement troopRosterElement in settlement.MilitaParty.MemberRoster)
				{
					mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number, false);
				}
				settlement.MilitaParty.RemoveParty();
			}
            
			mobileParty.IsLordParty = true;
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			

            TextObject information = GameTexts.FindText("str_GM_RevoltNotification");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.AddQuickInformation(information, 0, null, "");
            
            this.Revolutions.Add(new Revolution(mobileParty.Party.Id, settlement));
		}
    }
}

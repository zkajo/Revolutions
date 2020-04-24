using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Revolutions.CampaignBehaviours
{
	public class RebellionsCampaignBehavior : CampaignBehaviorBase
	{
		public RebellionsCampaignBehavior()
		{
			this._rebellionMapEvents = new Dictionary<MapEvent, RebellionsCampaignBehavior.Rebels>();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<MapEvent, RebellionsCampaignBehavior.Rebels>>("_rebellionMapEvents", ref this._rebellionMapEvents);
		}

		private static bool CheckRebellionEvent(Settlement settlement)
		{
			if (settlement.MilitaParty == null || settlement.MilitaParty.CurrentSettlement == settlement)
			{
				MobileParty militaParty = settlement.MilitaParty;
				float num = (militaParty != null) ? militaParty.Party.TotalStrength : 0f;
				MobileParty garrisonParty = settlement.Town.GarrisonParty;
				float num2 = (garrisonParty != null) ? garrisonParty.Party.TotalStrength : 0f;
				if (num >= num2)
				{
					return true;
				}
			}
			return false;
		}

		public void StartRebellionEvent(Settlement settlement)
		{
			PartyTemplateObject rebelsPartyTemplate = settlement.Culture.RebelsPartyTemplate;
			rebelsPartyTemplate.IncrementNumberOfCreated();
			MobileParty mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(string.Concat(new object[]
			{
				"rebels_of_",
				settlement.Culture.StringId,
				"_",
				rebelsPartyTemplate.NumberOfCreated
			}));
			TextObject textObject = new TextObject("{=2LIV2cy7}{SETTLEMENT}'s rebels", null);
			textObject.SetTextVariable("SETTLEMENT", settlement.Name);
			mobileParty.InitializeMobileParty(textObject, rebelsPartyTemplate, settlement.GatePosition, 0f, 0f, MobileParty.PartyTypeEnum.Default, -1);
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (CharacterObject characterObject in CharacterObject.Templates)
			{
				if (characterObject.Occupation == Occupation.Lord && characterObject.Culture == settlement.Culture)
				{
					list.Add(characterObject);
				}
			}
			mobileParty.Party.Owner = HeroCreator.CreateSpecialHero(list[MBRandom.RandomInt(list.Count)], settlement, null, null, -1);
			mobileParty.MemberRoster.AddToCounts(mobileParty.Party.Owner.CharacterObject, 1, false, 0, 0, true, -1);
			int value = MBMath.ClampInt(1, DefaultTraits.Commander.MinValue, DefaultTraits.Commander.MaxValue);
			mobileParty.Party.Owner.SetTraitLevel(DefaultTraits.Commander, value);
			mobileParty.Party.Owner.ChangeState(Hero.CharacterStates.Active);
			mobileParty.Name = MobilePartyHelper.GeneratePartyName(mobileParty.Party.Owner.CharacterObject);
			Clan clan = Clan.CreateSettlementRebels(settlement, mobileParty.Party.Owner);
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
			StartRebellionAction.Apply(mobileParty.Party.Owner, mobileParty, settlement);
			this._rebellionMapEvents.Add(mobileParty.MapEvent, new RebellionsCampaignBehavior.Rebels(settlement, mobileParty));
			TextObject textObject2 = new TextObject("{=xWydBJyS}Rebels in {SETTLEMENT} have risen against their lord, {RULER}.", null);
			textObject2.SetTextVariable("SETTLEMENT", settlement.Name);
			textObject2.SetTextVariable("RULER", settlement.OwnerClan.Leader.Name);
			InformationManager.AddQuickInformation(textObject2, 0, null, "");
		}

		public void OnMapEventEnded(MapEvent mapEvent)
		{
			RebellionsCampaignBehavior.Rebels rebels = null;
			if (this._rebellionMapEvents.TryGetValue(mapEvent, out rebels))
			{
				if (mapEvent.BattleState == BattleState.AttackerVictory)
				{
					rebels.Settlement.Town.Loyalty += 100f - rebels.Settlement.Town.Loyalty;
					rebels.Settlement.CalculateSettlementValueForFactions();
				}
				else
				{
					rebels.Settlement.Town.Loyalty = 50f;
				}
				rebels.Settlement.Town.IsRebeling = false;
				rebels.Settlement.ResetSiegeState();
				this._rebellionMapEvents.Remove(mapEvent);
			}
		}

        private Dictionary<MapEvent, RebellionsCampaignBehavior.Rebels> _rebellionMapEvents;

		private static bool _rebellionEnabled;

		public class RebellionsCampaignBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			public RebellionsCampaignBehaviorTypeDefiner() : base(350041)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RebellionsCampaignBehavior.Rebels), 1);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<MapEvent, RebellionsCampaignBehavior.Rebels>));
			}
		}

		private class Rebels
		{
			[SaveableProperty(1)]
			public Settlement Settlement { get; private set; }

			[SaveableProperty(2)]
			public MobileParty RebelParty { get; private set; }

			public Rebels(Settlement settlement, MobileParty rebelParty)
			{
				this.Settlement = settlement;
				this.RebelParty = rebelParty;
			}
		}
	}
}
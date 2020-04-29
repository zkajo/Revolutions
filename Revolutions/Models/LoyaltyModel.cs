using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.Factions;
using Revolutions.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Revolutions.Models
{
    public class LoyaltyModel : DefaultSettlementLoyaltyModel
    {
        private readonly DataStorage DataStorage;

        public LoyaltyModel(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override float CalculateLoyaltyChange(Town town, StatExplainer explanation = null)
        {
            var explainedNumber = new ExplainedNumber(0.0f, explanation, (TextObject)null);
            var info = RevolutionsManagers.SettlementManager.GetSettlementInfo(town.Settlement.StringId);

            if (!town.IsTown)
            {
                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
            }

            if (info.Settlement.MapFaction.Leader == Hero.MainHero)
            {
                explainedNumber.Add(SubModule.Configuration.BasePlayerLoyalty, GameTexts.FindText("str_loyalty_bannerlord"));

                if (SubModule.Configuration.PlayerAffectedByOverextension && SubModule.Configuration.OverextensionMechanics)
                {
                    this.Overextension(info, ref explainedNumber);
                }
            }
            else
            {
                this.BaseLoyalty(info, ref explainedNumber);

                if (SubModule.Configuration.OverextensionMechanics)
                {
                    this.Overextension(info, ref explainedNumber);
                }
            }

            return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
        }

        private void Overextension(SettlementInfoRevolutions settlement, ref ExplainedNumber explainedNumber)
        {
            if (settlement.CurrentFaction.StringId == settlement.LoyalFaction.StringId)
            {
                return;
            }

            if (SubModule.Configuration.EmpireLoyaltyMechanics)
            {
                if (this.IsTownOfImperialCulture(settlement) && settlement.IsCurrentFactionOfImperialCulture)
                {
                    return;
                }
            }

            var factionInfo = RevolutionsManagers.FactionManager.GetFactionInfo(settlement.CurrentFactionId);
            var overExtension = factionInfo.InitialTownsCount - factionInfo.CurrentTownsCount;

            explainedNumber.Add(overExtension * SubModule.Configuration.OverExtensionMultiplier, GameTexts.FindText("str_loyalty_overextension"));
        }

        private void BaseLoyalty(SettlementInfoRevolutions info, ref ExplainedNumber explainedNumber)
        {
            if (SubModule.Configuration.EmpireLoyaltyMechanics)
            {
                if (this.IsTownOfImperialCulture(info))
                {
                    if (info.IsCurrentFactionOfImperialCulture)
                    {
                        explainedNumber.Add(10, GameTexts.FindText("str_loyalty_imperial"));
                    }
                    else
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                    }
                }
                else
                {
                    if (info.IsCurrentFactionOfImperialCulture)
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_imperialAvers"));
                    }

                    if (info.LoyalFactionId != info.CurrentFactionId)
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                    }
                }
            }
            else
            {
                if (info.LoyalFactionId != info.CurrentFactionId)
                {
                    explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                }
            }
        }

        private bool IsTownOfImperialCulture(SettlementInfoRevolutions settlement)
        {
            return settlement.Settlement.Culture.Name.ToLower().Contains("empire");
        }
    }
}
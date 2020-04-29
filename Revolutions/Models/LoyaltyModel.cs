using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using Revolutions.Settlements;

namespace Revolutions.Models
{
    public class LoyaltyModel : DefaultSettlementLoyaltyModel
    {
        private readonly DataStorage DataStorage;

        public LoyaltyModel(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override float CalculateLoyaltyChange(Town town, StatExplainer statExplainer = null)
        {
            var explainedNumber = new ExplainedNumber(0.0f, statExplainer, null);
            var settlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(town.Settlement.StringId);

            if (!town.IsTown)
            {
                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
            }

            if (settlementInfoRevolutions.CurrentFaction.Leader == Hero.MainHero)
            {
                explainedNumber.Add(SubModule.Configuration.BasePlayerLoyalty, GameTexts.FindText("str_loyalty_bannerlord"));

                if (SubModule.Configuration.PlayerAffectedByOverextension && SubModule.Configuration.OverextensionMechanics)
                {
                    this.Overextension(settlementInfoRevolutions, ref explainedNumber);
                }
            }
            else
            {
                this.BaseLoyalty(settlementInfoRevolutions, ref explainedNumber);

                if (SubModule.Configuration.OverextensionMechanics)
                {
                    this.Overextension(settlementInfoRevolutions, ref explainedNumber);
                }
            }

            return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
        }

        private void Overextension(SettlementInfoRevolutions settlementInfoRevolutions, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfoRevolutions.CurrentFaction.StringId == settlementInfoRevolutions.LoyalFaction.StringId)
            {
                return;
            }

            if (SubModule.Configuration.EmpireLoyaltyMechanics)
            {
                if (settlementInfoRevolutions.IsOfImperialCulture && settlementInfoRevolutions.IsCurrentFactionOfImperialCulture)
                {
                    return;
                }
            }

            var factionInfo = settlementInfoRevolutions.CurrentFactionInfo;
            var overExtension = factionInfo.InitialTownsCount - factionInfo.CurrentTownsCount;

            explainedNumber.Add(overExtension * SubModule.Configuration.OverExtensionMultiplier, GameTexts.FindText("str_loyalty_overextension"));
        }

        private void BaseLoyalty(SettlementInfoRevolutions settlementInfoRevolutions, ref ExplainedNumber explainedNumber)
        {
            if (SubModule.Configuration.EmpireLoyaltyMechanics)
            {
                if (settlementInfoRevolutions.IsOfImperialCulture)
                {
                    if (settlementInfoRevolutions.IsCurrentFactionOfImperialCulture)
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
                    if (settlementInfoRevolutions.IsCurrentFactionOfImperialCulture)
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_imperialAvers"));
                    }

                    if (settlementInfoRevolutions.LoyalFactionId != settlementInfoRevolutions.CurrentFactionId)
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                    }
                }
            }
            else
            {
                if (settlementInfoRevolutions.LoyalFactionId != settlementInfoRevolutions.CurrentFactionId)
                {
                    explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                }
            }
        }
    }
}
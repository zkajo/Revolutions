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
            var explainedNumber = new ExplainedNumber(0.0f, explanation, (TextObject) null);
            var info = RevolutionsManagers.SettlementManager.GetSettlementInfo(town.Settlement.StringId);
            
            if (!town.IsTown)
            {
                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
            }

            if (info.GetSettlement().MapFaction.Leader == Hero.MainHero)
            {    
                explainedNumber.Add(SubModule.Configuration.BasePlayerLoyalty, GameTexts.FindText("str_loyalty_bannerlord"));

                if (SubModule.Configuration.PlayerAffectedByOverextension && SubModule.Configuration.OverextensionMechanics)
                {
                    Overextension(info, ref explainedNumber);
                }
            }
            else
            {
                BaseLoyalty(info, ref explainedNumber);

                if (SubModule.Configuration.OverextensionMechanics)
                {
                    Overextension(info, ref explainedNumber);
                }
            }
            
            return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
        }

        private void Overextension(SettlementInfoRevolutions settlement, ref ExplainedNumber explainedNumber)
        {
            if (settlement.GetCurrentFaction().StringId == settlement.GetLoyalFaction().StringId)
            {
                return;
            }

            if (SubModule.Configuration.EmpireLoyaltyMechanics)
            {
                if (IsTownOfImperialCulture(settlement) && settlement.IsCurrentOwnerOfImperialCulture())
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
                if (IsTownOfImperialCulture(info))
                {
                    if (info.IsCurrentOwnerOfImperialCulture())
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
                    if (info.IsCurrentOwnerOfImperialCulture())
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
            return settlement.GetSettlement().Culture.Name.ToLower().Contains("empire");
        }
    }
}
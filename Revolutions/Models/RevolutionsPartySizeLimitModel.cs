using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace Revolutions.Models
{
    public class RevolutionsPartySizeLimitModel : DefaultPartySizeLimitModel
    {
        private readonly DataStorage DataStorage;

        public RevolutionsPartySizeLimitModel(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override int GetPartyMemberSizeLimit(PartyBase party, StatExplainer explanation = null)
        {
            if(RevolutionsManagers.RevolutionManager.Revolutions.Any(revolution => revolution.PartyId == party.Id))
            {
                return party.NumberOfAllMembers;
            }

            return base.GetPartyMemberSizeLimit(party, explanation);
        }
    }
}
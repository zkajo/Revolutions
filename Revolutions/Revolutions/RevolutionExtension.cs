using TaleWorlds.CampaignSystem;

namespace Revolutions.Revolutions
{
    public static class RevolutionExtension
    {
        public static void SetInitialValues(this Revolution revolution)
        {
        }

        public static PartyBase GetParty(this Revolution revolution)
        {
            return RevolutionsManagers.RevolutionManager.GetParty(revolution);
        }
    }
}
using TaleWorlds.CampaignSystem;

namespace Revolutions.Revolutions
{
    public static class RevolutionExtension
    {
        public static PartyBase GetParty(this Revolution revolution)
        {
            return RevolutionManager.Instance.GetParty(revolution);
        }
    }
}
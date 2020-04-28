using Revolutions.Screens;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.Screens;

namespace Revolutions.CampaignBehaviours
{
    public class ModOptions : CampaignBehaviorBase
    {
        public static ModOptionsData OptionsData = new ModOptionsData();

        public static void CreateModOptionsMenu()
        {
            ScreenManager.PushScreen(new ModOptionsScreen(ModOptions.OptionsData));
        }

        public override void RegisterEvents()
        {

        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_RevolutionsModOptionsdata", ref OptionsData);
        }
    }
}
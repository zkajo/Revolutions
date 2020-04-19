using System.Collections.Generic;
using Revolutions.Screens;
using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.ViewModelCollection;

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
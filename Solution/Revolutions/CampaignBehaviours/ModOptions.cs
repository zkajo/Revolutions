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
        
        public override void RegisterEvents()
        {
            
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_RevolutionsModOptionsdata", ref OptionsData);
        }
    }
    
    [OverrideView(typeof(MapEscapeMenu))]
    public class ModOptionsMenu : GauntletMapEscapeMenu
    {
        public ModOptionsMenu(List<EscapeMenuItemVM> items) : base(items)
        {
            items.Add((new EscapeMenuItemVM(new TextObject("Revolutions Options", null), delegate(object o)
                {
                    ShowModOptions();
                }, null, false,
                false)));
        }

        private void ShowModOptions()
        {
            ScreenManager.PushScreen(new ModOptionsScreen(ModOptions.OptionsData));
        }
    }
}
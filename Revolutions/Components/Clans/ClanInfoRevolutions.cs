using System;
using ModLibrary.Clans;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Clans
{
    [Serializable]
    public class ClanInfoRevolutions : ClanInfo
    {
        public ClanInfoRevolutions() : base()
        {
            
        }
        
        public ClanInfoRevolutions(Clan clan) : base(clan)
        {
            
        }
    }
}
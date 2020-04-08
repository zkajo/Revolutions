using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Revolutions
{
    public class FactionInfo
    {
        public FactionInfo(IFaction faction)
        {
            factionID = faction.StringId;
            foreach (var settlement in faction.Settlements)
            {
                if (settlement.IsTown)
                {
                    initialTownNumber++;
                }
            }
        }

        public bool RevoltCanHappen()
        {
            return !hadRecentRevolt;
        }

        public Settlement RevoltedSettlement()
        {
            return revoltedSettlement;
        }

        public void UpdateFactionInfo()
        {
            UpdateCurrentTownCount();
            daysSinceLastRevolt++;

            //three years
            if (daysSinceLastRevolt > 252)
            {
                hadRecentRevolt = false;
                revoltedSettlement = null;
            }
        }

        public void CityRevolted(Settlement settlement)
        {
            hadRecentRevolt = true;
            revoltedSettlement = settlement;
            daysSinceLastRevolt = 0;
        }

        private void UpdateCurrentTownCount()
        {
            currentTownNumber = 0;
            foreach (var settlement in GetFaction().Settlements)
            {
                if (settlement.IsTown)
                {
                    currentTownNumber++;
                }
            }
        }

        public int TownsAboveInitial()
        {
            return currentTownNumber - initialTownNumber;
        }

        public int CurrentTowns()
        {
            return currentTownNumber;
        }

        public int InitialTowns()
        {
            return initialTownNumber;
        }

        public IFaction GetFaction()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == factionID)
                {
                    return faction;
                }
            }

            return null;
        }

        [SaveableField(1)] private string factionID;
        [SaveableField(2)] private int initialTownNumber = 0;
        [SaveableField(3)] private int currentTownNumber = 0;
        [SaveableField(4)] private bool hadRecentRevolt = false;
        [SaveableField(5)] private int daysSinceLastRevolt = 0;
        [SaveableField(6)] private Settlement revoltedSettlement;
    }
}

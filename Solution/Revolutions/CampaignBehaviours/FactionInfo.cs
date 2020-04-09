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
            _factionId = faction.StringId;
            foreach (var settlement in faction.Settlements)
            {
                if (settlement.IsTown)
                {
                    _initialTownNumber++;
                }
            }
        }

        public bool RevoltCanHappen()
        {
            return !_hadRecentRevolt;
        }

        public Settlement RevoltedSettlement()
        {
            return _revoltedSettlement;
        }

        public void UpdateFactionInfo()
        {
            UpdateCurrentTownCount();
            _daysSinceLastRevolt++;
            
            if (_daysSinceLastRevolt > 30)
            {
                _hadRecentRevolt = false;
                _revoltedSettlement = null;
            }
        }

        public void CityRevolted(Settlement settlement)
        {
            _hadRecentRevolt = true;
            _revoltedSettlement = settlement;
            _daysSinceLastRevolt = 0;
        }

        private void UpdateCurrentTownCount()
        {
            _currentTownNumber = 0;
            foreach (var settlement in GetFaction().Settlements)
            {
                if (settlement.IsTown)
                {
                    _currentTownNumber++;
                }
            }
        }

        public int TownsAboveInitial()
        {
            return _currentTownNumber - _initialTownNumber;
        }

        public int CurrentTowns()
        {
            return _currentTownNumber;
        }

        public int InitialTowns()
        {
            return _initialTownNumber;
        }

        public IFaction GetFaction()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == _factionId)
                {
                    return faction;
                }
            }

            return null;
        }

        [SaveableField(1)] private string _factionId;
        [SaveableField(2)] private int _initialTownNumber = 0;
        [SaveableField(3)] private int _currentTownNumber = 0;
        [SaveableField(4)] private bool _hadRecentRevolt = false;
        [SaveableField(5)] private int _daysSinceLastRevolt = 0;
        [SaveableField(6)] private Settlement _revoltedSettlement;
    }
}

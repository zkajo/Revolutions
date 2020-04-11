using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Revolutions.CampaignBehaviours
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

        public bool SuccessfulRevolt()
        {
            return _successfulRevolt;
        }

        public bool RevoltCanHappen()
        {
            return _canRevolt;
        }

        public Settlement RevoltedSettlement()
        {
            return _revoltedSettlement;
        }

        public void UpdateFactionInfo()
        {
            UpdateCurrentTownCount();
            _daysSinceLastRevolt++;
            
            if (_daysSinceLastRevolt > ModOptions.OptionsData.RevoltCooldownTime)
            {
                _canRevolt = true;
                _revoltedSettlement = null;
            }
        }

        public void CityRevoltedFailure(Settlement settlement)
        {
            _canRevolt = false;
            _revoltedSettlement = settlement;
            _daysSinceLastRevolt = 0;
            _successfulRevolt = false;
        }

        public void CityRevoltedSuccess(Settlement settlement)
        {
            _canRevolt = true;
            _revoltedSettlement = settlement;
            _successfulRevolt = true;
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

            if (Hero.MainHero.MapFaction.StringId == _factionId)
            {
                return Hero.MainHero.MapFaction;
            }
            
            return null;
        }

        [SaveableField(1)] private string _factionId;
        [SaveableField(2)] private int _initialTownNumber = 0;
        [SaveableField(3)] private int _currentTownNumber = 0;
        [SaveableField(4)] private bool _canRevolt = false;
        [SaveableField(5)] private int _daysSinceLastRevolt = 0;
        [SaveableField(6)] private Settlement _revoltedSettlement;
        [SaveableField(7)] private bool _successfulRevolt = false;
    }
}

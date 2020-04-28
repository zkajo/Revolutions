using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Revolutions.CampaignBehaviours
{
    public class FactionInfo
    {
        public string StringID => this._factionId;

        public IFaction Faction => this.GetFaction();

        public FactionInfo(IFaction faction)
        {
            this._factionId = faction.StringId;
            foreach (var settlement in faction.Settlements)
            {
                if (settlement.IsTown)
                {
                    this._initialTownNumber++;
                }
            }
        }

        public bool SuccessfulRevolt()
        {
            return this._successfulRevolt;
        }

        public bool RevoltCanHappen()
        {
            return this._canRevolt;
        }

        public Settlement RevoltedSettlement()
        {
            return this._revoltedSettlement;
        }

        public void UpdateFactionInfo()
        {
            this.UpdateCurrentTownCount();
            this._daysSinceLastRevolt++;

            if (this._daysSinceLastRevolt > ModOptions.OptionsData.RevoltCooldownTime)
            {
                this._canRevolt = true;
                this._revoltedSettlement = null;
            }
        }

        public void CityRevoltedFailure(Settlement settlement)
        {
            this._canRevolt = false;
            this._revoltedSettlement = settlement;
            this._daysSinceLastRevolt = 0;
            this._successfulRevolt = false;
        }

        public void CityRevoltedSuccess(Settlement settlement)
        {
            this._canRevolt = true;
            this._revoltedSettlement = settlement;
            this._successfulRevolt = true;
        }

        private void UpdateCurrentTownCount()
        {
            this._currentTownNumber = 0;
            foreach (var settlement in this.GetFaction().Settlements)
            {
                if (settlement.IsTown)
                {
                    this._currentTownNumber++;
                }
            }
        }

        public int TownsAboveInitial()
        {
            return this._currentTownNumber - this._initialTownNumber;
        }

        public int CurrentTowns()
        {
            return this._currentTownNumber;
        }

        public int InitialTowns()
        {
            return this._initialTownNumber;
        }

        private IFaction GetFaction()
        {
            foreach (var f in Campaign.Current.Factions)
            {
                if (f.StringId == this._factionId)
                {
                    return f;
                }
            }

            if (Hero.MainHero.MapFaction.StringId == this._factionId)
            {
                return Hero.MainHero.MapFaction;
            }

            return null;
        }

        [SaveableField(1)]
        private readonly string _factionId;

        [SaveableField(2)]
        private readonly int _initialTownNumber = 0;

        [SaveableField(3)]
        private int _currentTownNumber = 0;

        [SaveableField(4)]
        private bool _canRevolt = false;

        [SaveableField(5)]
        private int _daysSinceLastRevolt = 0;

        [SaveableField(6)]
        private Settlement _revoltedSettlement;

        [SaveableField(7)]
        private bool _successfulRevolt = false;
    }
}

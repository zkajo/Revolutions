using System;

namespace Revolutions
{
    [Serializable]
    public class Settings
    {
        public float RevoltCooldownTime = 30.0f;

        public bool EmpireLoyaltyMechanics = true;

        public bool OverextensionMechanics = true;

        public bool PlayerAffectedByOverextension = true;

        public int DaysUntilLoyaltyChange = 80;

        public bool AllowMinorFactions = false;

        public int MinimumObedianceLoyalty = 25;

        public int PlayerInTownLoyaltyIncrease = 5;
    }
}
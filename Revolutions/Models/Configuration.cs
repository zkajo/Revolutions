using System;

namespace Revolutions.Models
{
    [Serializable]
    public class Configuration
    {
        public float RevoltCooldownTime = 30.0f;

        public bool EmpireLoyaltyMechanics = true;

        public bool OverextensionMechanics = true;

        public bool PlayerAffectedByOverextension = true;

        public int DaysUntilLoyaltyChange = 80;

        public bool AllowMinorFactions = false;

        public bool DebugMode = false;
    }
}
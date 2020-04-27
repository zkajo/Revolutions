using TaleWorlds.SaveSystem;

namespace Revolutions
{
    public class ModOptionsData
    {
        [SaveableField(0)] public float RevoltCooldownTime = 30.0f;
        [SaveableField(1)] public bool EmpireLoyaltyMechanics = true;
        [SaveableField(2)] public bool OverextensionMechanics = true;
        [SaveableField(3)] public bool PlayerAffectedByOverextension = true;
        [SaveableField(4)] public int DaysUntilLoyaltyChange = 80;
        [SaveableField(5)] public bool AllowMinorFactions = false;
        [SaveableField(6)] public bool DebugMode = false;
    }
}
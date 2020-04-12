using TaleWorlds.SaveSystem;

namespace Revolutions
{
    public class ModOptionsData
    {
        [SaveableField(0)] public float RevoltCooldownTime = 30.0f;
        [SaveableField(1)] public bool EmpireLoyaltyMechanics = true;
        [SaveableField(2)] public bool OverextensionMechanics = true;
        [SaveableField(3)] public bool PlayerAffectedByOverextension = true;
    }
}
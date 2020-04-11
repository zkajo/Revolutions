using System.Runtime.CompilerServices;
using TaleWorlds.SaveSystem;

namespace Revolutions
{
    public class ModOptionsData
    {
        [SaveableField(0)] public float RevoltCooldownTime = 30.0f;
        [SaveableField(1)] public bool m_EmpireLoyaltyMechanics = true;
    }
}
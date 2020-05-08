using HarmonyLib;

namespace ModLibrary.Patches
{
    public static class Patcher
    {
        public static void PatchAll()
        {
            new Harmony("com.kntmods.bannerlord.revolutions").PatchAll();
        }
    }
}
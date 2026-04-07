using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using Godot;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen._Input))]
internal static class CharacterSelectScreenInputPatch
{
    private static void Postfix(NCharacterSelectScreen __instance, InputEvent inputEvent)
    {
        if (inputEvent is not InputEventKey keyEvent)
        {
            return;
        }

        if (!keyEvent.Pressed || keyEvent.Echo || keyEvent.Keycode != Key.B)
        {
            return;
        }
    }
}

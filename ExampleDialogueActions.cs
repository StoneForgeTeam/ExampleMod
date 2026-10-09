using StoneForge;
using StoneForge.Objects;

namespace ExampleMod;

// Registered automatically as examplemod:kill. Assign it in the NPC dialogue editor.
internal static class ExampleDialogueActions
{
    // Assign examplemod:kill_condition with Add condition in the NPC editor.
    [DialogCondition("kill_condition")]
    public static DialogConditionResult GetKillCondition(DialogOptionContext dialogue)
    {
        if (!dialogue.Speaker.Exists || dialogue.Speaker.Equals(dialogue.Player))
            return DialogConditionResult.Visible;
        if (dialogue.Speaker.Get("HP").AsReal <= 0)
            return DialogConditionResult.Visible;
        if (Game.CallScript("scr_gold_count", dialogue.Player, false, true).AsInt < 100)
            return DialogConditionResult.Visible;
        return DialogConditionResult.Enabled;
    }

    [DialogOption("kill")]
    public static void Kill(DialogOptionContext dialogue)
    {
        if (GetKillCondition(dialogue) != DialogConditionResult.Enabled)
            return;

        // Release the NPC conversation before running the game's death handling.
        if (dialogue.Panel.Exists)
            dialogue.Panel.Destroy();

        if (!dialogue.Speaker.Exists)
            return;

        var unit = dialogue.Speaker.As<o_unit>();

        dialogue.Speaker.Set("last_attacker", dialogue.Player);
        Combat.Damage(unit, DamageType.Pure, System.Math.Max(1, unit.HP.AsReal + 1), source: dialogue.Player.As<o_player>());

        if (dialogue.Speaker.Exists && dialogue.Speaker.Get("HP").AsReal <= 0)
        {
            Game.CallBuiltinAs("event_user", dialogue.Speaker, dialogue.Speaker, 6);
        }

        Game.CallScript("scr_gold_write_off", dialogue.Player, 100);
        Game.CallScript("scr_gold_count", dialogue.Player, false, true);
    }
}
